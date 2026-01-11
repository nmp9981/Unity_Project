using System.Collections.Generic;
// Joint
// - ConstraintRow를 생성한다
// - Solver 로직 없음
// - RigidBody 상태 직접 변경 금지
public abstract class Joint
{
    #region 공통
    // Joint 내부에서만 사용 (Jacobian 계산용)
    protected CustomRigidBody rigidA;
    protected CustomRigidBody rigidB;

    //각 물체의 로컬 축
    protected Vec3 localAnchorA;
    protected Vec3 localAnchorB;

    protected List<ConstraintRow> constraintRows;
    #endregion

    //BuildConstraintRows()에서 계산
    protected Vec3 rA;   // worldAnchor - bodyA.position
    protected Vec3 rB;

    #region Joint 설정값
    // Linear limit
    protected bool enableLinearLimit;
    protected float linearMin;
    protected float linearMax;

    // Angular limit
    protected bool enableAngularLimit;
    protected float angularMin;
    protected float angularMax;

    // Motor
    protected bool enableMotor;
    protected float motorSpeed;
    protected float maxMotorForce;
    #endregion

    /// <summary>
    /// Jacobian / effectiveMass / bias 정의
    /// </summary>
    public abstract void BuildConstraintRows(float dt);

    public virtual void OnWake()
    {

    }
    /// <summary>
    /// 선속 축 잠금
    /// </summary>
    /// <param name="axis"></param>
    protected void AddLinearLockRow(Vec3 axis)
    {
        ConstraintRow row = new ConstraintRow();

        // axis는 반드시 정규화
        Vec3 n = axis.Normalized;

        //Rigidbody
        row.bodyA = rigidA.id;
        row.bodyB = rigidB.id;

        // Jacobian
        row.JLinearA = (-1f) * n;
        row.JAngularA = (-1f) * Vec3.Cross(rA, n);

        row.JLinearB = n;
        row.JAngularB = Vec3.Cross(rB, n);

        // Bias (position correction), 상대앵커 위치를 축으로 투영
        float positionalError = Vec3.Dot((rB + rigidB.position) - (rA + rigidA.position), n);
        const float baumgarte = 0.2f;
        row.bias = -(baumgarte / SolverSettings.timeStep) * positionalError;

        row.accumulatedImpulse = 0f;
        row.minImpulse = float.NegativeInfinity;
        row.maxImpulse = float.PositiveInfinity;

        row.mode = ConstraintMode.Lock;
        constraintRows.Add(row);
    }
    /// <summary>
    /// 회전축 잠금
    /// </summary>
    /// <param name="axisWorld"></param>
    protected void AddAngularLockRow(Vec3 axisWorld)
    {
        ConstraintRow row = new ConstraintRow();

        row.bodyA = rigidA.id;
        row.bodyB = rigidB.id;

        // Angular Jacobian
        row.JLinearA = VectorMathUtils.ZeroVector3D();
        row.JLinearB = VectorMathUtils.ZeroVector3D();
        row.JAngularA = (-1f)*axisWorld;
        row.JAngularB = axisWorld;

        // effective mass
        Vec3 n = axisWorld.Normalized;

        row.JAngularA = (-1f)*n;
        row.JAngularB = n;

        float k =
            rigidA.invInertia * Vec3.Dot(n, n) +
            rigidB.invInertia * Vec3.Dot(n, n);

        row.effectiveMass = (k > 0.0f) ? 1.0f / k : 0.0f;

        // bias (velocity-level constraint)
        //속도 제약 (ω = 0)
        //위치 오차는 SolvePosition에서 따로 처리
        row.bias = 0.0f;

        // impulse
        row.accumulatedImpulse = 0.0f;
        row.minImpulse = float.NegativeInfinity;
        row.maxImpulse = float.PositiveInfinity;

        // metadata
        row.mode = ConstraintMode.Lock;
        row.dof = ConstraintDOF.AngularX; // ⚠️ 실제로는 axis에 따라 X/Y/Z 매핑

        constraintRows.Add(row);
    }
    /// <summary>
    /// 모든 축 자유
    /// </summary>
    /// <param name="axisWorld"></param>
    protected void AddAngularFreeRow(Vec3 axisWorld)
    {
        // Angular free DOF:
        // 아무 제약도 추가하지 않는다.
    }
    /// <summary>
    /// 모든 축 자유
    /// </summary>
    /// <param name="axisWorld"></param>
    protected void AddLinearFreeRow(Vec3 axisWorld)
    {
        // Angular free DOF:
        // 아무 제약도 추가하지 않는다.
    }
    /// <summary>
    /// 회전 제한
    /// </summary>
    /// <param name="axisWorld"></param>
    /// <param name="angle"></param>
    /// <param name="minAngle"></param>
    /// <param name="maxAngle"></param>
    protected void AddAngularLimitRow(
    Vec3 axisWorld,
    float angle,
    float minAngle,
    float maxAngle)
    {
        // 현재 각도가 허용 범위면 제약 없음
        if (angle >= minAngle && angle <= maxAngle)
            return;

        ConstraintRow row = new ConstraintRow();
        Vec3 n = axisWorld.Normalized;

        //Rigidbody
        row.bodyA = rigidA.id;
        row.bodyB = rigidB.id;

        // Angular Jacobian
        row.JAngularA = (-1f)*axisWorld;
        row.JAngularB = axisWorld;

        row.JLinearA = VectorMathUtils.ZeroVector3D();
        row.JLinearB = VectorMathUtils.ZeroVector3D();

        row.JAngularA = (-1f)*n;
        row.JAngularB = n;

        float k =
            rigidA.invInertia * Vec3.Dot(n, n) +
            rigidB.invInertia * Vec3.Dot(n, n);

        row.effectiveMass = (k > 0.0f) ? 1.0f / k : 0.0f;

        const float baumgarte = 0.2f;

        if (angle < minAngle)
        {
            float error = minAngle - angle;
            row.bias = (baumgarte / SolverSettings.timeStep) * error;
            row.minImpulse = 0.0f;
            row.maxImpulse = float.PositiveInfinity;
        }
        else
        {
            float error = angle - maxAngle;
            row.bias = -(baumgarte / SolverSettings.timeStep) * error;
            row.minImpulse = float.NegativeInfinity;
            row.maxImpulse = 0.0f;
        }

        row.accumulatedImpulse = 0.0f;

        // 메타데이터는 남겨도 됨 (디버그용)
        row.isLimit = true;

        constraintRows.Add(row);
    }
}
