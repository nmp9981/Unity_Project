using UnityEngine;

/// <summary>
/// 자유도
/// </summary>
enum ConstraintDOF
{
    LinearX, LinearY, LinearZ,
    AngularX, AngularY, AngularZ
}

/// <summary>
/// 모드
/// </summary>
enum ConstraintMode
{
    Free,
    Lock,
    Limit,
    Motor
}

struct ConstraintRowDenug
{
    int jointId;            // 어느 Joint 소속인지
    int rowIndex;           // Joint 내부 index

    ConstraintDOF dof;
    ConstraintMode mode;

    Vec3 axisWorld;         // 월드 기준 축
    float effectiveMass;    

    float Cdot;             // 속도 오차
    float bias;             // Baumgarte or bias term

    float impulse;          // 이번 step에서 적용된 impulse
    float accumulatedImpulse;

    bool active;            // 이번 iteration에서 실제 solve 되었는가
}

/// <summary>
/// 최소 단위 Constraint
/// </summary>
public struct ConstraintRow
{
    Vec3 JLinearA;
    Vec3 JAngularA;
    Vec3 JLinearB;
    Vec3 JAngularB;

    float effectiveMass;
    float bias;

    float accumulatedImpulse;
}


public class Constraint
{
    //Joint = ConstraintRow[];
}
