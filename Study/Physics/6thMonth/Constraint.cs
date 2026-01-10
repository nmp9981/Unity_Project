using System.Collections.Generic;

/// <summary>
/// 자유도
/// </summary>
public enum ConstraintDOF
{
    LinearX, LinearY, LinearZ,
    AngularX, AngularY, AngularZ
}

/// <summary>
/// 모드
/// </summary>
public enum ConstraintMode
{
    Free,
    Lock,
    Limit,
    Motor
}
/// <summary>
/// 각도 공간의 방향
/// </summary>
public enum LimitType
{
    Lower,
    Upper
}

/// <summary>
/// Debug (관측)용 - solver에 전달 금지
/// </summary>
public struct ConstraintRowDebug
{
    public int jointId;            // 어느 Joint 소속인지
    public int rowIndex;           // Joint 내부 index

    public ConstraintDOF dof;
    public ConstraintMode mode;

    public Vec3 axisWorld;         // 월드 기준 축
    public float effectiveMass;    

    public float Cdot;             // 속도 오차
    public float bias;             // Baumgarte or bias term

    public float impulse;          // 이번 step에서 적용된 impulse
    public float accumulatedImpulse;

    public bool active;            // 이번 iteration에서 실제 solve 되었는가
}

/// <summary>
/// 최소 단위 Constraint, solver 전용
/// </summary>
public struct ConstraintRow
{
    // Solver가 접근할 Body index
    public int bodyA;
    public int bodyB;

    //jacobian
    public Vec3 JLinearA, JAngularA;
    public Vec3 JLinearB, JAngularB;

    //Solver 파라미터
    public float effectiveMass;
    public float bias;

    // Impulse
    public float accumulatedImpulse;
    public float minImpulse;
    public float maxImpulse;

    // Debug / semantic (임시 유지)
    public ConstraintMode mode;
    public ConstraintDOF dof;
}
