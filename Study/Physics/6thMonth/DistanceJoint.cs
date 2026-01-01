using Unity.Hierarchy;

class DistanceJoint : Joint
{
    //각 쿨체의 로컬 축
    Vec3 localAnchorA;
    Vec3 localAnchorB;

    float restDistance;//길이
    float accumulatedImpulse;

    // Position correction 계수
    const float beta = 0.2f;
    const float slop = 0.001f;

    //생성자
    public DistanceJoint(CustomRigidBody a, CustomRigidBody b, Vec3 worldAnchorA, Vec3 worldAnchorB)
    {
        rigidA = a;
        rigidB = b;

        localAnchorA = rigidA.WorldToLocal(worldAnchorA);
        localAnchorB = rigidB.WorldToLocal(worldAnchorB);

        Vec3 delta = worldAnchorB - worldAnchorA;
        restDistance = delta.Magnitude;
    }

    //Solver 구현 : 추상 클래스 상속 후 구현
    //속도는 미리 제어, 위치는 과거 오차 바로 잡기
    public override void SolveVelocity(float dt)
    {
        SolveDistanceVelocity(dt);
    }

    public override void SolvePosition(float dt)
    {
        SolveDistancePosition(dt);
    }
    /// <summary>
    /// 속도 Solver
    /// </summary>
    void SolveDistanceVelocity(float dt)
    {
        //상대 위치 구하기, aanchor -> world
        Vec3 rA = Vec3.Rotation3DVec(rigidA.rotation , localAnchorA);
        Vec3 rB = Vec3.Rotation3DVec(rigidB.rotation , localAnchorB);

        Vec3 pA = rigidA.position + rA;
        Vec3 pB = rigidB.position + rB;

        //거리
        Vec3 delta = pB - pA;
        float dist = delta.Magnitude;
        if (dist < 1e-6f) return;//거리 변화량이 없음

        Vec3 axis = delta / dist;//축 계산

        //상대 속도 구하기
        Vec3 vRel =
            rigidB.velocity + Vec3.Cross(rigidB.angularVelocity, rB)
          - rigidA.velocity - Vec3.Cross(rigidA.angularVelocity, rA);

        //축 방향으로 상대 속도 투영
        float Cdot = Vec3.Dot(vRel, axis);

        //effective mass
        float k =
            rigidA.invMass + rigidB.invMass
          + Vec3.Dot(Vec3.Cross(rA, axis), Vec3.Cross(rA, axis)) * rigidA.invInertia
          + Vec3.Dot(Vec3.Cross(rB, axis), Vec3.Cross(rB, axis)) * rigidB.invInertia;

        if (k <= 0.0f) return;

        float beta = 0.2f;
        float lambda = -Cdot / k; 
        
        //해당 축 방향으로 impuse를 구함
        Vec3 impulse = axis * lambda;

        rigidA.velocity -= impulse * rigidA.invMass;
        rigidB.velocity += impulse * rigidB.invMass;

        rigidA.angularVelocity -= Vec3.Cross(rA, impulse) * rigidA.invInertia;
        rigidB.angularVelocity += Vec3.Cross(rB, impulse) * rigidB.invInertia;
    }
    /// <summary>
    /// 위치 Solver
    /// </summary>
    void SolveDistancePosition(float dt)
    {
        //상대 회전
        Vec3 rA = Vec3.Rotation3DVec(rigidA.rotation, localAnchorA);
        Vec3 rB = Vec3.Rotation3DVec(rigidB.rotation, localAnchorB);

        //상대 위치
        Vec3 pA = rigidA.position + rA;
        Vec3 pB = rigidB.position + rB;

        Vec3 delta = pB - pA;
        float dist = delta.Magnitude;
        if (dist < 1e-6f) return;

        //제약 오차
        float C = dist - restDistance;
        //축 계산
        Vec3 axis = delta / dist;

        //effective Mass
        float k =
            rigidA.invMass + rigidB.invMass
          + Vec3.Dot(Vec3.Cross(rA, axis), Vec3.Cross(rA, axis)) * rigidA.invInertia
          + Vec3.Dot(Vec3.Cross(rB, axis), Vec3.Cross(rB, axis)) * rigidB.invInertia;

        if (k <= 0.0f) return;

        //보정량
        float lambda = -(beta / dt) * C / k;
        Vec3 correction = axis * lambda;

        //위치 보정
        rigidA.position -= correction * rigidA.invMass;
        rigidB.position += correction * rigidB.invMass;

        //회전 보정
        rigidA.rotation = QuaternionUtility.IntegrateRotation(
            rigidA.rotation,
            Vec3.Cross(rA, correction) * (-rigidA.invInertia),
            1.0f
        );

        rigidB.rotation = QuaternionUtility.IntegrateRotation(
            rigidB.rotation,
            Vec3.Cross(rB, correction) * rigidB.invInertia,
            1.0f
        );
    }
}
