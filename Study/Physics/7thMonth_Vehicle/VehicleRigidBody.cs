using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class VehicleRigidBody : CustomRigidBody
{
    VehicleController controller;

    //바퀴들
    Wheel[] wheels;
    
    //접지 판정 임계값
    const float kMinCompression = 0.001f;
    //엔진 최대힘
    [SerializeField]
    float maxEngineForce;
    [SerializeField]
    float brakeStrength;

    [SerializeField] float frontTrack;
    [SerializeField] float rearTrack;

    //관성 모먼트
    float yawRate;
    float yawAccel;
    float yawTorque;

    [SerializeField] 
    float inertiaYaw; // Iz, 수직축 관성모멘트
    [SerializeField]
    private float stabilityGain = 1.5f;   // yawError → correctiveTorque 변환 비율, 튜닝용
    [SerializeField]
    private float maxBrakeTorque = 5000f; // 바퀴에 걸 수 있는 최대 브레이크 토크

    Vec3 cachedLocalAccel;


    public void SolveVehicle(float dt)
    {
        SolveWheelContacts();//상태 생성

        Vec3 localAccel = GetLocalAcceleration(dt);
        cachedLocalAccel = localAccel;
        //하중 계산
        ApplyLongitudinalLoad(localAccel);
        ApplyLateralLoad(localAccel);
       
        DistributeAxleLoads();       // 좌우 50:50 힘 분배
        PushLoadsToWheels();         // Wheel.normalForce 갱신
        UpdateNormalForces();//노멀 힘 업데이트

        SolveSuspension(dt);//force
        SolveTireForces(dt);//force

        ApplyStabilityControl(dt);//ESP
    }

    /// <summary>
    /// 엑셀 초기화
    /// </summary>
    void InitAxles()
    {
        frontAxle = new Axle(
            wheels[0], wheels[1],   // FL, FR
            isFront: true,
            trackWidth: frontTrack
        );

        rearAxle = new Axle(
            wheels[2], wheels[3],   // RL, RR
            isFront: false,
            trackWidth: rearTrack
        );
    }

    /// <summary>
    /// SweepSphere
    /// 접지 여부 판정
    /// contactPoint, normal 저장
    /// Compression 계산
    /// </summary>
    private void SolveWheelContacts()
    {
        for (int i=0;i< wheels.Length;i++)
        {
            Wheel wheel = wheels[i];

            // 1. 월드 공간 바퀴 중심
            Mat4 worldPos = transform3D.LocalToWorld;
            Vec3 wheelWorldPos = MatrixUtility.MulPoint(wheel.localPos, worldPos);

            // 2. Sweep 방향 (아래)
            Vec3 down = (-1) * transform3D.Up;   // 보통 -Y
            float maxSweep = wheel.suspension.restLength + wheel.radius;

            // 3. Sweep
            if (PhysicsWorld.Instance.SweepSphere(
                wheelWorldPos,
                wheel.radius,
                down,
                maxSweep,
                out SweepHit hit))
            {
                // 4. 접촉 정보
                wheel.contactNormal = hit.normal;
                wheel.contactPoint = hit.point;

                float distance = Vec3.Dot(hit.point - wheelWorldPos, (-1) * down);

                wheel.compression = wheel.suspension.restLength - distance;

                //접지 판정
                if (wheel.compression > kMinCompression) wheel.isGrounded = true;
                else
                {
                    wheel.isGrounded = false;
                    wheel.compression = 0;
                }

                //Hit는 났는데 서스펜션 축 기준이 아닐때 필터
                Vec3 suspensionDir = (-1f)*down;
                float alignment =Vec3.Dot(wheel.contactNormal, suspensionDir);

                if (alignment < 0.2f)//서스펜션 방향과 어긋난 접촉
                {
                    wheel.isGrounded = false;
                    wheel.compression = 0;
                }
            }
            else
            {
                wheel.isGrounded = false;
                wheel.compression = 0;
            }

            wheels[i] = wheel;//갱신
        }
    }



    /// <summary>
    /// 땅에 닿는 wheel만 처리
    /// spring+damper force 계산
    /// ApplyForceAtPoint
    /// </summary>
    /// <param name="dt"></param>
    private void SolveSuspension(float dt)
    {
        for (int i= 0; i<wheels.Length;i++)
        {
            Wheel wheel = wheels[i];//구조체라 값복사 문제가 있어 따로 빼야함

            //접지 상태가 아님
            if (!wheel.isGrounded)
            {
                continue;
            }

            //타이어 기준 좌표계
            Vec3 up = wheel.contactNormal;             // 접촉면 법선
            Vec3 forward = transform3D.Forward;
            Vec3 right = Vec3.Cross(up, forward).Normalized;
            forward = Vec3.Cross(right, up).Normalized;

            //접촉점 속도 분해
            Vec3 pointVel = GetVelocityAtPoint(wheel.contactPoint);
            float vLong = Vec3.Dot(pointVel, forward);
            float vLat = Vec3.Dot(pointVel, right);

            // 스프링 힘
            float springForce = wheel.suspension.stiffness * wheel.compression;
            
            // 감쇠력
            float compressionVel = Vec3.Dot(pointVel, wheel.contactNormal);

            float dampingForce = wheel.suspension.damping * compressionVel;

            //최종 힘
            Vec3 suspensionDir = wheel.contactNormal;
            float totalForce = springForce - dampingForce; // 부호는 이후 튜닝 가능
            Vec3 force = suspensionDir * totalForce;

            //바퀴에 힘 적용
            ApplyForceAtPoint(force, wheel.contactPoint);

            wheels[i] = wheel;
        }
    }
    /// <summary>
    /// 접촉점 속도 계산
    /// lateral / longitudinal 분해
    /// 마찰력 계산
    /// ApplyForceAtPoint
    /// </summary>
    /// <param name="dt"></param>
    private void SolveTireForces(float dt)
    {
        for (int i = 0; i < wheels.Length; i++)
        {
            Wheel wheel = wheels[i];//구조체라 따로 뺌

            //땅에 접지 안됨
            if (!wheel.isGrounded)
                continue;

            // 노멀포스
            float Fz = wheel.normalForce;
            if (Fz <= 0f) 
                continue;

            //타이어 3축 좌표계
            Vec3 up = wheel.contactNormal;
            Vec3 baseForward = transform3D.Forward;
            Mat3 steerRot = MatrixUtility.Rotate(wheel.steerAngle);//조향 회전

            Vec3 forward = steerRot * baseForward;//차량 전방
            forward = (forward - up * Vec3.Dot(forward, up)).Normalized;//접촉면에 투영(UP Dir Remove)
            Vec3 right = Vec3.Cross(up, forward).Normalized;//옆방향

            // Raw tire forces (Linear Model)
            float F_lat = -wheel.corneringStiffness * wheel.slipAngle;
            float F_long = wheel.longitudinalStiffness * wheel.slipRatio;

            // Friction Circle
            float maxForce = wheel.tireGrip * Fz;
            float forceMag = MathUtility.Root(F_lat * F_lat + F_long * F_long);

            // 힘 제한
            if (forceMag > maxForce)
            {
                float scale = maxForce / forceMag;
                F_lat *= scale;
                F_long *= scale;
            }

            // 바퀴 기준 힘 → 월드 힘
            Vec3 wheelForward = forward;
            Vec3 wheelRight = right;

            Vec3 tireForce = wheelForward * F_long + wheelRight * F_lat;

            //Yaw
            Vec3 r = wheel.contactPoint - WorldCenterOfMass;
            Vec3 torque = Vec3.Cross(r, tireForce);

            yawTorque += torque.y; // 월드 Y 기준

            //차체 힘 적용
            ApplyForceAtPoint(tireForce, wheel.contactPoint);

            wheels[i] = wheel;//값 갱신
        }

        // 모든 바퀴 계산 끝난 뒤
        yawAccel = yawTorque / inertiaYaw;
        yawRate += yawAccel * dt;

        // rigidbody 각속도에 반영
        angularVelocity.y = yawRate;

        // 프레임 끝
        yawTorque = 0f;
    }
    /// <summary>
    /// 노말 방향 힘 업데이트
    /// </summary>
    public void UpdateNormalForces()
    {
        for(int i = 0; i < wheels.Length; i++)
        {
            Wheel w = wheels[i];
            //노말 방향 힘 : 정적 방향 + 동적 방향
            w.normalForce = MathUtility.Max(w.staticLoad + w.dynamicLoad,0f);
            wheels[i] = w;
        }
    }

    /// <summary>
    /// 힘 분배
    /// </summary>
    public void DistributeAxleLoads()
    {
        frontAxle.DistributeLongitudinal();
        rearAxle.DistributeLongitudinal();
    }
    /// <summary>
    /// 바퀴에 힘 싣기
    /// </summary>
    void PushLoadsToWheels()
    {
        frontAxle.PushToWheels();
        rearAxle.PushToWheels();
    }

    /// <summary>
    /// 횡하중 계산
    /// </summary>
    /// <param name="localAccel"></param>
    void ApplyLateralLoad(Vec3 localAccel)
    {
        float ay = localAccel.x; // 로컬 X = 좌우 가속

        float deltaFzFront = (mass.value * ay * cgHeight) / frontTrack;
        float deltaFzRear = (mass.value * ay * cgHeight) / rearTrack;

        frontAxle.ApplyLateralLoad(deltaFzFront);
        rearAxle.ApplyLateralLoad(deltaFzRear);
    }
    /// <summary>
    /// Stability Control / ESP
    /// yawRate vs 실제 yawRate 비교 후 바퀴별 brakeTorque 적용
    /// 차량 안정화 - 차량이 원하는 만큼만 회전
    /// </summary>
    /// <param name="dt"></param>
    void ApplyStabilityControl(float dt)
    {
        float desiredYawRate = controller.SteerInput * velocity.Magnitude / GetTurnRadius();
        float yawError = desiredYawRate - yawRate;

        //양수면 차가 덜 돌고 있음 → 바깥쪽 바퀴 브레이크 증가
        //음수면 차가 너무 돌고 있음 → 안쪽 바퀴 브레이크 증가
        float correctiveTorque = yawError * stabilityGain;
        correctiveTorque = MathUtility.ClampValue(correctiveTorque, -maxBrakeTorque, maxBrakeTorque);//-1~1 범위

        // 바퀴별 Brake Torque 분배 (예: 좌우)
        frontAxle.ApplyBrakeSplit(correctiveTorque * 0.5f);
        rearAxle.ApplyBrakeSplit(correctiveTorque * 0.5f);
    }
    /// <summary>
    /// 바퀴의 회전 각도 구하는 함수
    /// </summary>
    /// <returns></returns>
    private float GetTurnRadius()
    {
        float steerAngleRad = MathUtility.Deg2Rad * frontAxle.GetAverageSteerAngle();//rad각도로 변환
        if (MathUtility.Abs(steerAngleRad) < 0.001f) return float.MaxValue; // 직진
        return wheelBase / Mathf.Tan(steerAngleRad);
    }
}
