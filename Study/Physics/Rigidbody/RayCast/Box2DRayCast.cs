public class Box2DRayCast : CustomCollider2D
{
    Vec2 position;        // world, OBB중심의 월드 좌표
    Vec2 halfExtent;      // local, Box의 로컬 좌표계에서 각 축 방향으로의 반길이
    float angle;//x축 기준 회전 각도

    /// <summary>
    /// Box Raycast
    /// normal은 오직 tMin이 갱신될 때만 바뀐다
    /// </summary>
    /// <param name="ray"></param>
    /// <param name="maxT">최대 거리</param>
    /// <param name="hit">부딪힌 물체</param>
    /// <returns></returns>
    public override bool RayCast2D(Ray2D ray, float maxT, out RaycastHit2D hit)
    {
        // 1. localRay : Box의 로컬 좌표계에서 본 Ray 정보
        Ray2D localRay;
        localRay.origin = Vec2.Rotation((ray.origin - position),-angle);//점 변환, ray 시작점
        localRay.dir = Vec2.Rotation(ray.dir,-angle);//벡터 변환

        // 2. slab test using halfExtent
        // P(t) = origin + t * dir
        float tMin = 0.0f;//Ray가 Box에 “들어오는” 가장 늦은 시점, 세 축 조건을 모두 만족하기 시작하는 최초 시점
        float tMax = maxT;//Ray가 Box에서 “나가는” 가장 이른 시점, 어느 한 축이라도 Box를 벗어나기 시작하는 최초 시점

        Vec2 enterNormalLocal = VectorMathUtils.ZeroVector2D();//tMin을 갱신한 최종 진입 면 normal
        Vec2 exitNormalLocal = VectorMathUtils.ZeroVector2D();//tMax를 갱신한 최종 탈출 면 normal

        //각 x,y축에 대해 진행
        for (int axis = 0; axis < 2; axis++)
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

            Vec2 axisEnterNormal = VectorMathUtils.ZeroVector2D();//현재 축 slab에 진입하는 면의 로컬 normal
            Vec2 axisExitNormal = VectorMathUtils.ZeroVector2D();//현재 축 slab에서 빠져나가는 면의 로컬 normal

            axisEnterNormal.Array[axis] = (dir > 0) ? -1 : 1;
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

            //겹치는 지점 없음
            if (tMin > tMax)
            {
                hit = default;
                return false;
            }
        }

        //Ray 전체가 Box 뒤쪽에 있는 경우 제거
        if (tMax < 0.0f)
        {
            hit = default;
            return false;
        }

        //최종 Hit 결정
        bool inside = (tMin < 0.0f);
        hit.t = inside ? tMax : tMin;//실제 충돌 지점의 Ray parameter
        //Raycast 최대 거리 밖에서 맞은 경우
        if (hit.t < 0.0f || hit.t > maxT)
        {
            hit = default;
            return false;
        }

        hit.normal = Vec2.Rotation((inside ? exitNormalLocal : enterNormalLocal),angle); //월드 좌표로 변환해야함, Box면의 월드공간 법선
        hit.normal = hit.normal.Normalized;
        hit.position = ray.origin + hit.t * ray.dir;//월드 좌표 충돌 지점
        hit.collider = this;//충돌한 collider (this)
        return true;
    }
}
