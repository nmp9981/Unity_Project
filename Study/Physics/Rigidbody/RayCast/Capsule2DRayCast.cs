using NUnit.Framework.Internal;

public class Capsule2DRayCast : CustomCollider2D
{
    Vec2 pointA;   // world (capsule 시작)
    Vec2 pointB;   // world (capsule 끝)
    float radius; //구쪽 반지름

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
        bool hasHit = false;
        float bestT = maxT;
        RaycastHit2D bestHit = default;

        Vec2 d = ray.dir;            // Ray 방향
        Vec2 m = ray.origin - pointA;    // Ray 시작 → A
        Vec2 n = pointB - pointA;             // Capsule 축

        //2차 방정식 만들기
        float nn = Vec2.Dot(n, n);
        float nd = Vec2.Dot(n, d);
        float mn = Vec2.Dot(m, n);

        Vec2 dPerp = d - n * (nd / nn);//d를 n에 투영
        Vec2 mPerp = m - n * (mn / nn);//m을 n에 투영

        float a = Vec2.Dot(dPerp, dPerp);
        float b = Vec2.Dot(dPerp, mPerp);
        float c = Vec2.Dot(mPerp, mPerp) - radius * radius;

        if (a > MathUtility.EPSILON)
        {
            //판별식
            float discr = b * b - a * c;
            //실수해 존재
            if (discr >= 0.0f)
            {
                float sqrtD = MathUtility.Root(discr);
                float t0 = (-b - sqrtD) / a;
                float t1 = (-b + sqrtD) / a;

                for (int i = 0; i < 2; i++)
                {
                    float t = (i == 0) ? t0 : t1;
                    //해 허용 범위를 넘김
                    if (t < 0.0f || t > bestT) continue;

                    //원통 내에 있는지 판정
                    float s = (mn + t * nd) / nn;
                    if (s >= 0.0f && s <= 1.0f)
                    {
                        Vec2 hitPos = ray.origin + t * d;//부딪힌 지점
                        Vec2 proj = pointA + s * n;//n에 투영

                        Vec2 normal = (hitPos - proj).Normalized;

                        //충돌체 정보
                        bestT = t;
                        bestHit.t = t;
                        bestHit.position = hitPos;
                        bestHit.normal = normal;
                        bestHit.collider = this;
                        hasHit = true;
                    }
                }
            }
        }

        // Endcaps
        CheckCircle(pointA, radius, ray, ref bestT, ref bestHit, ref hasHit);
        CheckCircle(pointB, radius, ray, ref bestT, ref bestHit, ref hasHit);

        hit = bestHit;
        return hasHit;
    }

    /// <summary>
    /// Circle Raycast
    /// normal은 오직 tMin이 갱신될 때만 바뀐다
    /// </summary>
    /// <param name="ray"></param>
    /// <param name="maxT">ray 최대 거리</param>
    /// <param name="hit">부딪힌 물체</param>
    /// <returns></returns>
    public void CheckCircle(Vec2 center,float radius,Ray2D ray,ref float bestT,ref RaycastHit2D bestHit,ref bool hasHit)
    {
        //2차 방정식으로 변환
        Vec2 m = ray.origin - center;
        Vec2 d = ray.dir;

        float a = Vec2.Dot(d, d); // 정규화 시 = 1
        float b = Vec2.Dot(d, m);
        float c = Vec2.Dot(m, m) - radius * radius;

        //2차 방정식의 판별식
        float discr = b * b - a * c;
        if (discr < 0.0f) return;//허근, 실수해 없음
        
        //실수해
        float sqrtD = MathUtility.Root(discr);
        float t0 = (-b - sqrtD) / a;//min
        float t1 = (-b + sqrtD) / a;//max

        // Ray 뒤쪽
        if (t1 < 0.0f) return;

        float t = (t0 >= 0.0f) ? t0 : t1;
        if (t < 0.0f || t > bestT) return;

        Vec2 hitPos = ray.origin + t * d;
        Vec2 normal = (hitPos - center).Normalized;

        bestT = t;
        bestHit.t = t;
        bestHit.position = hitPos;
        bestHit.normal = normal;
        bestHit.collider = this;
        hasHit = true;
    }
}
