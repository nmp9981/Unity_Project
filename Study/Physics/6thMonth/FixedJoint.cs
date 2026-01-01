using System;
using UnityEngine;

class FixedJoint : Joint
{
    //각 쿨체의 로컬 축
    Vec3 localAnchorA;
    Vec3 localAnchorB;

    float effectiveMass;
    float angularEffectiveMass;

    //생성자
    public FixedJoint(CustomRigidBody a, CustomRigidBody b, Vec3 worldAnchor)
    {
        rigidA = a;
        rigidB = b;

        localAnchorA = rigidA.WorldToLocal(worldAnchor);
        localAnchorB = rigidB.WorldToLocal(worldAnchor);
    }

    //Solver 구현 : 추상 클래스 상속 후 구현
    //속도는 미리 제어, 위치는 과거 오차 바로 잡기
    public override void SolveVelocity(float dt)
    {
        SolveLinearVelocity();
        SolveAngularVelocity();
    }

    public override void SolvePosition(float dt)
    {
        SolveLinearPosition();
        SolveAngularPosition(dt);
    }
    /// <summary>
    /// 선속 Solver
    /// </summary>
    void SolveLinearVelocity()
    {
        //축 위치 계산
        Vec3 rA = Vec3.Rotation3DVec(rigidA.rotation ,localAnchorA);
        Vec3 rB = Vec3.Rotation3DVec(rigidB.rotation , localAnchorB);

        Vec3 pA = rigidA.position + rA;
        Vec3 pB = rigidB.position + rB;

        //상대 속도 계산
        Vec3 vRel = rigidB.velocity + Vec3.Cross(rigidB.angularVelocity, rB) - rigidA.velocity - Vec3.Cross(rigidA.angularVelocity, rA);

        //각 축별로 Constraint 적용
        ApplyLinearConstraint(VectorMathUtils.RightVector3D(), vRel,rA,rB);
        ApplyLinearConstraint(VectorMathUtils.UpVector3D(),vRel,rA,rB);
        ApplyLinearConstraint(VectorMathUtils.FrontVector3D(),vRel,rA,rB);
    }

    /// <summary>
    /// 각속도 Solver
    /// </summary>
    void SolveAngularVelocity()
    {
        ApplyAngularConstraint(VectorMathUtils.RightVector3D());
        ApplyAngularConstraint(VectorMathUtils.UpVector3D());
        ApplyAngularConstraint(VectorMathUtils.FrontVector3D());
    }

    /// <summary>
    /// 주어진 축에대해 Constraint적용
    /// </summary>
    /// <param name="axis"></param>
    void ApplyLinearConstraint(Vec3 axis, Vec3 vRel, Vec3 rA, Vec3 rB)
    {
        effectiveMass = 1.0f / (rigidA.invMass + rigidB.invMass);

        //상대속도를 축에 투영
        float Cdot = Vec3.Dot(vRel, axis);
        float lambda = -Cdot * effectiveMass;

        Vec3 impulse = axis*lambda;

        rigidA.velocity -= impulse * rigidA.invMass;
        rigidB.velocity += impulse * rigidB.invMass;

        rigidA.angularVelocity -= Vec3.Cross(rA, impulse) * rigidA.invInertia;
        rigidB.angularVelocity += Vec3.Cross(rB, impulse) * rigidB.invInertia;
    }
    
  
    void ApplyAngularConstraint(Vec3 axis)
    {
        angularEffectiveMass = 1.0f / (rigidA.invInertia + rigidB.invInertia);
        Vec3 wRel = rigidB.angularVelocity - rigidA.angularVelocity;

        float Cdot = Vec3.Dot(wRel, axis);
        float lambda = -Cdot * angularEffectiveMass;

        Vec3 impulse = axis*lambda;

        rigidA.angularVelocity -= impulse* rigidA.invInertia;
        rigidB.angularVelocity += impulse* rigidB.invInertia;
    }

    /// <summary>
    /// 틀어진 위치를 되돌림
    /// 두 anchor는 항상 같은 위치
    /// </summary>
    void SolveLinearPosition()
    {
        // 월드 anchor
        Vec3 rA = Vec3.Rotation3DVec(rigidA.rotation, localAnchorA);
        Vec3 rB = Vec3.Rotation3DVec(rigidB.rotation, localAnchorB);

        Vec3 pA = rigidA.position + rA;
        Vec3 pB = rigidB.position + rB;

        // position error, 제약식, drift
        Vec3 C = pB - pA;

        // 허용 오차 (slop), 두 물체의 anchor위치가 같은지 검사
        float slop = 0.001f;
        if (C.Square < slop * slop)//제약식 만족이라 위치 조절 필요없음
            return;

        // Baumgarte 계수
        float beta = 0.2f;

        // effective mass (선형만)
        float invMassSum = rigidA.invMass + rigidB.invMass;
        if (invMassSum == 0.0f)//둘다 정적인 물체
            return;

        Vec3 correction = -(beta / invMassSum) * C;

        rigidA.position -= correction * rigidA.invMass;
        rigidB.position += correction * rigidB.invMass;
    }
    /// <summary>
    /// 틀어진 회전을 되돌림
    /// 두 rigid의 상대 회전이 0이 되게 만들어라
    /// 축+각도로 생각
    /// Fixed Joint는 회전 자유도도 0
    /// </summary>
    void SolveAngularPosition(float dt)
    {
        CustomQuaternion qA = rigidA.rotation;
        CustomQuaternion qB = rigidB.rotation;

        //qerror​=qB​⋅qA^(-1) : 상대 회전
        // 현재 상대 회전
        CustomQuaternion qRel = QuaternionUtility.Inverse(qA)*qB;

        // 초기 상대 회전은 joint 생성 시 저장해둔다고 가정
        CustomQuaternion qError =
            qRel * QuaternionUtility.Inverse(initialRelativeRotation);

        // 작은 각도 근사
        Vec3 axisError = qError.vec*2.0f;

        float slop = 0.001f;
        if (axisError.Square < slop * slop)
            return;

        float beta = 0.2f;
        Vec3 correction = -beta * axisError;

        rigidA.rotation = IntegrateRotation(rigidA.rotation, correction * rigidA.invInertia*(-1), dt);
        rigidB.rotation = IntegrateRotation(rigidB.rotation, correction * rigidB.invInertia, dt);
    }
    /// <summary>
    /// 회전 적분
    /// </summary>
    /// <param name="rot"></param>
    /// <param name="corr"></param>
    /// <param name="dt"></param>
    /// <returns></returns>
    CustomQuaternion IntegrateRotation(CustomQuaternion rot, Vec3 corr, float dt)
    {
        CustomQuaternion omegaQ = new CustomQuaternion(0.0f, corr);
        CustomQuaternion dq = rot * omegaQ;
        CustomQuaternion nextRot = rot + dq * (0.5f * dt);
        return nextRot.Normalized;
    }
}
