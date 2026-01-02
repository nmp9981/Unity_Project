using System;

class SliderJoint : Joint
{
    //각 쿨체의 로컬 축
    Vec3 localAnchorA;
    Vec3 localAnchorB;

    float effectiveMass;
    float angularEffectiveMass;

    // joint가 허용하는 회전축 (로컬)
    Vec3 localAxisA;
    Vec3 localAxisB;

    //축 방향 제어
    bool enableLimit;
    float minLimit;
    float maxLimit;

    //속도 제어
    bool enableMotor;
    //선속도
    float motorLinearSpeed;
    float maxMotorForce;
    float motorImpulse;
    //각속도
    float motorAngularSpeed;
    float angularMotorImpulse;
    float maxAngularMotorTorque;

    //Impulse 제한
    float lowerLimitImpulse;
    float upperLimitImpulse;

    //생성자
    public SliderJoint(CustomRigidBody a, CustomRigidBody b, Vec3 worldAnchor, Vec3 worldAxis)
    {
        rigidA = a;
        rigidB = b;

        localAnchorA = rigidA.WorldToLocal(worldAnchor);
        localAnchorB = rigidB.WorldToLocal(worldAnchor);

        localAxisA = rigidA.WorldToLocalDirection(worldAxis).Normalized;
        localAxisB = rigidB.WorldToLocalDirection(worldAxis).Normalized;
    }

    //Solver 구현 : 추상 클래스 상속 후 구현
    //속도는 미리 제어, 위치는 과거 오차 바로 잡기
    public override void SolveVelocity(float dt)
    {
        SolveLinearVelocityPerp();   // ⟂ 축
        SolveLinearVelocityLimit();  // ∥ 축 (선택)
        SolveLinearVelocityMotor(dt);  // 원하는 선속도 부여
       
        SolveAngularVelocity();//회전 정렬
        SolveAngularVelocityMotor(dt);//회전 모터
    }

    public override void SolvePosition(float dt)
    {
        SolveLinearPositionPerp();   // ⟂ 축
        SolveLinearPositionLimit();  // ∥ 축 (선택)
        SolveAngularPosition(dt);
    }
    /// <summary>
    /// 축 수직 이동 제한
    /// </summary>
    void SolveLinearVelocityPerp()
    {
        //축 위치 계산
        Vec3 rA = Vec3.Rotation3DVec(rigidA.rotation, localAnchorA);
        Vec3 rB = Vec3.Rotation3DVec(rigidB.rotation, localAnchorB);

        Vec3 pA = rigidA.position + rA;
        Vec3 pB = rigidB.position + rB;

        //상대 속도 계산
        Vec3 vRel = GetRelativeVelocity(rA,rB);

        //여기서 두 축을 구한다.
        Vec3 axisA = Vec3.Rotation3DVec(rigidA.rotation, localAxisA).Normalized;
        Vec3 axisB = Vec3.Rotation3DVec(rigidB.rotation, localAxisB).Normalized;

        //n1,n2를 구한다.
        Vec3 n1;
        if (Math.Abs(axisA.x) < 0.9f)
            n1 = Vec3.Cross(axisA, VectorMathUtils.RightVector3D()).Normalized;
        else
            n1 = Vec3.Cross(axisA, VectorMathUtils.UpVector3D()).Normalized;

        Vec3 n2 = Vec3.Cross(axisA, n1);

        Vec3 axisError = Vec3.Cross(axisA, axisB);

        // slider는 axisA 방향 회전만 허용
        // axisA에 수직한 두 축(n1,n2)에 대해서만 각속도 제약
        ApplyLinearConstraint(n1, vRel, rA, rB);
        ApplyLinearConstraint(n2, vRel, rA, rB);
    }

    /// <summary>
    /// 각속도 Solver
    /// </summary>
    void SolveAngularVelocity()
    {
        //여기서 두 축을 구한다.
        Vec3 axisA = Vec3.Rotation3DVec(rigidA.rotation, localAxisA).Normalized;
        Vec3 axisB = Vec3.Rotation3DVec(rigidB.rotation, localAxisB).Normalized;

        //n1,n2를 구한다.
        Vec3 n1;
        if (Math.Abs(axisA.x) < 0.9f)
            n1 = Vec3.Cross(axisA, VectorMathUtils.RightVector3D()).Normalized;
        else
            n1 = Vec3.Cross(axisA, VectorMathUtils.UpVector3D()).Normalized;

        Vec3 n2 = Vec3.Cross(axisA, n1);

        Vec3 axisError = Vec3.Cross(axisA, axisB);

        ApplyAngularConstraint(n1);
        ApplyAngularConstraint(n2);
    }

    /// <summary>
    /// 주어진 축에대해 Constraint적용
    /// </summary>
    /// <param name="axis"></param>
    void ApplyLinearConstraint(Vec3 axis, Vec3 vRel, Vec3 rA, Vec3 rB)
    {
        float k = ComputeLinearK(axis, rA, rB);
        effectiveMass = 1.0f /k;

        //상대속도를 축에 투영
        float Cdot = Vec3.Dot(vRel, axis);

        float lambda = -Cdot * effectiveMass;

        Vec3 impulse = axis * lambda;

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

        Vec3 impulse = axis * lambda;

        rigidA.angularVelocity -= impulse * rigidA.invInertia;
        rigidB.angularVelocity += impulse * rigidB.invInertia;
    }

    /// <summary>
    /// 틀어진 위치를 되돌림, 축 수직 위치 제한
    /// 두 anchor는 항상 같은 위치
    /// </summary>
    void SolveLinearPositionPerp()
    {
        // 여기서 다시 계산
        Vec3 axis = GetWorldAxis();

        // 월드 anchor
        Vec3 rA = Vec3.Rotation3DVec(rigidA.rotation, localAnchorA);
        Vec3 rB = Vec3.Rotation3DVec(rigidB.rotation, localAnchorB);

        Vec3 pA = rigidA.position + rA;
        Vec3 pB = rigidB.position + rB;

        // position error, 제약식, drift
        Vec3 C = (pB - pA) - axis * Vec3.Dot(pB - pA, axis);//축에 수직한 성분만 0이어야함, 축방향으로 투영된 부분은 제약식에서 제거

        // 허용 오차 (slop), 두 물체의 anchor위치가 같은지 검사
        float slop = 0.001f;
        if (C.Square < slop * slop)//제약식 만족이라 위치 조절 필요없음
            return;

        // Baumgarte 계수
        float beta = 0.2f;

        // effective mass (선형만)
        float k =ComputeLinearK(axis, rA, rB);
        if (k == 0.0f)//둘다 정적인 물체
            return;

        Vec3 correction = -(beta / k) * C;

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
        // 여기서 다시 계산
        Vec3 axis = GetWorldAxis();

        CustomQuaternion qA = rigidA.rotation;
        CustomQuaternion qB = rigidB.rotation;

        //qerror​=qB​⋅qA^(-1) : 상대 회전
        // 현재 상대 회전
        CustomQuaternion qError = QuaternionUtility.Inverse(qA) * qB;

        Vec3 axisError = qError.vec;
        // 회전 오차를 축에 수직한 성분만 남긴다
        Vec3 error = axisError
                   - axis * Vec3.Dot(axisError, axis);

        float slop = 0.001f;
        if (error.Square < slop * slop)
            return;

        float beta = 0.2f;
        Vec3 correction = -beta * error;

        rigidA.rotation = QuaternionUtility.IntegrateRotation(rigidA.rotation, correction * rigidA.invInertia * (-1), dt);
        rigidB.rotation = QuaternionUtility.IntegrateRotation(rigidB.rotation, correction * rigidB.invInertia, dt);
    }
    /// <summary>
    /// 축 방향 Limit제한
    /// </summary>
    void SolveLinearVelocityLimit()
    {
        //제한이 없음
        if (!enableLimit) return;

        // 월드 anchor
        Vec3 rA = Vec3.Rotation3DVec(rigidA.rotation, localAnchorA);
        Vec3 rB = Vec3.Rotation3DVec(rigidB.rotation, localAnchorB);

        Vec3 axis = GetWorldAxis();
        float d = CurrentDistanceAlongAxis(rA,rB);

        //Limit
        float C = 0.0f;
        if (d < minLimit) C = d - minLimit;
        else if (d > maxLimit) C = d - maxLimit;
        else return;

        //상대속도를 축에 투영
        Vec3 vRel = GetRelativeVelocity(rA,rB);
        float Cdot = Vec3.Dot(vRel, axis);

        //이미 복귀중이면 패스
        if (C < 0 && Cdot > 0) return;
        if(C>0 && Cdot < 0) return;

        float k = ComputeLinearK(axis,rA,rB);
        if(k==0)   return;

        //Impulse(충격량) 계산
        float lambda = -Cdot / k;
        Vec3 impulse = axis * lambda;

        ApplyLinearImpulse(impulse, rA, rB);
    }

    /// <summary>
    /// 축 방향 Limit 제한
    /// </summary>
    void SolveLinearPositionLimit()
    {
        //제한이 없을 경우
        if (!enableLimit) return;

        Vec3 axis = GetWorldAxis();

        Vec3 rA = Vec3.Rotation3DVec(rigidA.rotation,localAnchorA);
        Vec3 rB = Vec3.Rotation3DVec(rigidB.rotation, localAnchorB);

        Vec3 pA = rigidA.position + rA;
        Vec3 pB = rigidB.position + rB;

        float d = Vec3.Dot(pB - pA, axis);

        float C = 0.0f;
        if (d < minLimit) C = d - minLimit;
        else if (d > maxLimit) C = d - maxLimit;
        else return;

        float k = ComputeLinearK(axis, rA, rB);
        if (k == 0) return;

        float beta = 0.2f;
        float lambda = -beta * C / k;

        Vec3 impulse = axis * lambda;

        ApplyLinearImpulse(impulse, rA, rB);
    }

    /// <summary>
    /// 축방향 원하는 속도 부여 - 순수 속도 제약
    /// </summary>
    void SolveLinearVelocityMotor(float dt)
    {
        if (!enableMotor) return;

        Vec3 rA = Vec3.Rotation3DVec(rigidA.rotation, localAnchorA);
        Vec3 rB = Vec3.Rotation3DVec(rigidB.rotation, localAnchorB);
        Vec3 axis = GetWorldAxis();

        //상대 속도
        Vec3 vRel = GetRelativeVelocity(rA, rB);
        //motor 제약식
        float Cdot = Vec3.Dot(vRel, axis) - motorLinearSpeed;

        float k = ComputeLinearK(axis, rA, rB);
        if (k == 0) return;

        float lambda = -Cdot / k;

        // 모터 힘 제한 (중요!)
        float oldImpulse = motorImpulse;
        motorImpulse = MathUtility.ClampValue(
            motorImpulse + lambda,
            -maxMotorForce * dt,
             maxMotorForce * dt
        );
        lambda = motorImpulse - oldImpulse;

        Vec3 impulse = axis * lambda;
        ApplyLinearImpulse(impulse, rA, rB);
    }
    /// <summary>
    /// 축방향 원하는 속도 부여 - 순수 속도 제약
    /// </summary>
    void SolveAngularVelocityMotor(float dt)
    {
        if (!enableMotor) return;

        Vec3 rA = Vec3.Rotation3DVec(rigidA.rotation, localAnchorA);
        Vec3 rB = Vec3.Rotation3DVec(rigidB.rotation, localAnchorB);
        //월드 축
        Vec3 axis = GetWorldAxis();

        //상대 각속도
        Vec3 vRel = rigidB.angularVelocity - rigidA.angularVelocity;
        //motor 제약식
        float Cdot = Vec3.Dot(vRel, axis) - motorLinearSpeed;

        float k = ComputeLinearK(axis, rA, rB);
        if (k == 0) return;

        float lambda = -Cdot / k;

        //각 모터 힘 제한 (중요!)
        float oldImpulse = angularMotorImpulse;
        angularMotorImpulse = MathUtility.ClampValue(
            angularMotorImpulse + lambda,
            -maxAngularMotorTorque * dt,
             maxAngularMotorTorque * dt
        );
        lambda = angularMotorImpulse - oldImpulse;

        Vec3 impulse = axis * lambda;
        rigidA.angularVelocity -= impulse * rigidA.invInertia;
        rigidB.angularVelocity += impulse * rigidB.invInertia;
    }

    #region 공통 함수 - 추후 타 클래스로 옮길예정
    /// <summary>
    /// K값 계산
    /// </summary>
    /// <param name="axis"></param>
    /// <param name="rA"></param>
    /// <param name="rB"></param>
    /// <returns></returns>
    float ComputeLinearK(Vec3 axis, Vec3 rA, Vec3 rB)
    {
        float k = rigidA.invMass + rigidB.invMass
  + Vec3.Dot(Vec3.Cross(rA, axis), Vec3.Cross(rA, axis)) * rigidA.invInertia
  + Vec3.Dot(Vec3.Cross(rB, axis), Vec3.Cross(rB, axis)) * rigidB.invInertia;
        return k;
    }
    /// <summary>
    /// 월드 축 계산
    /// </summary>
    /// <returns></returns>
    Vec3 GetWorldAxis()
    {
        return Vec3.Rotation3DVec(rigidA.rotation, localAxisA).Normalized;
    }
    /// <summary>
    /// 상대 속도 계산
    /// </summary>
    /// <returns></returns>
    Vec3 GetRelativeVelocity(Vec3 rA,Vec3 rB)
    {
        //상대 속도 계산
        Vec3 vRel = rigidB.velocity + Vec3.Cross(rigidB.angularVelocity, rB) - rigidA.velocity - Vec3.Cross(rigidA.angularVelocity, rA);
        return vRel;
    }

    /// <summary>
    /// 슬라이더 거리
    /// </summary>
    /// <returns></returns>
    float CurrentDistanceAlongAxis(Vec3 rA,Vec3 rB)
    {
        Vec3 axis = GetWorldAxis();
        Vec3 pA = rigidA.position + rA;
        Vec3 pB = rigidB.position + rB;

        //상대 위치, 월드 축 내적값
        return Vec3.Dot(pB - pA, axis);
    }
    /// <summary>
    /// Impulse 적용
    /// </summary>
    /// <param name="impulse"></param>
    /// <param name="rA"></param>
    /// <param name="rB"></param>
    void ApplyLinearImpulse(Vec3 impulse, Vec3 rA, Vec3 rB)
    {
        rigidA.velocity -= impulse * rigidA.invMass;
        rigidB.velocity += impulse * rigidB.invMass;

        rigidA.angularVelocity -= Vec3.Cross(rA, impulse) * rigidA.invInertia;
        rigidB.angularVelocity += Vec3.Cross(rB, impulse) * rigidB.invInertia;
    }
    #endregion
}
