using UnityEngine;

class SixDOFJoint : Joint
{
    DOFConstraint linear[3];
    DOFConstraint angular[3];
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
