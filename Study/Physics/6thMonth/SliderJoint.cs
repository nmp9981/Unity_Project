class SliderJoint : Joint
{
    public override void BuildConstraintRows(float dt)
    {
        // 주축
        AddLinearLimitRow(axis, currentPos, minPos, maxPos);

        // 나머지 축
        AddLinearLockRow(perp1);
        AddLinearLockRow(perp2);

        AddAngularLockRow(X);
        AddAngularLockRow(Y);
        AddAngularLockRow(Z);
    }
}
