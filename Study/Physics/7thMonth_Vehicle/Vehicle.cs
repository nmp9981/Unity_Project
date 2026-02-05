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

    //접촉점 기준 좌표
    public Vec3 contactPoint;   // 접촉점
    public Vec3 contactNormal;  // 접촉 법선

    //tire
    public float tireGrip;
    public float steerAngle;//조향
    public float angularValocity;
}

public class Vehicle
{
    public CustomRigidBody body;
    public PhysicsWorld physicsWorld;
    public Wheel[] wheels;

    public void Update(float dt)
    {
        foreach (var wheel in wheels)
        {
            SolveWheel(wheel, dt);
        }
    }
    //삭제
    void SolveWheel(Wheel wheel, float dt)
    {
        // 1. 월드 공간 바퀴 중심
        Mat4 worldPos = body.transform3D.LocalToWorld;
        Vec3 wheelWorldPos = MatrixUtility.MulPoint(wheel.localPos,worldPos);

        // 2. Sweep 방향 (아래)
        Vec3 down = (-1)*body.transform3D.Up;   // 보통 -Y

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

            float distance =Vec3.Dot(hit.point - wheelWorldPos,(-1)*down);

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
