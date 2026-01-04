using System;
using UnityEngine.Animations;

class HingeJoint : Joint
{
    //각 쿨체의 로컬 축
    Vec3 localAnchorA;
    Vec3 localAnchorB;

    float effectiveMass;
    float angularEffectiveMass;

    // joint가 허용하는 회전축 (로컬)
    Vec3 localAxisA;
    Vec3 localAxisB;

    // ⟂ 각속도 제약
    float angularPerpImpulse1;
    float angularPerpImpulse2;

    // 각도 제한
    bool enableAngularLimit;
    float angularLimitImpulse;
    float minAngle;
    float maxAngle;

    // 모터
    bool enableMotor;
    float motorSpeed;
    float angularMotorImpulse;
    float maxAngularMotorTorque;

    //로컬 축
    Vec3 localPerpAxisA;
    Vec3 localPerpAxisB;

    //생성자
    public HingeJoint(CustomRigidBody a, CustomRigidBody b, Vec3 worldAnchor, Vec3 worldAxis)
    {
        rigidA = a;
        rigidB = b;

        localAnchorA = rigidA.WorldToLocal(worldAnchor);
        localAnchorB = rigidB.WorldToLocal(worldAnchor);

        localAxisA = rigidA.WorldToLocalDirection(worldAxis).Normalized;
        localAxisB = rigidB.WorldToLocalDirection(worldAxis).Normalized;

        // 힌지 축과 직교하는 기준 벡터 생성
        Vec3 temp;
        if (Math.Abs(localAxisA.x) < 0.9f)
            temp = Vec3.Cross(localAxisA, VectorMathUtils.RightVector3D());
        else
            temp = Vec3.Cross(localAxisA, VectorMathUtils.UpVector3D());

        localPerpAxisA = temp.Normalized;

        // B도 동일 (월드 기준으로 맞춰주는 게 중요)
        localPerpAxisB = rigidB.WorldToLocalDirection(Vec3.Rotation3DVec(rigidA.rotation, localPerpAxisA));
    }

    //Solver 구현 : 추상 클래스 상속 후 구현
    //속도는 미리 제어, 위치는 과거 오차 바로 잡기
    public override void SolveVelocity(float dt)
    {
        SolveLinearVelocity();           // anchor 속도 제거
        SolveAngularVelocityPerp();      // ⟂ 회전 제거
        SolveAngularVelocityLimit();     // 각속도 제한
        SolveAngularVelocityMotor(dt);   // 모터
    }

    public override void SolvePosition(float dt)
    {
        SolveLinearPosition();
        SolveAngularPositionHinge(dt);
        SolveAngularPositionLimit();  // 각도 범위 제한
    }

    public override void WarmStart(float dt)
    {
        GetHingeBasis(out Vec3 axis, out Vec3 n1, out Vec3 n2);
     
        // 🔹 Angular Perp Warm Start
        ApplyAngularImpulse(n1, angularPerpImpulse1);
        ApplyAngularImpulse(n2, angularPerpImpulse2);

        // 🔹 Angular Limit Warm Start, 방향이 맞아야함
        float angle = CurrentHingeAngle();

        if (angle < minAngle && angularLimitImpulse > 0)
        {
            ApplyAngularImpulse(axis, angularLimitImpulse);
        }
        else if (angle > maxAngle && angularLimitImpulse < 0)
        {
            ApplyAngularImpulse(axis, angularLimitImpulse);
        }
        else
        {
            // 방향이 맞지 않으면 폐기
            angularLimitImpulse = 0.0f;
        }

        // 🔹 Angular Motor Warm Start, 방향이 맞아야함
        if ((angle <= minAngle && angularMotorImpulse < 0) || (angle >= maxAngle && angularMotorImpulse > 0))
        {
            angularMotorImpulse = 0;
        }
        else
        {
            ApplyAngularImpulse(axis, angularMotorImpulse);
        }
    }
    public override void OnWake()
    {
        // Angular ⟂ 제거 impulse
        angularPerpImpulse1 = 0.0f;
        angularPerpImpulse2 = 0.0f;

        // Angular limit
        angularLimitImpulse = 0.0f;

        // Angular motor
        angularMotorImpulse = 0.0f;
    }
    #region Velocity
    /// <summary>
    /// 선속 Solver
    /// </summary>
    void SolveLinearVelocity()
    {
        //축 위치 계산
        Vec3 rA = Vec3.Rotation3DVec(rigidA.rotation, localAnchorA);
        Vec3 rB = Vec3.Rotation3DVec(rigidB.rotation, localAnchorB);

        Vec3 pA = rigidA.position + rA;
        Vec3 pB = rigidB.position + rB;

        //상대 속도 계산
        Vec3 vRel = rigidB.velocity + Vec3.Cross(rigidB.angularVelocity, rB) - rigidA.velocity - Vec3.Cross(rigidA.angularVelocity, rA);

        //각 축별로 Constraint 적용
        ApplyLinearConstraint(VectorMathUtils.RightVector3D(), vRel, rA, rB);
        ApplyLinearConstraint(VectorMathUtils.UpVector3D(), vRel, rA, rB);
        ApplyLinearConstraint(VectorMathUtils.FrontVector3D(), vRel, rA, rB);
    }

    /// <summary>
    /// 각속도 Solver
    /// </summary>
    void SolveAngularVelocityPerp()
    {
        GetHingeBasis(out Vec3 axis, out Vec3 n1, out Vec3 n2);

        // 상대 각속도
        Vec3 wRel = rigidB.angularVelocity - rigidA.angularVelocity;

        //n1,n2는 회전 방향이 아니므로 움직이면 안됨
        ApplyAngularVelocityConstraint(n1, wRel);
        ApplyAngularVelocityConstraint(n2, wRel);
    }
    /// <summary>
    /// 각속도 제한
    /// </summary>
    void SolveAngularVelocityLimit()
    {
        if (!enableAngularLimit) return;

        GetHingeBasis(out Vec3 axis, out Vec3 n1, out Vec3 n2);

        //현재 힌지 각도
        float angle = CurrentHingeAngle();
        //상대 각속도
        float wRel = Vec3.Dot(rigidB.angularVelocity - rigidA.angularVelocity, axis);
        
        //조건부 활성화
        bool lowerActive = (angle <= minAngle && wRel < 0.0f);
        bool upperActive = (angle >= maxAngle && wRel > 0.0f);

        if (!lowerActive && !upperActive)
            return;

        //제약식
        float lambda = SolveAngularLambda(wRel);

        // 🔥 누적 (Warm Start용)
        float oldImpulse = angularLimitImpulse;
        angularLimitImpulse += lambda;

        // 부호 클램프, 현재 상태에 맞는 Impulse만 사용
        if (lowerActive)
            angularLimitImpulse = MathUtility.Max(angularLimitImpulse, 0.0f);
        else if (upperActive)
            angularLimitImpulse = MathUtility.Min(angularLimitImpulse, 0.0f);

        lambda = angularLimitImpulse - oldImpulse;

        ApplyAngularImpulse(axis, lambda);
    }
    /// <summary>
    /// 힌지 축 방향 각속도가 목표
    /// </summary>
    /// <param name="dt"></param>
    void SolveAngularVelocityMotor(float dt)
    {
        if (!enableMotor) return;

        GetHingeBasis(out Vec3 axis, out Vec3 n1, out Vec3 n2);

        //상대 각속도를 축방향으로 투영
        float wRel = Vec3.Dot(rigidB.angularVelocity - rigidA.angularVelocity, axis);

        //제약식
        float Cdot = wRel - motorSpeed;
        float lambda = SolveAngularLambda(Cdot);

        // 누적 + clamp
        float oldImpulse = angularMotorImpulse;
        float maxImpulse = maxAngularMotorTorque * dt;

        angularMotorImpulse = MathUtility.ClampValue(angularMotorImpulse + lambda, -maxImpulse, maxImpulse);

        lambda = angularMotorImpulse - oldImpulse;

        ApplyAngularImpulse(axis, lambda);
    }
    #endregion

    #region Apply Function
    /// <summary>
    /// 주어진 축에대해 Constraint적용
    /// </summary>
    /// <param name="axis"></param>
    void ApplyLinearConstraint(Vec3 axis, Vec3 vRel, Vec3 rA, Vec3 rB)
    {
        float invMass = rigidA.invMass + rigidB.invMass;
        if (invMass == 0) return;

        //상대 속도를 축에 투영
        float lambda = -Vec3.Dot(vRel, axis) / invMass;
        Vec3 impulse = axis * lambda;

        rigidA.velocity -= impulse * rigidA.invMass;
        rigidB.velocity += impulse * rigidB.invMass;

        rigidA.angularVelocity -= Vec3.Cross(rA, impulse) * rigidA.invInertia;
        rigidB.angularVelocity += Vec3.Cross(rB, impulse) * rigidB.invInertia;
    }
    void SolveAngularFreeAxis(Vec3 axis, Vec3 wRel, ref float accumulatedImpulse)
    {
        float Cdot = Vec3.Dot(wRel, axis);
        float lambda = SolveAngularLambda(Cdot);

        accumulatedImpulse += lambda;
        ApplyAngularImpulse(axis, lambda);
    }

    void ApplyAngularVelocityConstraint(Vec3 axis, Vec3 wRel)
    {
        float Cdot = Vec3.Dot(wRel, axis);
        float k = rigidA.invInertia + rigidB.invInertia;
        if (k == 0.0f) return;

        float lambda = -Cdot / k;

        Vec3 impulse = axis * lambda;

        rigidA.angularVelocity -= impulse * rigidA.invInertia;
        rigidB.angularVelocity += impulse * rigidB.invInertia;
    }
    void ApplyAngularImpulse(Vec3 axis, float impulse)
    {
        Vec3 J = axis * impulse;
        rigidA.angularVelocity -= J * rigidA.invInertia;
        rigidB.angularVelocity += J * rigidB.invInertia;
    }
    float SolveAngularLambda(float Cdot)
    {
        float k = rigidA.invInertia + rigidB.invInertia;
        if (k == 0) return 0;
        return -Cdot / k;
    }
    #endregion

    #region Position
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
    void SolveAngularPositionHinge(float dt)
    {
        GetHingeBasis(out Vec3 axis, out _, out _);
        //제약식
        CustomQuaternion qError = QuaternionUtility.Inverse(rigidA.rotation) * rigidB.rotation;

        // 축에 수직한 회전 오차만 제거
        Vec3 error = qError.vec - axis * Vec3.Dot(qError.vec, axis);

        float slop = 1e-4f;
        if (error.Square < slop * slop)
            return;

        float k = rigidA.invInertia + rigidB.invInertia;
        if (k == 0) return;

        float beta = 0.2f;
        Vec3 lambda = -beta * error / k;

        //서로 다른 방향으로 회전
        rigidA.rotation = QuaternionUtility.IntegrateRotation(rigidA.rotation,lambda *(-1) * rigidA.invInertia,dt);
        rigidB.rotation =QuaternionUtility.IntegrateRotation(rigidB.rotation,lambda * rigidB.invInertia,dt);
    }
    void SolveAngularPositionLimit() { 
        /* 그대로 유지 */
    }
    #endregion

    #region Utility
    /// <summary>
    /// A 기준에서 B가 힌지 축을 중심으로 얼마나 회전했는지
    /// range : -pi ~ pi
    /// axis : hinge axis
    /// </summary>
    /// <returns></returns>
    float CurrentHingeAngle()
    {
        GetHingeBasis(out Vec3 axis, out _, out _);
        // 기준 벡터 (힌지 축에 수직한 로컬 축 하나)
        Vec3 refA_local = localPerpAxisA; // 생성자에서 저장
        Vec3 refB_local = localPerpAxisB;

        // 월드 변환
        Vec3 uA = Vec3.Rotation3DVec(rigidA.rotation, refA_local);
        Vec3 uB = Vec3.Rotation3DVec(rigidB.rotation, refB_local);

        // 축 성분 제거
        uA -= axis * Vec3.Dot(uA, axis);
        uB -= axis * Vec3.Dot(uB, axis);

        //정규화
        uA = uA.Normalized;
        uB = uB.Normalized;

        float sin = Vec3.Dot(axis, Vec3.Cross(uA, uB));
        float cos = Vec3.Dot(uA, uB);

        return MathUtility.Atan2(sin, cos); // -PI ~ PI
    }
    /// <summary>
    /// 힌지 축 / 직교축 계산
    /// </summary>
    /// <param name="axis"></param>
    /// <param name="n1"></param>
    /// <param name="n2"></param>
    void GetHingeBasis(out Vec3 axis, out Vec3 n1, out Vec3 n2)
    {
        axis = VectorMathUtils.GetWorldAxis(rigidA.rotation, localAxisA);

        if (Math.Abs(axis.x) < 0.9f)
            n1 = Vec3.Cross(axis, VectorMathUtils.RightVector3D()).Normalized;
        else
            n1 = Vec3.Cross(axis, VectorMathUtils.UpVector3D()).Normalized;

        n2 = Vec3.Cross(axis, n1);
    }
    #endregion
}
