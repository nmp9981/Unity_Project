public class CircleRayCast : CustomCollider2D
{
    Vec2 center;        // world
    float radius;      // 반지름

    /// <summary>
    /// Shpere Raycast
    /// normal은 오직 tMin이 갱신될 때만 바뀐다
    /// </summary>
    /// <param name="ray"></param>
    /// <param name="maxT">ray 최대 거리</param>
    /// <param name="hit">부딪힌 물체</param>
    /// <returns></returns>
    public override bool RayCast2D(Ray2D ray, float maxT, out RaycastHit2D hit)
    {
        //2차 방정식으로 변환
        Vec2 m = ray.origin - center;

        float a = Vec2.Dot(ray.dir, ray.dir); // 정규화 시 = 1
        float b = Vec2.Dot(ray.dir, m);
        float c = Vec2.Dot(m, m) - radius * radius;

        //2차 방정식의 판별식
        float discr = b * b - a * c;
        if (discr < 0.0f)//허근, 실수해 없음
        {
            hit = default;
            return false;
        }

        //실수해
        float sqrtD = MathUtility.Root(discr);
        float t0 = (-b - sqrtD) / a;//min
        float t1 = (-b + sqrtD) / a;//max

        // Ray 뒤쪽
        if (t1 < 0.0f)
        {
            hit = default;
            return false;
        }

        bool inside = (t0 < 0.0f);
        float t = inside ? t1 : t0;

        //겹치지 않음
        if (t > maxT)
        {
            hit = default;
            return false;
        }

        //최종 충돌체 정보
        hit.t = t;
        hit.position = ray.origin + t * ray.dir;
        hit.normal = (hit.position - center).Normalized;
        hit.collider = this;
        return true;
    }
}
