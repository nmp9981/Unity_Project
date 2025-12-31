using System;
using UnityEngine;

class FixedJoint : Joint
{
    //각 쿨체의 로컬 축
    Vec3 localAnchorA;
    Vec3 localAnchorB;

    //생성자
    public FixedJoint(CustomRigidBody a, CustomRigidBody b, Vec3 worldAnchor)
    {
        rigidA = a;
        rigidB = b;

        localAnchorA = rigidA.WorldToLocal(worldAnchor);
        localAnchorB = rigidB.WorldToLocal(worldAnchor);
    }

    //Solver 구현 : 추상 클래스 상속 후 구현
    public override void SolveVelocity(float dt)
    {
        SolveLinearVelocity();
        SolveAngularVelocity();
    }

    public override void SolvePosition(float dt)
    {
        SolveLinearPosition();
        SolveAngularPosition();
    }
    /// <summary>
    /// 선속 Solver
    /// </summary>
    void SolveLinearVelocity()
    {
        //축 위치 계산
        Vec3 rA = rigidA.rotation * localAnchorA;
        Vec3 rB = rigidB.rotation * localAnchorB;

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
        Vec3 wRel = rigidB.angularVelocity - rigidA.angularVelocity;

        float Cdot = Vec3.Dot(wRel, axis);
        float lambda = -Cdot * angularEffectiveMass;

        Vec3 impulse = axis*lambda;

        rigidA.angularVelocity -= impulse* rigidA.invInertia;
        rigidB.angularVelocity += impulse* rigidB.invInertia;
    }

    void SolveLinearPosition()
    {

    }
    void SolveAngularPosition()
    {

    }
}
