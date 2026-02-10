using UnityEngine;

/// <summary>
/// Suspension
/// 바퀴가 땅을 향해 쏜 Sweep의 결과를 힘으로 바꾸는 장치
/// </summary>
public struct Suspension
{
    public float restLength;   // 서스펜션 자연 길이
    public float maxLength;    // 최대 늘어남

    public float stiffness;    // 스프링 상수 (k)
    public float damping;      // 감쇠 계수 (c)
}

/// <summary>
/// 바퀴 구조체
/// 차량–지면 인터페이스 상태 묶음(State Container)
/// 휠은 전부 계산용 가상 구조체
/// </summary>
public struct Wheel
{
    //운전중 여부
    public bool isDriven;

    // === 고정 파라미터 (설계값) ===
    public Vec3 localPos;        // CoM 기준 휠 위치
    public float radius;        // 바퀴 반지름
    public float restLength;    // 서스펜션 기본 길이

    public Suspension suspension;

    // === 프레임별 상태값 === runtime
    public bool isGrounded;      // 접촉 중인가?
    public float compression; // 현재 서스펜션 길이
    public float normalForce;             // Fz (서스펜션 결과)

    //하중 관련
    public float staticLoad;    // 정적 하중
    public float dynamicLoad;   // 가속도 기반 추가 하중

    //접촉점 기준 좌표
    public Vec3 contactPoint;   // 접촉점
    public Vec3 contactNormal;  // 접촉 법선

    //tire
    public float corneringStiffness;      // C_alpha
    public float longitudinalStiffness;   // C_sigma
    public float tireGrip;//마찰 계수
    public float steerAngle;//조향,radian

    //회전
    public float angularVelocity;

    //바퀴 관성
    public float wheelInertia; 

    //슬립 핵심
    public float slipRatio;   // 종방향
    public float slipAngle;   // 횡방향

    //토크 값
    public float driveTorque;   // 엔진 or 모터
    public float brakeTorque;  // 브레이크

    //디버깅용, 슬립 입력값
    public float vLong;
    public float vLat;
}

/// <summary>
/// 엑셀 클래스
/// </summary>
public class Axle
{
    public Wheel left;//왼쪽 바퀴
    public Wheel right;//오른쪽 바퀴

    public bool isFront;      // 전륜 / 후륜
    public float trackWidth;  // 좌우 거리

    public float lateralLoad;//누적 하중
    public float staticLoad;
    public float dynamicLoad;
    public float totalLoad;

    public float leftLoad;
    public float rightLoad;

    public Axle(Wheel left, Wheel right, bool isFront, float trackWidth)
    {
        this.left = left;
        this.right = right;
        this.isFront = isFront;
        this.trackWidth = trackWidth;
    }

    /// <summary>
    /// 힘 분배
    /// </summary>
    public void DistributeLongitudinal()
    {
        leftLoad = (staticLoad + dynamicLoad) * 0.5f;
        rightLoad = (staticLoad + dynamicLoad) * 0.5f;
    }
    /// <summary>
    /// 바퀴 normal 힘 계산
    /// </summary>
    public void PushToWheels()
    {
        left.normalForce = Mathf.Max(leftLoad, 0f);
        right.normalForce = Mathf.Max(rightLoad, 0f);
    }
    /// <summary>
    /// 좌우 하중 분배
    /// </summary>
    /// <param name="deltaFz"></param>
    public void ApplyLateralLoad(float deltaFz)
    {
        float perWheel = deltaFz * 0.5f;

        left.dynamicLoad += perWheel;
        right.dynamicLoad -= perWheel;
    }
}


public class Vehicle
{
    public CustomRigidBody body;
    public PhysicsWorld physicsWorld;
    public Wheel[] wheels;

    //차량 기준 축
    Vec3 forward;
    Vec3 right;
    Vec3 up;

    public void Update(float dt)
    {
        UpdateBasis();

        // 1️. 바퀴 회전부터
        for (int i = 0; i < wheels.Length; i++)
        {
            UpdateWheelAngularVelocity(ref wheels[i], dt);
        }
        //2. 이후 슬립 계산
        UpdateWheelSlip(dt);

        //3. 디버그
        OnDrawGizmos();

        //4. 힘 적용
    }

    /// <summary>
    /// 기준 축 업데이트
    /// </summary>
    void UpdateBasis()
    {
        forward = body.transform3D.Forward;
        right = body.transform3D.Right;
        up = body.transform3D.Up;
    }

    void UpdateWheelSlip(float dt)
    {
        const float eps = 0.1f;

        for (int i = 0; i < wheels.Length; i++)
        {
            Wheel w = wheels[i];//구조체

            //땅에 닿지 않음
            if (!w.isGrounded)
            {
                w.vLong = 0f;
                w.vLat = 0f;
                w.slipAngle = 0f;
                w.slipRatio = 0f;
                wheels[i] = w;
                continue;
            }

            // 1️⃣ 접촉점 속도
            Vec3 v = body.GetVelocityAtPoint(w.contactPoint);

            // 2️⃣ 바퀴 로컬 축
            // 조향 적용
            Vec3 wheelForward = forward;
            Vec3 wheelRight = Vec3.Cross(wheelForward,up);

            // 3️⃣ 속도 분해
            float vLong = Vec3.Dot(v, wheelForward);
            float vLat = Vec3.Dot(v, wheelRight);

            w.vLong = vLong;
            w.vLat = vLat;

            // 4️⃣ Lateral Slip Angle (오늘의 핵심 결과물)
            w.slipAngle = MathUtility.Atan2(vLat,MathUtility.Abs(vLong) + eps);
            if (w.steerAngle != 0f)//이때 회전
            {
                wheelForward = QuaternionUtility.AngleAxis(w.steerAngle * MathUtility.Rad2Deg,up) * forward;
            }

            // 5️⃣ Longitudinal Slip은 Day2에서 계산
            float wheelSurfaceSpeed = w.angularVelocity * w.radius;
            w.slipRatio = (wheelSurfaceSpeed - vLong) / MathUtility.Max(MathUtility.Abs(vLong), eps);

            wheels[i] = w;//구조체 값 갱신
        }
    }

    /// <summary>
    /// 디버그 용 기즈모
    /// </summary>
    void OnDrawGizmos()
    {
        foreach (var w in wheels)
        {
            if (!w.isGrounded) continue;

            //좌표 변환
            Vector3 contactPos = new Vector3(w.contactPoint.x, w.contactPoint.y, w.contactPoint.z);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(
                contactPos,
                contactPos + body.transform3D.Forward.ToUnity * w.vLong * 0.1f
            );

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(
                contactPos,
                contactPos + body.transform3D.Right.ToUnity * w.vLat * 0.1f
            );
        }
    }

    /// <summary>
    /// 바퀴 각속도 업데이트
    /// </summary>
    /// <param name="w"></param>
    /// <param name="dt"></param>
    void UpdateWheelAngularVelocity(ref Wheel w, float dt)
    {
        float torque = 0f;

        //구동 토크
        if (w.isDriven) torque += w.driveTorque;

        //브레이크 토크(회전 반대 방향)
        torque -= MathUtility.Sign(w.angularVelocity)* w.brakeTorque;

        //각 가속도
        float angularAccel = torque / w.wheelInertia;

        //적분
        w.angularVelocity += (angularAccel * dt);

        //정지 안전 장치
        if (MathUtility.Abs(w.angularVelocity) < 0.5f && MathUtility.Abs(w.driveTorque) < 0.01f && w.brakeTorque > 0f)
        {
            w.angularVelocity = 0f;
        }
    }

    //삭제
    void SolveWheel(Wheel wheel, float dt)
    {
        // 1. 월드 공간 바퀴 중심
        Mat4 worldPos = body.transform3D.LocalToWorld;
        Vec3 wheelWorldPos = MatrixUtility.MulPoint(wheel.localPos, worldPos);

        // 2. Sweep 방향 (아래)
        Vec3 down = (-1) * body.transform3D.Up;   // 보통 -Y

        float maxSweep =
            wheel.suspension.restLength + wheel.radius;

        // 3. Sweep
        if (physicsWorld.SweepSphere(
            wheelWorldPos,
            wheel.radius,
            down,
            maxSweep,
            out SweepHit hit))
        {
            wheel.isGrounded = true;

            // 4. 접촉 정보
            wheel.contactNormal = hit.normal;
            wheel.contactPoint = hit.point;

            float distance = Vec3.Dot(hit.point - wheelWorldPos, (-1) * down);

            wheel.compression =
                wheel.suspension.restLength - distance;

            ApplySuspensionForce(wheel, dt);
        }
        else
        {
            wheel.isGrounded = false;
            wheel.compression = 0;
        }
    }
    //삭제
    void ApplySuspensionForce(Wheel wheel, float dt)
    {
        if (wheel.compression <= 0)
            return;

        // 1. 스프링 힘
        float springForce = wheel.suspension.stiffness * wheel.compression;

        // 2. 감쇠력
        Vec3 pointVel = body.GetVelocityAtPoint(wheel.contactPoint);

        float compressionVel = Vec3.Dot(pointVel, wheel.contactNormal);

        float dampingForce = wheel.suspension.damping * compressionVel;

        // 3. 최종 힘
        float force =
            springForce - dampingForce;

        if (force < 0)
            force = 0;

        Vec3 finalForce =
            wheel.contactNormal * force;

        // 4. 차체에 적용
        body.ApplyForceAtPoint(
            finalForce,
            wheel.contactPoint
        );
    }
}
