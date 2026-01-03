public static class JointCommon
{
    /// <summary>
    /// K값 계산
    /// </summary>
    /// <param name="axis"></param>
    /// <param name="rA"></param>
    /// <param name="rB"></param>
    /// <returns></returns>
    public static float ComputeLinearK(Vec3 axis, Vec3 rA, Vec3 rB, CustomRigidBody rigidA, CustomRigidBody rigidB)
    {
        float k = rigidA.invMass + rigidB.invMass
  + Vec3.Dot(Vec3.Cross(rA, axis), Vec3.Cross(rA, axis)) * rigidA.invInertia
  + Vec3.Dot(Vec3.Cross(rB, axis), Vec3.Cross(rB, axis)) * rigidB.invInertia;
        return k;
    }

    /// <summary>
    /// 상대 속도 계산
    /// </summary>
    /// <returns></returns>
    public static Vec3 GetRelativeVelocity(Vec3 rA, Vec3 rB, CustomRigidBody rigidA, CustomRigidBody rigidB)
    {
        //상대 속도 계산
        Vec3 vRel = rigidB.velocity + Vec3.Cross(rigidB.angularVelocity, rB) - rigidA.velocity - Vec3.Cross(rigidA.angularVelocity, rA);
        return vRel;
    }

    /// <summary>
    /// 슬라이더 거리
    /// </summary>
    /// <returns></returns>
    public static float CurrentDistanceAlongAxis(Vec3 rA, Vec3 rB, CustomRigidBody rigidA, CustomRigidBody rigidB, Vec3 localAxisA)
    {
        Vec3 axis = VectorMathUtils.GetWorldAxis(rigidA.rotation, localAxisA);
        Vec3 pA = rigidA.position + rA;
        Vec3 pB = rigidB.position + rB;

        //상대 위치, 월드 축 내적값
        return Vec3.Dot(pB - pA, axis);
    }
    /// <summary>
    /// Impulse 적용
    /// </summary>
    /// <param name="impulse"></param>
    /// <param name="rA"></param>
    /// <param name="rB"></param>
    public static void ApplyLinearImpulse(Vec3 impulse, Vec3 rA, Vec3 rB, CustomRigidBody rigidA, CustomRigidBody rigidB)
    {
        rigidA.velocity -= impulse * rigidA.invMass;
        rigidB.velocity += impulse * rigidB.invMass;

        rigidA.angularVelocity -= Vec3.Cross(rA, impulse) * rigidA.invInertia;
        rigidB.angularVelocity += Vec3.Cross(rB, impulse) * rigidB.invInertia;
    }
}
