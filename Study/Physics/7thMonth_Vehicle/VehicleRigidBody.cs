using UnityEngine;
using UnityEngine.InputSystem.HID;

public class VehicleRigidBody : CustomRigidBody
{
    Wheel[] wheels;

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

                wheel.compression = wheel.suspension.restLength - distance;
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
            // 1. 스프링 힘
            float springForce = wheel.suspension.stiffness * wheel.compression;

            // 2. 감쇠력
            Vec3 pointVel = GetVelocityAtPoint(wheel.contactPoint);

            float compressionVel = Vec3.Dot(pointVel, wheel.contactNormal);

            float dampingForce = wheel.suspension.damping * compressionVel;

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

    }
}
