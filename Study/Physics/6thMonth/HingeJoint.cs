using System;
using UnityEngine.Animations;

class HingeJoint : Joint
{
    public override void BuildConstraintRows(float dt)
    {
        // 위치 고정 (3축)
        AddLinearLockRow(X);
        AddLinearLockRow(Y);
        AddLinearLockRow(Z);

        // 회전: 힌지 축만 자유
        AddAngularFreeRow(hingeAxis);

        // 나머지 회전 잠금
        AddAngularLockRow(perpAxis1);
        AddAngularLockRow(perpAxis2);

        // 필요하면 각도 제한
        AddAngularLimitRow(
            hingeAxis,
            currentAngle,
            minAngle,
            maxAngle);
    }
}
