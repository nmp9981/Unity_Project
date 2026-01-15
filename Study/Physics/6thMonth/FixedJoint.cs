class FixedJoint : Joint
{
    public override void BuildConstraintRows(float dt)
    {
        // 앵커
        Vec3 worldA = rigidA.LocalToWorld(localAnchorA);
        Vec3 worldB = rigidB.LocalToWorld(localAnchorB);

        rA = worldA - rigidA.position;
        rB = worldB - rigidB.position;

        // A 기준 축
        Vec3 x = rigidA.rotation * VectorMathUtils.RightVector3D();
        Vec3 y = rigidA.rotation * VectorMathUtils.UpVector3D();
        Vec3 z = rigidA.rotation * VectorMathUtils.FrontVector3D();

        // 위치 고정
        AddLinearLockRow(x);
        AddLinearLockRow(y);
        AddLinearLockRow(z);

        // 회전 고정
        AddAngularLockRow(x);
        AddAngularLockRow(y);
        AddAngularLockRow(z);
    }
}
