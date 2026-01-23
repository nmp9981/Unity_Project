public class BoxRayCast : CustomCollider3D
{
    public Vec3 halfExtent;        // 🔥 Box의 로컬 반길이

    /// <summary>
    /// Box Raycast
    /// normal은 오직 tMin이 갱신될 때만 바뀐다
    /// </summary>
    /// <param name="ray"></param>
    /// <param name="maxT">최대 거리</param>
    /// <param name="hit">부딪힌 물체</param>
    /// <returns></returns>
    public override bool RayCast(Ray3D ray, float maxT, out RaycastHit3D hit)
    {
        //최신화
        transform3D.UpdateMatrices();

        // 1. localRay : Box의 로컬 좌표계에서 본 Ray 정보
        Ray3D localRay;
        localRay.origin = MatrixUtility.MulPoint(ray.origin, transform3D.LocalToWorld);
        localRay.dir = MatrixUtility.MulVector(ray.dir, transform3D.WorldToLocal);

        // 2. slab test using halfExtent
        // P(t) = origin + t * dir
        float tMin = 0.0f;//Ray가 Box에 “들어오는” 가장 늦은 시점, 세 축 조건을 모두 만족하기 시작하는 최초 시점
        float tMax = maxT;//Ray가 Box에서 “나가는” 가장 이른 시점, 어느 한 축이라도 Box를 벗어나기 시작하는 최초 시점

        Vec3 enterNormalLocal = VectorMathUtils.ZeroVector3D();//tMin을 갱신한 최종 진입 면 normal
        Vec3 exitNormalLocal = VectorMathUtils.ZeroVector3D();//tMax를 갱신한 최종 탈출 면 normal

        //각 x,y,z축에 대해 진행
        for (int axis = 0; axis < 3; axis++)
        {
            float origin = localRay.origin.Array[axis];
            float dir = localRay.dir.Array[axis];
            float min = -halfExtent.Array[axis];
            float max = halfExtent.Array[axis];

            //Ray not Move
            if (MathUtility.Abs(dir) < MathUtility.EPSILON)
            {
                // Ray가 slab과 평행
                if (origin < min || origin > max)
                {
                    hit = default;
                    return false;
                }
                continue;
            }

            //min, max기준으로 설정
            float invD = 1.0f / dir;
            float t1 = (min - origin) * invD;
            float t2 = (max - origin) * invD;

            float enter = MathUtility.Min(t1, t2);//들어오는 시점
            float exit = MathUtility.Max(t1, t2);//나가는 시점

            Vec3 axisEnterNormal = VectorMathUtils.ZeroVector3D();//현재 축 slab에 진입하는 면의 로컬 normal
            Vec3 axisExitNormal = VectorMathUtils.ZeroVector3D();//현재 축 slab에서 빠져나가는 면의 로컬 normal

            axisEnterNormal.Array[axis] = (t1 < t2) ? -1 : 1;
            axisExitNormal.Array[axis] = -axisEnterNormal.Array[axis];

            if (enter > tMin)
            {
                tMin = enter;
                enterNormalLocal = axisEnterNormal;
            }

            if (exit < tMax)
            {
                tMax = exit;
                exitNormalLocal = axisExitNormal;
            }

            //Ray 전체가 Box 뒤쪽에 있는 경우 제거
            if (tMax < 0.0f)
            {
                hit = default;
                return false;
            }

            //겹치는 지점 없음
            if (tMin > tMax)
            {
                hit = default;
                return false;
            }
        }

        //최종 Hit 결정
        bool inside = (tMin < 0.0f);
        hit.t = inside ? tMax : tMin;//실제 충돌 지점의 Ray parameter
        Vec3 localNormal = inside ? exitNormalLocal : enterNormalLocal;

        hit.normal = MatrixUtility.MulVector(localNormal, transform3D.LocalToWorld);
        hit.normal = hit.normal.Normalized;
        hit.position = ray.origin + hit.t * ray.dir;//월드 좌표 충돌 지점
        hit.collider = this;//충돌한 collider (this)
        return true;
    }
}
