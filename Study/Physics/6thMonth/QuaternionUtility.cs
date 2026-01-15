/// <summary>
/// 쿼터니언 정의
/// </summary>
[System.Serializable]
public struct CustomQuaternion
{
    //성분(스칼라 + 벡터)
    public float scala;
    public Vec3 vec;

    //생성자
    public CustomQuaternion(float scala, Vec3 vec)
    {
        this.scala = scala;
        this.vec = vec;
    }

    //연산자
    public static CustomQuaternion operator +(CustomQuaternion a, CustomQuaternion b)
        => new CustomQuaternion(a.scala+b.scala, a.vec + b.vec);
    public static CustomQuaternion operator -(CustomQuaternion a, CustomQuaternion b)
        => new CustomQuaternion(a.scala - b.scala, a.vec - b.vec);
    public static CustomQuaternion operator *(CustomQuaternion a, CustomQuaternion b)
        => new CustomQuaternion(
            a.scala*b.scala-Vec3.Dot(a.vec,b.vec), 
            a.scala*b.vec+b.scala*a.vec+Vec3.Cross(a.vec,b.vec)
            );
    public static CustomQuaternion operator *(CustomQuaternion q, float d)
        => new CustomQuaternion(q.scala * d, q.vec * d);
      public static Vec3 operator *(CustomQuaternion q, Vec3 v)
  {
      // 벡터를 quaternion으로 변환
      CustomQuaternion qv = new CustomQuaternion(0.0f, v);
      // 회전: q * v * q^-1
      CustomQuaternion result = q * qv * q.Conjugate;
      return result.vec;
  }
    public static CustomQuaternion operator /(CustomQuaternion q, float d)
        => new CustomQuaternion(q.scala/d,q.vec/d);

    //크기
    public float Square => scala*scala+Vec3.Dot(vec,vec);
    public float Magnitude => MathUtility.Root(scala * scala + Vec3.Dot(vec, vec));
    public CustomQuaternion Normalized => new CustomQuaternion(scala/Magnitude, vec/Magnitude);

    //켤레
    public CustomQuaternion Conjugate => new CustomQuaternion(scala, new Vec3(-vec.x, -vec.y, -vec.z));
}


public static class QuaternionUtility
{
    //역수
    public static CustomQuaternion Inverse(CustomQuaternion q)
    {
        return q.Conjugate / q.Square;
    }

    /// <summary>
    /// 회전 적분
    /// </summary>
    /// <param name="rot"></param>
    /// <param name="corr"></param>
    /// <param name="dt"></param>
    /// <returns></returns>
    public static CustomQuaternion IntegrateRotation(CustomQuaternion rot, Vec3 corr, float dt)
    {
        CustomQuaternion omegaQ = new CustomQuaternion(0.0f, corr);
        CustomQuaternion dq = rot * omegaQ;
        CustomQuaternion nextRot = rot + dq * (0.5f * dt);
        return nextRot.Normalized;
    }
    /// <summary>
/// q 회전 중에서 주어진 axis 방향으로 “얼마나 회전했는가” (signed angle)
/// </summary>
/// <param name="q"></param>
/// <param name="axis"></param>
/// <returns></returns>
public static float TwistAngle(CustomQuaternion q, Vec3 axis)
{
    // 반드시 정규화
    Vec3 n = axis.Normalized;

    // quaternion vector part
    Vec3 v = q.vec;

    // twist 성분 추출 (projection)
    Vec3 vTwist = n * Vec3.Dot(v, n);

    // twist quaternion
    CustomQuaternion qTwist = new CustomQuaternion(
        q.scala,
        vTwist
    );

    qTwist = qTwist.Normalized;

    // angle = 2 * acos(w)
    float angle = 2.0f * MathUtility.Acos(MathUtility.ClampValue(qTwist.scala, -1f, 1f));

    // 부호 결정 (축 방향)
    float sign = Vec3.Dot(vTwist, n) >= 0.0f ? 1.0f : -1.0f;

    return angle * sign;
}
}
