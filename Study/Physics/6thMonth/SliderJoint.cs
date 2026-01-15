class SliderJoint : Joint
{
    //슬라이더 축
    Vec3 localSliderAxisA;
    Vec3 localSliderAxisB;

    //위치 최대, 최소
    float minPos;
    float maxPos;

    public override void BuildConstraintRows(float dt)
    {
        //월드 앵커 계산
        Vec3 worldA = rigidA.LocalToWorld(localAnchorA);
        Vec3 worldB = rigidB.LocalToWorld(localAnchorB);

        rA = worldA - rigidA.position;
        rB = worldB - rigidB.position;

        //슬라이더에 수직인 두 축(월드 기준)
        Vec3 axisA = (rigidA.rotation * localSliderAxisA).Normalized;
        Vec3 axisB = (rigidB.rotation * localSliderAxisB).Normalized;
        //두 축 평균
        Vec3 sliderAxisWorld = (axisA + axisB).Normalized;

        Vec3 perpAxis1 = VectorMathUtils.AnyPerpendicular(sliderAxisWorld);
        Vec3 perpAxis2 = Vec3.Cross(sliderAxisWorld, perpAxis1).Normalized;

        //현재 슬라이더 위치는 두 앵커 차이를 슬라이더 축에 투영한 값
        Vec3 delta = worldB - worldA;
        float currentPos = Vec3.Dot(delta, sliderAxisWorld);

        // Linear
        AddLinearLimitRow(sliderAxisWorld, currentPos, minPos, maxPos);
        AddLinearLockRow(perpAxis1);
        AddLinearLockRow(perpAxis2);

        // Angular (축 정렬만 유지)
        AddAngularLockRow(perpAxis1);
        AddAngularLockRow(perpAxis2);
    }
}
