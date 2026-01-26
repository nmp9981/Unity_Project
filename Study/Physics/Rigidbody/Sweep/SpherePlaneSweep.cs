/// <summary>
/// Sphere vs Plane
/// </summary>
public class SpherePlaneSweep
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
    bool SweepSpherePlane(Vec3 center,float radius,Vec3 dir,float maxT,Plane plane,out SweepHit hit)
    {
        hit = default;

        Vec3 n = plane.normal;//평면의 법선벡터
        float denom = Vec3.Dot(n, dir);//평면과 구 사이의 방향

        //구가 평면으로부터 멀어짐
        if (denom > 0) return false;

        //평면과 구 사이의 거리
        float dist = Vec3.Dot(plane.normal, center)-plane.d;
        //관통
        if (dist < radius) return false;

        //접촉 시간
        float t = (radius - dist) / denom;
        if(t<0 || t > maxT) return false;

        //평면에 접촉, Hit 정보 구성
        hit.t = t;
        hit.normal = n;
        hit.point = center + dir * t - n * radius;//실제 접촉점
        return true;
    }
}
