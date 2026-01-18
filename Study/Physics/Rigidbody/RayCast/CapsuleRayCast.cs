// “선분에 대한 거리” + 양 끝 Sphere 보정
public class CapsuleRayCast : CustomCollider3D
{
    Vec3 pointA;   // world (capsule 시작)
    Vec3 pointB;   // world (capsule 끝)
    float radius; //구쪽 반지름

    /// <summary>
    /// Shpere Raycast
    /// normal은 오직 tMin이 갱신될 때만 바뀐다
    /// </summary>
    /// <param name="ray"></param>
    /// <param name="maxT">ray 최대 거리</param>
    /// <param name="hit">부딪힌 물체</param>
    /// <returns></returns>
    public override bool RayCast(Ray3D ray, float maxT, out RaycastHit3D hit)
    {
        bool hasHit = false;
        float bestT = maxT;
        RaycastHit3D bestHit = default;

        Vec3 d = ray.dir;            // Ray 방향
        Vec3 m = ray.origin - pointA;    // Ray 시작 → A
        Vec3 n = pointB-pointA;             // Capsule 축

        //2차 방정식 만들기
        float nn = Vec3.Dot(n,n);
        float nd = Vec3.Dot(n,d);
        float mn = Vec3.Dot(m,n);

        Vec3 dPerp = d - n * (nd / nn);//d를 n에 투영
        Vec3 mPerp = m - n * (mn / nn);//m을 n에 투영

        float a = Vec3.Dot(dPerp,dPerp);
        float b = Vec3.Dot(dPerp,mPerp);
        float c = Vec3.Dot(mPerp,mPerp) - radius * radius;

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
                    if (t < 0.0f || t > bestT) continue;

                    float s = (mn + t * nd) / nn;
                    if (s >= 0.0f && s <= 1.0f)
                    {
                        Vec3 hitPos = ray.origin + t * d;
                        Vec3 proj = pointA + s * n;

                        Vec3 normal = (hitPos - proj).Normalized;

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
        CheckSphere(pointA, radius, ray, ref bestT, ref bestHit, ref hasHit);
        CheckSphere(pointB, radius, ray, ref bestT, ref bestHit, ref hasHit);

        hit = bestHit;
        return hasHit;
    }
}
