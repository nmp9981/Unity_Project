public class SweepSphereSphere
{
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
    public static bool SweepSphereAndSphere(Vec3 center0,float radius0,Vec3 dir,float maxT,Vec3 center1,float radius1,out SweepHit hit)
    {
        hit = default;

        Vec3 m = center0 - center1;
        float R = radius0 + radius1;

        //판별식 계수
        float b = Vec3.Dot(m, dir);
        float c = Vec3.Dot(m, m) - R*R;

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
}
