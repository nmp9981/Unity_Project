class DistanceJoint : Joint
{
    public override void BuildConstraintRows(float dt)
    {
        // 선형: 전부 Lock
        AddLinearLockRow(X);
        AddLinearLockRow(Y);
        AddLinearLockRow(Z);

        // 회전: 전부 Free
        AddAngularFreeRow(X);
        AddAngularFreeRow(Y);
        AddAngularFreeRow(Z);
    }
}
