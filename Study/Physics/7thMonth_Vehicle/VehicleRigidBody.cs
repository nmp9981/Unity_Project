using System.Runtime.CompilerServices;
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

    public void SolveVehicle(float dt)
    {
        SolveWheelContacts();//상태 생성
        SolveSuspension(dt);//force
        SolveTireForces(dt);//force
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

            //차체 힘 적용
            ApplyForceAtPoint(tireForce, wheel.contactPoint);

            wheels[i] = wheel;//값 갱신
        }
    }

    /// <summary>
    /// 바퀴 시각화
    /// </summary>
    /// <param name="wheel"></param>
    /// <param name="vLong"></param>
    /// <param name="dt"></param>
    void VisualWheel(Wheel wheel, float vLong,float dt)
    {
        wheel.angularVelocity += vLong / wheel.radius;
        //wheelVisual.Rotate(wheel.angularVelocity * dt);
    }
}
