class FixedJoint : Joint
{
    public override void BuildConstraintRows(float dt)
    {
        // 위치 완전 고정
        AddLinearLockRow(X);
        AddLinearLockRow(Y);
        AddLinearLockRow(Z);

        // 회전 완전 고정
        AddAngularLockRow(X);
        AddAngularLockRow(Y);
        AddAngularLockRow(Z);
    }
}
