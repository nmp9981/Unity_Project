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
    public static bool SweepSphereBox(SphereCollider s,CustomCollider3D b,Vec3 d,float maxT,out SweepHit hit)
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
            float p = s.center[axis];//각 축 point
            float v = d.Array[axis];//각 축별 방향

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

    /// <summary>
    /// 구와 OBB충돌 검사
    /// </summary>
    /// <param name="s"></param>
    /// <param name="obb"></param>
    /// <param name="dir"></param>
    /// <param name="maxT"></param>
    /// <param name="hit"></param>
    /// <returns></returns>
    public static bool SweepSphereOBB(Vec3 sphereCenter,float sphereRadius,Vec3 dir,float maxT, Transform3D obbTransform,Vec3 obbHalfExtent, CustomCollider3D collider,out SweepHit hit)
    {
        //world -> OBB Local
        Mat3 R = obbTransform.rotation;//회전 행렬
        Mat3 invR = MatrixUtility.Transpose(R);//전치(역)행렬

        Vec3 sCenter = sphereCenter;
        float sRadius = sphereRadius;

        Vec3 localCenter = invR*(sCenter-obbTransform.position);//로컬 중심 좌표
        Vec3 localDir = invR* dir;//로컬 방향

        //Local AABB 정의, 확장
        Vec3 min = (-1f)*obbTransform.scale/2 - VectorMathUtils.OneVector3D() * sRadius;
        Vec3 max = obbTransform.scale/2 + VectorMathUtils.OneVector3D() * sRadius;

        //Ray vs AABB
        float t;
        Vec3 localNormal;
        hit = default;
        if (!ColiisionUtility.RayAABB3D(localCenter, localDir, min, max, maxT, out t, out localNormal))
            return false;

        //결과 복원(Local->world)
        Vec3 worldNormal = R * localNormal;

        hit.t = t;
        hit.normal = worldNormal;
        hit.point = sCenter + dir * t - worldNormal * sRadius;
        
        return true;
    }
    /// <summary>
    /// 캡슐과 평면의 충돌
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    /// <param name="r"></param>
    /// <param name="v"></param>
    /// <param name="plane"></param>
    /// <param name="maxT"></param>
    /// <param name="tHit"></param>
    /// <param name="normal"></param>
    /// <returns></returns>
    public static bool SweepCapsulePlane(Vec3 A, Vec3 B, float r,Vec3 v, Plane plane, float maxT, out float tHit,out Vec3 normal)
    {
        tHit = maxT;
        normal = plane.normal;

        float dA = Vec3.Dot(plane.normal, A) - plane.d;//점 A거리
        float dB = Vec3.Dot(plane.normal, B) - plane.d;//점 B거리
        float vN = Vec3.Dot(plane.normal, v);//캡슐 속도 방향

        //멀어짐
        if (vN >= 0.0f)
            return false;

        bool hit = false;

        float tA = (r - dA) / vN;
        if(tA>=0 && tA <= tHit)//점A에 부딪힘
        {
            tHit = tA;
            hit = true;
        }
        float tB = (r - dB) / vN;
        if (tB >= 0 && tB <= tHit)//점B에 부딪힘
        {
            tHit = tB;
            hit = true;
        }
        return hit;
    }
    /// <summary>
    /// 캡슐 충돌
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    /// <param name="r"></param>
    /// <param name="v"></param>
    /// <param name="plane"></param>
    /// <param name="maxT"></param>
    /// <param name="tHit"></param>
    /// <param name="normal"></param>
    /// <returns></returns>
    public static bool SweepCapsuleSphere(Capsule capsule, Vec3 capsuleVel, Sphere sphere, Vec3 sphereVel, float maxT, out SweepHit hit)
    {
        hit = default;

        Vec3 relVel = sphereVel - capsuleVel;
        float R = capsule.r + sphere.radius;

        bool hasHit = false;
        float bestT = maxT;
        Vec3 bestNormal = VectorMathUtils.ZeroVector3D();

        // 1️⃣ Sphere vs Capsule A
        if (SweepSpherePoint(sphere.center, relVel, capsule.a, R, maxT, out float tA, out Vec3 nA))
        {
            if (tA < bestT)
            {
                bestT = tA;
                bestNormal = nA;
                hasHit = true;
            }
        }

        // 2️⃣ Sphere vs Capsule B
        if (SweepSpherePoint(sphere.center, relVel, capsule.b, R, maxT, out float tB, out Vec3 nB))
        {
            if (tB < bestT)
            {
                bestT = tB;
                bestNormal = nB;
                hasHit = true;
            }
        }

        // 3️⃣ Sphere vs Capsule Side (Infinite Cylinder)
        if (SweepSphereCapsuleSide(capsule, sphere.center, relVel, R, maxT, out float tSide, out Vec3 nSide))
        {
            if (tSide < bestT)
            {
                bestT = tSide;
                bestNormal = nSide;
                hasHit = true;
            }
        }

        if (!hasHit)
            return false;

        //충돌체 정보
        hit.t = bestT;
        hit.normal = bestNormal.Normalized;
        hit.point = sphere.center + bestT * sphereVel - hit.normal * sphere.radius;
        return true;
    }
    /// <summary>
    /// 구 vs 기둥
    /// </summary>
    /// <param name="capsule"></param>
    /// <param name="sphereCenter"></param>
    /// <param name="relVel"></param>
    /// <param name="radius"></param>
    /// <param name="maxT"></param>
    /// <param name="t"></param>
    /// <param name="normal"></param>
    /// <returns></returns>
    public static bool SweepSphereCapsuleSide(Capsule capsule,Vec3 sphereCenter,Vec3 relVel,float radius,float maxT,out float t,out Vec3 normal)
    {
        t = 0;
        normal = VectorMathUtils.ZeroVector3D();

        //축 정의
        Vec3 d = capsule.b - capsule.a;//실제 거리
        float len = d.Magnitude;//거리
        Vec3 axis = d / len;//축

        //상대 위치를 축에 분해
        Vec3 m = sphereCenter - capsule.a;
        Vec3 mPerp = m - axis * Vec3.Dot(m, axis);
        Vec3 vPerp = relVel - axis * Vec3.Dot(relVel, axis);

        //원기둥 충돌 방정식
        float a = Vec3.Dot(vPerp, vPerp);
        float b = 2 * Vec3.Dot(mPerp, vPerp);
        float c = Vec3.Dot(mPerp, mPerp) - radius * radius;

        //방정식 풀이
        if (!MathUtility.SolveEquation2(a, b, c, out float t0, out float t1))
            return false;
        float hitT = (t0 >= 0) ? t0 : t1;
        //해 범위를 벗어남
        if (hitT < 0 || hitT > maxT) return false; 

        //선분 범위 체크
        float s = Vec3.Dot(axis, m+hitT*relVel);
        //선분 범위를 벗어남
        if (s < 0 || s > len) return false;

        t = hitT;

        //충돌체 정보
        Vec3 hitCenter = sphereCenter + t * relVel;
        Vec3 closest = capsule.a + s * axis;
        normal = (hitCenter - closest).Normalized;

        return true;
    }
    /// <summary>
    /// 끝구
    /// </summary>
    /// <param name="center"></param>
    /// <param name="vel"></param>
    /// <param name="point"></param>
    /// <param name="radius"></param>
    /// <param name="maxT"></param>
    /// <param name="t"></param>
    /// <param name="normal"></param>
    /// <returns></returns>
    public static bool SweepSpherePoint(Vec3 center,Vec3 vel,Vec3 point,float radius,float maxT,out float t,out Vec3 normal)
    {
        t = 0;
        normal = VectorMathUtils.ZeroVector3D();

        Vec3 m = center - point;

        //구 방정식
        float a = Vec3.Dot(vel, vel);
        float b = 2 * Vec3.Dot(vel, m);
        float c = Vec3.Dot(m, m) - radius * radius;

        //방정식 풀이
        if (!MathUtility.SolveEquation2(a, b, c, out float t0, out float t1))
            return false;
        float hitT = (t0 >= 0) ? t0 : t1;
        //해 범위를 벗어남
        if (hitT < 0 || hitT > maxT) return false;

        t = hitT;
        //충돌체 정보
        Vec3 hitPos = center + t * vel;
        normal = (hitPos-point).Normalized;

        return true;
    }
    /// <summary>
    /// 캡슐 vs OBB box
    /// </summary>
    /// <param name="capsule"></param>
    /// <param name="capsuleVel"></param>
    /// <param name="box"></param>
    /// <param name="boxVel"></param>
    /// <param name="maxT"></param>
    /// <param name="hit"></param>
    /// <returns></returns>
    public static bool SweepCapsuleOBB(Capsule capsule,Vec3 capsuleVel,Box box,Vec3 boxVel,float maxT,out SweepHit hit)
    {
        hit = default;

        // 1️⃣ 상대 속도
        Vec3 relVel = capsuleVel - boxVel;

        // 2️⃣ OBB → 로컬
        Vec3 localA = box.WorldToLocalPoint(capsule.a);
        Vec3 localB = box.WorldToLocalPoint(capsule.b);
        Vec3 localVel = box.WorldToLocalVector(relVel);

        // 3️⃣ Minkowski 확장 AABB
        Vec3 expand = new Vec3(capsule.r, capsule.r, capsule.r);
        Vec3 min = (-1)*box.halfExtent - expand;
        Vec3 max = box.halfExtent + expand;

        bool hasHit = false;
        float bestT = maxT;
        Vec3 bestNormalLocal = VectorMathUtils.ZeroVector3D();

        // 4️⃣ A endpoint
        Ray3D aRay = new Ray3D();
        aRay.origin = localA;
        aRay.dir = localVel;
        if (ColiisionUtility.RayAABB3D(localA,localVel,min, max, maxT,out float tA, out Vec3 nA))
        {
            if (!hasHit || tA < bestT)
            {
                bestT = tA;
                bestNormalLocal = nA;
                hasHit = true;
            }
        }

        // 5️⃣ B endpoint
        Ray3D bRay = new Ray3D();
        bRay.origin = localB;
        bRay.dir = localVel;
        if (ColiisionUtility.RayAABB3D(localB,localVel,min, max, maxT,out float tB, out Vec3 nB))
        {
            if (!hasHit || tB < bestT)
            {
                bestT = tB;
                bestNormalLocal = nB;
                hasHit = true;
            }
        }

        if (!hasHit)
            return false;

        // 6️⃣ 월드 복원
        hit.t = bestT;
        hit.normal = box.LocalToWorldVector(bestNormalLocal).Normalized;
        if (Vec3.Dot(hit.normal, relVel) > 0)//normal방향 보정
            hit.normal = (-1)*hit.normal;

        Vec3 dirLocal = box.WorldToLocalVector(capsule.b - capsule.a);

        Vec3 supportLocal =Vec3.Dot(dirLocal, bestNormalLocal) > 0? localB: localA;

        Vec3 support = box.LocalToWorldPoint(supportLocal);
        hit.point = support + capsuleVel * bestT - hit.normal * capsule.r;
        hit.collider = box.collider;
        return true;
    }
}
