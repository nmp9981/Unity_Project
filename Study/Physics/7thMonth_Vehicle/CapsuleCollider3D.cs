public class CapsuleCollider3D : CustomCollider3D
{
    public float radius;
    public float halfHeight;
    public override CustomColliderType Type => CustomColliderType.Capsule;

    /// <summary>
    /// 월드 공간 캡슌 형성
    /// </summary>
    /// <returns></returns>
    public Capsule GetWorldCapsule()
    {
        transform3D.UpdateMatrices();

        Vec3 up = transform3D.Up;
        Vec3 center = transform3D.position;

        float halfLine =
            MathUtility.Max(0.0f, 2 * halfHeight * 0.5f - radius);

        Vec3 a = center + up * halfLine;
        Vec3 b = center - up * halfLine;
        Capsule newCapsule = new Capsule();
        newCapsule.a = a;
        newCapsule.b = b;
        newCapsule.r = radius;

        return newCapsule;
    }

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
        Capsule capsule = GetWorldCapsule();

        bool hasHit = false;
        float bestT = maxT;
        RaycastHit3D bestHit = default;

        Vec3 d = ray.dir;            // Ray 방향
        Vec3 m = ray.origin - capsule.a;    // Ray 시작 → A
        Vec3 n = capsule.b-capsule.a;             // Capsule 축

        //2차 방정식 만들기
        float nn = Vec3.Dot(n, n);
        float nd = Vec3.Dot(n, d);
        float mn = Vec3.Dot(m, n);

        Vec3 dPerp = d - n * (nd / nn);//d를 n에 투영
        Vec3 mPerp = m - n * (mn / nn);//m을 n에 투영

        float a = Vec3.Dot(dPerp, dPerp);
        float b = Vec3.Dot(dPerp, mPerp);
        float c = Vec3.Dot(mPerp, mPerp) - capsule.r * capsule.r;

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
                        Vec3 hitPos = ray.origin + t * d;//부딪힌 지점
                        Vec3 proj = capsule.a + s * n;//n에 투영

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
        CheckSphere(capsule.a, capsule.r, ray, ref bestT, ref bestHit, ref hasHit);
        CheckSphere(capsule.b, capsule.r, ray, ref bestT, ref bestHit, ref hasHit);

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
    public void CheckSphere(Vec3 center, float radius, Ray3D ray, ref float bestT, ref RaycastHit3D bestHit, ref bool hasHit)
    {
        //2차 방정식으로 변환
        Vec3 m = ray.origin - center;
        Vec3 d = ray.dir;

        float a = Vec3.Dot(d, d); // 정규화 시 = 1
        float b = Vec3.Dot(d, m);
        float c = Vec3.Dot(m, m) - radius * radius;

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

        Vec3 hitPos = ray.origin + t * d;
        Vec3 normal = (hitPos - center).Normalized;

        bestT = t;
        bestHit.t = t;
        bestHit.position = hitPos;
        bestHit.normal = normal;
        bestHit.collider = this;
        hasHit = true;
    }

    /// <summary>
    /// Sphere Sweep vs Capsule
    /// </summary>
    public override bool SweepSphere(
        Vec3 center,
        float radius,
        Vec3 dir,
        float maxT,
        out SweepHit hit)
    {
        // 1. Capsule (정지)
        Capsule capsule = GetWorldCapsule();
        Vec3 capsuleVel = VectorMathUtils.ZeroVector3D();

        // 2. Sphere (이동)
        Sphere sphere;
        sphere.center = center;
        sphere.radius = radius;
        Vec3 sphereVel = dir;

        // 3. 일반 Sweep 함수 호출
        return SphereSweep.SweepCapsuleSphere(capsule,capsuleVel,sphere,sphereVel,maxT,out hit);
    }
}
