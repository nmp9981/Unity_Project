using UnityEngine;

public class SphereSweep
{
    /// <summary>
    /// 구와 평면 충돌
    /// </summary>
    /// <param name="center">C0</param>
    /// <param name="radius">r</param>
    /// <param name="dir">구의 방향(정규화)</param>
    /// <param name="maxT">최대 판정 거리</param>
    /// <param name="plane">바닥</param>
    /// <param name="hit">부딪힌 물체</param>
    /// <returns></returns>
    public static bool SweepSpherePlane(Vec3 center, float radius, Vec3 dir, float maxT, Plane plane, out SweepHit hit)
    {
        hit = default;

        Vec3 n = plane.normal;//평면의 법선벡터
        float denom = Vec3.Dot(n, dir);//평면과 구 사이의 방향

        //구가 평면으로부터 멀어짐
        if (denom > 0) return false;

        //평면과 구 사이의 거리
        float dist = Vec3.Dot(plane.normal, center) - plane.d;
        //관통
        if (dist < radius) return false;

        //접촉 시간
        float t = (radius - dist) / denom;
        if (t < 0 || t > maxT) return false;

        //평면에 접촉, Hit 정보 구성
        hit.t = t;
        hit.normal = n;
        hit.point = center + dir * t - n * radius;//실제 접촉점
        return true;
    }

    /// <summary>
    /// 구와 구 충돌
    /// 움직이는 구vs 고정 구
    /// </summary>
    /// <param name="center0">구의 중심</param>
    /// <param name="radius0">구의 반지름</param>
    /// <param name="dir">구0이 움직이는 방향</param>
    /// <param name="maxT">최대 인정 T</param>
    /// <param name="center1">구의 중심</param>
    /// <param name="radius1">구의 반지름</param>
    /// <param name="hit">충돌체</param>
    /// <returns></returns>
    public static bool SweepSphereAndSphere(Vec3 center0, float radius0, Vec3 dir, float maxT, Vec3 center1, float radius1, out SweepHit hit)
    {
        hit = default;

        Vec3 m = center0 - center1;
        float R = radius0 + radius1;

        //판별식 계수
        float b = Vec3.Dot(m, dir);
        float c = Vec3.Dot(m, m) - R * R;

        // 이미 겹침
        if (c < 0.0f) return false;

        // 멀어지는 중
        if (b > 0.0f) return false;

        //판별식
        float discr = b * b - c;
        if (discr < 0.0f)//허근
            return false;

        //해, 이른 접촉이므로 더 작은해
        float t = -b - MathUtility.Root(discr);

        //t범위 벗어남
        if (t < 0.0f || t > maxT)
            return false;

        // Hit 정보
        hit.t = t;

        Vec3 hitCenter0 = center0 + dir * t;
        Vec3 normal = (hitCenter0 - center1).Normalized;

        hit.normal = normal;
        hit.point = hitCenter0 - normal * radius0;

        return true;
    }

    /// <summary>
    /// 구, 박스 충돌
    /// 움직이지 않는 구 VS 확장 box
    /// </summary>
    /// <param name="s"></param>
    /// <param name="b"></param>
    /// <param name="d"></param>
    /// <param name="maxT"></param>
    /// <param name="hit"></param>
    /// <returns></returns>
    bool SweepSphereBox(SphereCollider s,CustomCollider3D b,Vec3 d,float maxT,out SweepHit hit)
    {
        hit = default;

        //box 확장
        Vec3 expandedMin = b.minPosition() - s.radius * VectorMathUtils.OneVector3D();
        Vec3 expandedMax = b.maxPosition() + s.radius * VectorMathUtils.OneVector3D();

        float tmin = 0f;
        float tmax = maxT;
        Vec3 normal = VectorMathUtils.ZeroVector3D();

        //각 축별로 계산
        for (int axis = 0; axis < 3; axis++)
        {
            float p = s.center[axis];
            float v = d.Array[axis];

            if (Mathf.Abs(v) < MathUtility.EPSILON)
            {
                if (p < expandedMin.Array[axis] || p > expandedMax.Array[axis])
                    return false;
            }
            else
            {
                float invV = 1f / v;
                float t1 = (expandedMin.Array[axis] - p) * invV;
                float t2 = (expandedMax.Array[axis] - p) * invV;

                float enter =MathUtility.Min(t1, t2);
                float exit = MathUtility.Max(t1, t2);

                if (enter > tmin)
                {
                    tmin = enter;
                    normal = VectorMathUtils.ZeroVector3D();
                    normal.Array[axis] = (t1 > t2) ? 1f : -1f;
                }

                tmax = MathUtility.Min(tmax, exit);

                if (tmin > tmax)
                    return false;
            }
        }

        //해 범위를 넘김
        if (tmin < 0 || tmin > maxT)
            return false;

        //최종 충돌체 정보
        hit.t = tmin;
        hit.normal = normal;
        Vec3 sphereCenter = new Vec3(s.center.x,s.center.y,s.center.z);
        hit.point = sphereCenter + d * tmin - normal * s.radius;

        return true;
    }
}
