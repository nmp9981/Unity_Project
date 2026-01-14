class HingeJoint : Joint
{
    Vec3 localHingeAxisA;   // ✔️ hinge 전용
    Vec3 localHingeAxisB;   // ✔️ (있으면 더 정확)

    float angularMin;
    float angularMax;

    public override void BuildConstraintRows(float dt)
    {
        //월드 앵커 계산
        Vec3 worldA = rigidA.LocalToWorld(localAnchorA);
        Vec3 worldB = rigidB.LocalToWorld(localAnchorB);

        rA = worldA - rigidA.position;
        rB = worldB - rigidB.position;

        //힌지 축 (월드 기준, 정규화)
        Vec3 hingeAxisWorld = (rigidA.rotation * localHingeAxisA).Normalized;

        //힌지에 수직인 두 축(월드 기준)
        Vec3 perpAxis1 = hingeAxisWorld.AnyPerpendicular().Normalized;
        Vec3 perpAxis2 = Vec3.Cross(hingeAxisWorld, perpAxis1).Normalized;

        // 위치 고정 (3축)
        AddLinearLockRow(perpAxis1);
        AddLinearLockRow(perpAxis2);
        AddLinearLockRow(hingeAxisWorld);

        // 회전: 힌지 축만 자유
        AddAngularFreeRow(hingeAxisWorld);

        // 나머지 회전 잠금
        AddAngularLockRow(perpAxis1);
        AddAngularLockRow(perpAxis2);

        // 필요하면 각도 제한
        if (enableAngularLimit)
        {
            //현재 힌지 각도
            float currentAngle = ComputeTwistAngle(rigidA.rotation, rigidB.rotation, hingeAxisWorld);

            AddAngularLimitRow(hingeAxisWorld,currentAngle,angularMin,angularMax);
        }
    }
    /// <summary>
    /// 현재 힌지 회전각 구하기
    /// </summary>
    /// <param name="qA"></param>
    /// <param name="qB"></param>
    /// <param name="hingeAxis"></param>
    float ComputeTwistAngle(CustomQuaternion qA, CustomQuaternion qB, Vec3 hingeAxis)
    {
        // 상대 회전
        CustomQuaternion qRel = qB * QuaternionUtility.Inverse(qA);

        // 힌지 축 기준 회전량 추출
        float currentAngle = QuaternionUtility.TwistAngle(qRel, hingeAxis);
        return currentAngle;
    }
}
