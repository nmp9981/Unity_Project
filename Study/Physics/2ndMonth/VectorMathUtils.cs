using UnityEngine;

public static class VectorMathUtils
{
    /// <summary>
    /// 투영 벡터 구하기
    /// 벡터 v를 벡터 a로 정사영한 벡터 계산
    /// 벡터가 주어졌을때 계산
    /// </summary>
    /// <param name="v">구할 벡터</param>
    /// <param name="axis">투영할 벡터</param>
    /// <returns></returns>
    public static Vector3 Project(Vector3 v, Vector3 axis)
    {
        return Vector3.Dot(v, axis) / Vector3.Dot(axis, axis) * axis;
    }

    /// <summary>
    /// 평면 위로의 투영 벡터
    /// 평면이 있을때 계산
    /// </summary>
    /// <param name="v">구할 벡터</param>
    /// <param name="n">평면의 법선 벡터</param>
    /// <returns></returns>
    public static Vector3 ProjectOnPlane(Vector3 v, Vector3 n)
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
    public static void Decompose(Vector3 v, Vector3 normal, out Vector3 perp, out Vector3 parallel)
    {
        parallel = ProjectOnPlane(v, normal);//평면에 투영한다.
        perp = Project(v, normal);//법선 벡터에 투영한다.
    }

    /// <summary>
    /// 반사 벡터 구하기
    /// 반발 계수를 적용
    /// </summary>
    /// <param name="v">구할 벡터</param>
    /// <param name="normal">평면의 법선 벡터</param>
    /// <param name="restitution">반발 계수</param>
    /// <returns></returns>
    public static Vector3 ReflectWithRestitution(Vector3 v, Vector3 normal, float restitution)
    {
        return Vector3.Reflect(v, normal) * restitution;
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
    public static Vector3 ComputeImpulse(Vector3 v1, Vector3 v2, float m1, float m2, Vector3 normal, float e)
    {
        //법선 방향을 따른 상대 속도, 두 물체가 서로 다가오는 속도
        float rv = Vector3.Dot(v2 - v1, normal);
        //충격량
        float j = -(1 + e) * rv / (1.0f / m1 + 1.0f / m2);
        return normal * j;//최종 충격량 벡터 = 법선 벡터 * 충격량
    }
}
