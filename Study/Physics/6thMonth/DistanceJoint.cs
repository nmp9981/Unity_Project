class DistanceJoint : Joint
{
    float minDistance;
    float maxDistance;

    public override void BuildConstraintRows(float dt)
    {
        // 앵커
        Vec3 worldA = rigidA.LocalToWorld(localAnchorA);
        Vec3 worldB = rigidB.LocalToWorld(localAnchorB);

        rA = worldA - rigidA.position;
        rB = worldB - rigidB.position;

        Vec3 delta = worldB - worldA;
        float dist = delta.Magnitude;

        //거리 0
        if (dist < 1e-6f)
            return;

        //방향
        Vec3 n = delta / dist;

        // 거리 제한 (양방향)
        AddLinearLimitRow(n,dist,minDistance,maxDistance);

        // 회전은 완전 자유 (아무것도 추가하지 않음)
    }
}
