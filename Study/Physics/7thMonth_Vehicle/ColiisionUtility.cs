using System.Data;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UIElements;

public class ColiisionUtility
{
    public static bool CheckAABBBox(CustomCollider3D aBox, CustomCollider3D bBox)
    {
        return aBox.GetBounds().Intersects(bBox.GetBounds());
    }

    /// <summary>
    /// AABB충돌 여부 - 3D
    /// </summary>
    /// <param name="aBox">오브젝트 A 박스</param>
    /// <param name="bBox">오브젝트 B 박스</param>
    /// <returns></returns>
    public static bool IsCollisionAABB3D(CustomCollider3D aBox, CustomCollider3D bBox)
    {
        //각 축별 충돌 여부
        bool isCollisionX = true;
        bool isCollisionY = true;
        bool isCollisionZ = true;

        var A = aBox.GetBounds();
        var B = bBox.GetBounds();

        //x축 비교
        if (A.max.x < B.min.x || A.min.x > B.max.x)
        {
            isCollisionX = false;
        }
        //y축 비교
        if (A.max.y < B.min.y || A.min.y > B.max.y)
        {
            isCollisionY = false;
        }
        //z축 비교
        if (A.max.z < B.min.z || A.min.z > B.max.z)
        {
            isCollisionZ = false;
        }
        //모두 겹치면 충돌
        if (isCollisionX && isCollisionY && isCollisionZ)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 구끼리의 충돌
    /// </summary>
    /// <param name="center1">구1의 중심 좌표</param>
    /// <param name="r1">구1의 반지름</param>
    /// <param name="center2">구2의 중심 좌표</param>
    /// <param name="r2">구2의 반지름</param>
    /// <returns></returns>
    public static bool IsCollisionCircle(Vec3 center1, float r1, Vec3 center2, float r2)
    {
        //벡터 차
        Vec3 diff = center1 - center2;

        //반지름 합
        float sumRadius = r2 + r1;
        float sumRadius2 = sumRadius * sumRadius;

        //중심간 거리 제곱
        float diffCenter2 = diff.x*diff.x + diff.y*diff.y+diff.z*diff.z;

        //충돌 검사
        if (diffCenter2 < sumRadius2) return true;
        return false;
    }

    /// <summary>
    /// AABB박스와 구와의 충돌 체크
    /// </summary>
    /// <param name="box">AABB박스</param>
    /// <param name="sphereCenter">구의 중심</param>
    /// <param name="radius">구의 반지름</param>
    /// <returns></returns>
    public static bool IsColliderSphere_AABB(CustomCollider3D box, Vec3 sphereCenter, float radius)
    {
        //원과 가장 가까운점 찾기
        var boxMinPos = box.minPosition();
        var boxMaxPos = box.maxPosition();

        float closestX = MathUtility.ClampValue(sphereCenter.x, boxMinPos.x, boxMaxPos.x);
        float closestY = MathUtility.ClampValue(sphereCenter.y, boxMinPos.y, boxMaxPos.y);
        float closestZ = MathUtility.ClampValue(sphereCenter.z, boxMinPos.z, boxMaxPos.z);

        //가장 가까운 점과 원의 중심간 거리 비교
        float dist2 = (closestX - sphereCenter.x) * (closestX - sphereCenter.x)
            + (closestY - sphereCenter.y) * (closestY - sphereCenter.y)
            + (closestZ - sphereCenter.z) * (closestZ - sphereCenter.z);

        //반지름보다 더 작으면 충돌
        return dist2 < radius * radius;
    }

    /// <summary>
    /// Plane과 AABB 충돌 체크 및 충돌 응답 (토요일 수준)
    /// Plane은 정적, RigidBody가 없는 것으로 가정
    /// </summary>
    /// <param name="aabb">움직이는 AABB</param>
    /// <param name="planeNormal">Plane의 법선</param>
    /// <param name="planePoint">Plane 위의 한 점</param>
    public static void ResolvePlaneCollision3D(CustomRigidBody aabb, Vec3 planeNormal, Vec3 planePoint)
    {
        // AABB 반높이
        float halfHeight = aabb.col.GetBounds().extents.y; // AABB Extents 사용
        Vec3 pos = aabb.currentState.position;

        // 바닥면 위치
        float aabbBottom = pos.y - halfHeight;

        // Plane 높이 (y 기준 평면)
        float planeHeight = planePoint.y;

        // 겹침 계산
        float penetration = planeHeight - aabbBottom;

        // 겹침
        if (penetration > 0f)
        {
            // 위치 보정: Plane 위로 이동(겹친만큼 위로 이동)
            pos.y += penetration;
            aabb.currentState.position = pos;

            // 속도 반사 (탄성)
            float restitution = 0.5f; // 반발 계수
            if (aabb.velocity.y < 0)
            {
                aabb.velocity.y = -aabb.velocity.y * restitution;
            }
        }
    }

    /// <summary>
    /// 충돌 정보 계산
    /// </summary>
    public static ContactInfo GetContactAABB3D(CustomCollider3D colA, CustomCollider3D colB)
    {
        //두 바운드박스
        var boundA = colA.GetBounds();
        var boundB = colB.GetBounds();

        //두 바운드 박스 겹침 여부 파악
        if (boundA.max.x < boundB.min.x || boundA.min.x > boundB.max.x ||
    boundA.max.y < boundB.min.y || boundA.min.y > boundB.max.y ||
    boundA.max.z < boundB.min.z || boundA.min.z > boundB.max.z)
        {
            return null; // 충돌 없음
        }

        //충돌 정보 생성
        ContactInfo contactInfo = new ContactInfo();

        //Impulse 초기화
        contactInfo.normalImpulse = 0f;
        contactInfo.tangentImpulse = 0f;

        //질량 계산
        float invMassA = (colA.rigidBody != null && colA.rigidBody.mass.value > 0f)
            ? 1f / colA.rigidBody.mass.value : 0f;

        float invMassB = (colB.rigidBody != null && colB.rigidBody.mass.value > 0f)
            ? 1f / colB.rigidBody.mass.value : 0f;

        contactInfo.invMassSum = invMassA + invMassB;

        //마찰 계수 계산
        float muA = colA.material != null ? colA.material.friction : 0f;
        float muB = colB.material != null ? colB.material.friction : 0f;

        // 보통 min 또는 평균
        contactInfo.frictionValue = MathUtility.Min(muA, muB);

        // 축별 겹침량
        float overlapX = MathUtility.Min(boundA.max.x-boundB.min.x, boundB.max.x - boundA.min.x);
        float overlapY = MathUtility.Min(boundA.max.y - boundB.min.y, boundB.max.y - boundA.min.y);
        float overlapZ = MathUtility.Min(boundA.max.z - boundB.min.z, boundB.max.z - boundA.min.z);
        
        // penetration = 가장 작은 축의 겹침량
        contactInfo.penetration = MathUtility.Min(overlapX, MathUtility.Min(overlapY, overlapZ));

        // 축 결과에 따라 밀어내는 방향 결정
        if (contactInfo.penetration == overlapX)
            contactInfo.normal = (boundA.center.x < boundB.center.x) ? VectorMathUtils.LeftVector3D() : VectorMathUtils.RightVector3D();
        else if (contactInfo.penetration == overlapY)
            contactInfo.normal = (boundA.center.y < boundB.center.y) ? VectorMathUtils.DownVector3D() : VectorMathUtils.UpVector3D();
        else
            contactInfo.normal = (boundA.center.z < boundB.center.z) ? VectorMathUtils.BackVector3D() : VectorMathUtils.FrontVector3D();

        //마찰 벡터 등록
        Vec3 any = MathUtility.Abs(contactInfo.normal.y) < 0.9f?VectorMathUtils.UpVector3D(): VectorMathUtils.RightVector3D();
        Vec3 crossAny = Vec3.Cross(any,contactInfo.normal);
        contactInfo.tangent = crossAny.Normalized;

        //리지드바디 등록
        contactInfo.rigidA = colA.rigidBody;
        contactInfo.rigidB = colB.rigidBody;

        return contactInfo;
    }
    /// <summary>
    /// 충돌 응답
    /// 충돌하면 튕겨야함
    /// 정적 오브젝트는 움직이면 안됨
    /// </summary>
    /// <param name="colA">물체 A</param>
    /// <param name="colB">물체 B</param>
    /// <param name="contact">충돌 정보</param>
    public static void ResponseCollision3D(CustomCollider3D colA, CustomCollider3D colB, ContactInfo contact)
    {
        if (contact == null || colA==null || colB==null) return;

        //rigidbody가 null일 수 있다.
        var rbA = colA.rigidBody;
        var rbB = colB.rigidBody;

        //RigidBody가 없으면 정적 충돌체 (ex : 벽, 바닥 등)
        bool isAStatic = (rbA == null) || (rbA.mass == null) || (rbA.mass.value <= 0f);
        bool isBStatic = (rbB == null) || (rbB.mass == null) || (rbB.mass.value <= 0f);

        //정적 콜라이더 여부에 따른 속도 설정
        Vec3 velA = isAStatic ? VectorMathUtils.ZeroVector3D() : rbA.velocity;
        Vec3 velB = isBStatic ? VectorMathUtils.ZeroVector3D() : rbB.velocity;

        //노말 벡터
        Vec3 normal = contact.normal;

        //상대 속도 - 물체 B가 A 기준으로 얼마나 빠르게 움직이는지
        Vec3 relativeVelocity = velB-velA;
        float velAlongNormal = Vec3.Dot(relativeVelocity, normal);

        //서로 멀어지면 응답 X, 서로 멀어지는 중
        const float slop = 0.001f;
        //제약식>0 이면 충돌하지 않으니까 충돌 검사의미 없음
        if (velAlongNormal > 0.0f) return;

        //반발 계수, 둘 중 더 작은값이 충돌의 전체 성질을 결정
        //e=0 : 완전 비탄성, e=1 : 완전 탄성
        float eA = (colA.material != null) ? colA.material.bounciness : 0f;
        float eB = (colB.material != null) ? colB.material.bounciness : 0f;
        float restitution = MathUtility.Min(eA, eB);
        restitution = MathUtility.ClampValue(restitution, 0f, 1f);

        //질량 역수
        float invMassA = isAStatic ? 0 : 1 / rbA.mass.value;
        float invMassB = isBStatic ? 0 : 1 / rbB.mass.value;
        float invMassSum = invMassA + invMassB;

        //둘다 정적 물체
        if (invMassSum <= 0f)
        {
            return;
        }

        //서로 충돌중
        if (velAlongNormal < 0f)
        {
            //충격량 크기
            float j = -(1f + restitution) * velAlongNormal;
            j /= invMassSum; //둘다 정적 오브젝트가 아니라서 괜찮음

            //충격 벡터 = 크기 * 밀리는 방향
            Vec3 impulse = normal * j;

            //서로 반대방향으로 밀림
            if (!isAStatic) rbA.velocity -= impulse * invMassA;
            if (!isBStatic) rbB.velocity += impulse * invMassB;
        }

        //완전 침투 보정
        float depth = MathUtility.Max(contact.penetration - slop, 0f);

        if (depth > 0f)
        {
            // full correction: percent=1.0f
            Vec3 correction = normal * depth;

            // inverse mass 비율로 분배
            if (!isAStatic)
                rbA.currentState.position -= correction * (invMassA / invMassSum);

            if (!isBStatic)
                rbB.currentState.position += correction * (invMassB / invMassSum);
        }

        // ------------------ 미세 속도 0으로 조정 ------------------
        const float velocityEps = 0.001f;
        float eps2 = velocityEps * velocityEps;
        if (!isAStatic)
        {
            if (Vec3.Dot(rbA.velocity, rbA.velocity) < eps2) rbA.velocity = VectorMathUtils.ZeroVector3D();
        }
        if (!isBStatic)
        {
            if (Vec3.Dot(rbB.velocity, rbB.velocity) < eps2) rbB.velocity = VectorMathUtils.ZeroVector3D();
        }
    }

    /// <summary>
    /// 지면과 충돌했는지 판정
    /// </summary>
    /// <param name="colA">A물체</param>
    /// <param name="colB">B물체</param>
    /// <param name="contact">충돌 정보</param>
    public static void CheckGround(CustomCollider3D colA,CustomCollider3D colB, ContactInfo contact)
    {
        CustomRigidBody rbA = colA.rigidBody;
        CustomRigidBody rbB = colB.rigidBody;

        //충돌 정보가 없음
        if(contact == null || colA.rigidBody == null) return;

        // Plane 위에 서 있는지: normal.y가 거의 위쪽
        const float groundThreshold = 0.5f; // 0~1, 0.5 이상이면 "바닥"으로 판단
        const float velThreshold = 0.05f;   // 바닥에서 속도가 거의 0이면 접지

        if (contact.normal.y > groundThreshold)//normal이 위쪽인지
        {
            if (rbA.velocity.y <= velThreshold)//속도 조건
            {
                rbA.isGrounded = true;
                rbA.velocity.y = 0f; // 바닥에서 미세 튐 방지
            }
        }

        // B도 Rigidbody 있으면 체크
        if (rbB != null)
        {
            if (contact.normal.y < -0.5f && rbB.velocity.y <= 0f)
            {
                rbB.isGrounded = true;
                rbB.velocity.y = 0f;
            }
        }
    }
    /// <summary>
    /// RayAABB에서 t+Normal만 뽑는 함수
    /// 1. slab test
    /// 2. tMin 계산
    /// 3. tMin을 갱신한 축의 normal 반환
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="dir"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="maxT"></param>
    /// <param name="t"></param>
    /// <param name="normal"></param>
    /// <returns></returns>
    public static bool RayAABB3D(Vec3 origin,Vec3 dir,Vec3 min,Vec3 max,float maxT,out float t,out Vec3 normal)
    {
        float tMin = 0.0f;
        float tMax = maxT;

        Vec3 enterNormal = VectorMathUtils.ZeroVector3D();
        Vec3 exitNormal = VectorMathUtils.ZeroVector3D();

        //각 축별
        for(int axis = 0; axis < 3; axis++)
        {
            float o = origin.Array[axis];
            float d = dir.Array[axis];
            float minA = min.Array[axis];
            float maxA = max.Array[axis];

            //방향 변화 없음
            if (MathUtility.Abs(d) < MathUtility.EPSILON)
            {
                //겹치는 축이 없음
                if (o < minA || o > maxA)
                {
                    t = 0;
                    normal = VectorMathUtils.ZeroVector3D();
                    return false;
                }
                continue;
            }

            float invD = 1.0f / d;
            float t1 = (minA - o) * invD;
            float t2 = (maxA - o) * invD;

            //진입, 탈출점
            float enter = MathUtility.Min(t1, t2);
            float exit = MathUtility.Max(t1, t2);

            Vec3 axisEnterNormal = VectorMathUtils.ZeroVector3D();
            Vec3 axisExitNormal = VectorMathUtils.ZeroVector3D();

            axisEnterNormal.Array[axis] = (t1 < t2) ? -1.0f : 1.0f;
            axisExitNormal.Array[axis] = -axisEnterNormal.Array[axis];

            if (enter > tMin)
            {
                tMin = enter;
                enterNormal = axisEnterNormal;
            }

            if (exit < tMax)
            {
                tMax = exit;
                exitNormal = axisExitNormal;
            }

            if (tMin > tMax)
            {
                t = 0;
                normal = VectorMathUtils.ZeroVector3D();
                return false;
            }
        }
        // inside case 포함
        bool inside = (tMin < 0.0f);
        t = inside ? tMax : tMin;
        normal = inside ? exitNormal : enterNormal;
        return true;
    }
    /// <summary>
    /// RayAABB에서 t+Normal만 뽑는 함수
    /// 1. slab test
    /// 2. tMin 계산
    /// 3. tMin을 갱신한 축의 normal 반환
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="dir"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="maxT"></param>
    /// <param name="t"></param>
    /// <param name="normal"></param>
    /// <returns></returns>
    public static bool RayAABB2D(Vec2 origin, Vec2 dir, Vec2 min, Vec2 max, float maxT, out float t, out Vec2 normal)
    {
        float tMin = 0.0f;
        float tMax = maxT;

        Vec2 enterNormal = VectorMathUtils.ZeroVector2D();
        Vec2 exitNormal = VectorMathUtils.ZeroVector2D();

        //각 축별
        for (int axis = 0; axis < 2; axis++)
        {
            float o = origin.Array[axis];
            float d = dir.Array[axis];
            float minA = min.Array[axis];
            float maxA = max.Array[axis];

            //방향 변화 없음
            if (MathUtility.Abs(d) < MathUtility.EPSILON)
            {
                //겹치는 축이 없음
                if (o < minA || o > maxA)
                {
                    t = 0;
                    normal = VectorMathUtils.ZeroVector2D();
                    return false;
                }
                continue;
            }

            float invD = 1.0f / d;
            float t1 = (minA - o) * invD;
            float t2 = (maxA - o) * invD;

            //진입, 탈출점
            float enter = MathUtility.Min(t1, t2);
            float exit = MathUtility.Max(t1, t2);

            Vec2 axisEnterNormal = VectorMathUtils.ZeroVector2D();
            Vec2 axisExitNormal = VectorMathUtils.ZeroVector2D();

            axisEnterNormal.Array[axis] = (t1 < t2) ? -1.0f : 1.0f;
            axisExitNormal.Array[axis] = -axisEnterNormal.Array[axis];

            if (enter > tMin)
            {
                tMin = enter;
                enterNormal = axisEnterNormal;
            }

            if (exit < tMax)
            {
                tMax = exit;
                exitNormal = axisExitNormal;
            }

            if (tMin > tMax)
            {
                t = 0;
                normal = VectorMathUtils.ZeroVector2D();
                return false;
            }
        }
        // inside case 포함
        bool inside = (tMin < 0.0f);
        t = inside ? tMax : tMin;
        normal = inside ? exitNormal : enterNormal;
        return true;
    }
}
