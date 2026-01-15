/// <summary>
/// 벡터 정의 2D, 3D
/// </summary>
[System.Serializable]
public struct Vec2
{
    //성분
    public float x;
    public float y;

    //생성자
    public Vec2(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    //기본 연산자 오버로딩
    public static Vec2 operator +(Vec2 a, Vec2 b)
        => new Vec2(a.x + b.x, a.y + b.y);
    public static Vec2 operator -(Vec2 a, Vec2 b)
       => new Vec2(a.x - b.x, a.y - b.y);
    public static Vec2 operator *(Vec2 a, float d)
       => new Vec2(a.x * d, a.y * d);
    public static Vec2 operator /(Vec2 a, float d)
       => new Vec2(a.x / d, a.y / d);

    //길이, 방향
    public float Square => x * x + y * y;
    public float Magnitude => MathUtility.Root(x * x + y * y);
    public Vec2 Normalized => this / (Magnitude == 0 ? 0.0001f : Magnitude);//0으로 나눌 수 없음

    //내적, 행렬식
    public static float Dot(Vec2 a, Vec2 b)
        => a.x * b.x + a.y * b.y;
    public static float Det(Vec2 a, Vec2 b)
        => a.x * b.y - a.y * b.x;
}

[System.Serializable]
public struct Vec3
{
    //성분
    public float x;
    public float y;
    public float z;

    //생성자
    public Vec3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    //기본 연산자 오버로딩
    public static Vec3 operator +(Vec3 a, Vec3 b)
        => new Vec3(a.x + b.x, a.y + b.y, a.z+b.z);
    public static Vec3 operator -(Vec3 a, Vec3 b)
       => new Vec3(a.x - b.x, a.y - b.y, a.z - b.z);
    public static Vec3 operator *(Vec3 a, float d)
       => new Vec3(a.x*d, a.y*d, a.z*d);
    public static Vec3 operator *(float d, Vec3 a)
   => new Vec3(d*a.x, d*a.y, d*a.z);
    public static Vec3 operator /(Vec3 a, float d)
       => new Vec3(a.x/d, a.y/d, a.z/d);

    //길이, 방향
    public float Square => x * x + y * y + z * z;
    public float Magnitude => MathUtility.Root(x * x + y * y + z * z);
    public Vec3 Normalized => this / (Magnitude == 0 ? 0.0001f : Magnitude);//0으로 나눌 수 없음

    //회전
    //QVQ^(-1)
    public static Vec3 Rotation3DVec(CustomQuaternion q, Vec3 v)
    {
        Vec3 u = new Vec3(q.vec.x, q.vec.y, q.vec.z);
        float s = q.scala;

        return
            2.0f * Vec3.Dot(u, v) * u
          + (s * s - Vec3.Dot(u, u)) * v
          + 2.0f * s * Vec3.Cross(u, v);
    }

    //내적, 외적
    public static float Dot(Vec3 a, Vec3 b)
        => a.x * b.x + a.y * b.y + a.z * b.z;
    public static Vec3 Cross(Vec3 a, Vec3 b)
        => new Vec3(a.y*b.z-a.z*b.y
            ,a.z*b.x-a.x*b.z
            ,a.x*b.y-b.x*a.y);

    //보간
    public static Vec3 Lerp(Vec3 a, Vec3 b, float alpha)
        => a+(b-a)*alpha;
}


public static class VectorMathUtils
{
    /// <summary>
    /// 단위 벡터 반환 - 2D,3D
    /// </summary>
    /// <returns></returns>
    public static Vec2 OneVector2D()=> new Vec2(1, 1);
    public static Vec3 OneVector3D()=> new Vec3(1, 1, 1);

    /// <summary>
    /// 영 벡터 반환 - 2D,3D
    /// </summary>
    /// <returns></returns>
    public static Vec2 ZeroVector2D() => new Vec2(0, 0);
    public static Vec3 ZeroVector3D() => new Vec3(0, 0, 0);

    /// <summary>
    /// 방향 벡터 반환
    /// </summary>
    /// <returns></returns>
    public static Vec2 LeftVector2D()=>new Vec2(-1, 0);
    public static Vec2 RightVector2D() => new Vec2(1, 0);
    public static Vec2 UpVector2D() => new Vec2(0, 1);
    public static Vec2 DownVector2D() => new Vec2(0, -1);
    public static Vec3 LeftVector3D()=>new Vec3(-1, 0, 0);
    public static Vec3 RightVector3D() => new Vec3(1, 0, 0);
    public static Vec3 UpVector3D() => new Vec3(0, 1, 0);
    public static Vec3 DownVector3D() => new Vec3(0, -1, 0);
    public static Vec3 FrontVector3D() => new Vec3(0, 0, 1);
    public static Vec3 BackVector3D() => new Vec3(0, 0, -1);


    /// <summary>
    /// 투영 벡터 구하기
    /// 벡터 v를 벡터 a로 정사영한 벡터 계산
    /// 벡터가 주어졌을때 계산
    /// </summary>
    /// <param name="v">구할 벡터</param>
    /// <param name="axis">투영할 벡터</param>
    /// <returns></returns>
    public static Vec3 Project(Vec3 v, Vec3 axis)
    {
        float dotValue = Vec3.Dot(v, axis);
        return axis*(dotValue / axis.Square);
    }

    /// <summary>
    /// 평면 위로의 투영 벡터
    /// 평면이 있을때 계산
    /// </summary>
    /// <param name="v">구할 벡터</param>
    /// <param name="n">평면의 법선 벡터</param>
    /// <returns></returns>
    public static Vec3 ProjectOnPlane(Vec3 v, Vec3 n)
    {
        return v - Project(v, n);
    }

    /// <summary>
    /// 벡터 분해
    /// 1) 평면과 평행한 벡터
    /// 2) 법선 벡터와 평행한 벡터
    /// </summary>
    /// <param name="v">구할 벡터</param>
    /// <param name="normal">법선 벡터</param>
    /// <param name="perp">평면과 평행한 벡터</param>
    /// <param name="parallel">법선 벡터와 평행한 벡터</param>
    public static void Decompose(Vec3 v, Vec3 normal, out Vec3 perp, out Vec3 parallel)
    {
        parallel = ProjectOnPlane(v, normal);//평면에 투영한다.
        perp = Project(v, normal);//법선 벡터에 투영한다.
    }

    /// <summary>
    /// 반사 벡터 구하기 - 2D
    /// 반발 계수를 적용
    /// </summary>
    /// <param name="v">입력 벡터</param>
    /// <param name="normal">평면의 법선 벡터</param>
    /// <param name="restitution">반발 계수</param>
    /// <returns></returns>
    public static Vec2 ReflectWithRestitution2D(Vec2 v, Vec2 normal, float restitution)
    {
        float dotPN = -Vec2.Dot(v, normal);
        Vec2 reflectVec = v + normal * (2 * dotPN);
        return reflectVec * restitution;
    }
    /// <summary>
    /// 반사 벡터 구하기 - 3D
    /// 반발 계수를 적용
    /// </summary>
    /// <param name="v">입력 벡터</param>
    /// <param name="normal">평면의 법선 벡터</param>
    /// <param name="restitution">반발 계수</param>
    /// <returns></returns>
    public static Vec3 ReflectWithRestitution3D(Vec3 v, Vec3 normal, float restitution)
    {
        float dotPN = -Vec3.Dot(v,normal);
        Vec3 reflectVec = v + normal*(2*dotPN);
        return reflectVec * restitution;
    }

    /// <summary>
    /// 충돌 충격량 계산
    /// </summary>
    /// <param name="v1">충돌전 물체1의 속도 벡터</param>
    /// <param name="v2">충돌전 물체2의 속도 벡터</param>
    /// <param name="m1">물체 1의 질량</param>
    /// <param name="m2">물체 2의 질량</param>
    /// <param name="normal">충돌 지점의 법선 벡터</param>
    /// <param name="e">반발 계수</param>
    /// <returns></returns>
    public static Vec3 ComputeImpulse(Vec3 v1, Vec3 v2, float m1, float m2, Vec3 normal, float e)
    {
        //법선 방향을 따른 상대 속도, 두 물체가 서로 다가오는 속도
        float rv = Vec3.Dot(v2 - v1, normal);
        //충격량
        float j = CalImpulseAmount(m1,m2,rv,e);
        return normal * j;//최종 충격량 벡터 = 법선 벡터 * 충격량
    }

    /// <summary>
    /// 두물체가 충돌했을 때의 충격량
    /// </summary>
    /// <param name="m1">물체1 질량</param>
    /// <param name="m2">물체2 질량</param>
    /// <param name="relVel AlongNormal">법선 방향을 따른 상대 속도</param>
    /// <param name="e">반발계수</param>
    /// <returns></returns>
    public static float CalImpulseAmount(float m1, float m2, float relVelAlongNormal, float e)
    {
        return -(1 + e) * relVelAlongNormal / (1 / m1 + 1 / m2);
    }
    /// <summary>
/// Vector a에 임의 수직인 축 계산
/// </summary>
/// <param name="a"></param>
/// <returns></returns>
public static Vec3 AnyPerpendicular(Vec3 a)
{
    // this는 정규화된 벡터라고 가정
    Vec3 n = a.Normalized;

    //축이 거의 평행할 때 cross가 0 되는 걸 피해야 함
    Vec3 basis;
    if (MathUtility.Abs(n.x) < 0.9f)
        basis = new Vec3(1, 0, 0);
    else
        basis = new Vec3(0, 1, 0);

    // 수직 벡터 생성
    Vec3 perp = Vec3.Cross(n, basis);

    return perp.Normalized;
}
}
