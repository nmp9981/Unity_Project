using UnityEngine;

public class ColiisionUtility
{
    public static bool CheckAABBBox(CustomCollider3D aBox, CustomCollider3D bBox)
    {
        return aBox.GetBounds().Intersects(bBox.GetBounds());
    }

    /// <summary>
    /// AABB충돌 여부 - 2D
    /// </summary>
    /// <param name="aBox">오브젝트 A 박스</param>
    /// <param name="bBox">오브젝트 B 박스</param>
    /// <returns></returns>
    public static bool IsCollisionAABB2D(CustomCollider2D aBox, CustomCollider2D bBox)
    {
        //각 축별 충돌 여부
        bool isCollisionX = true;
        bool isCollisionY = true;

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

        //모두 겹치면 충돌
        if (isCollisionX && isCollisionY)
        {
            return true;   
        }
        return false;
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
    /// 원끼리의 충돌
    /// </summary>
    /// <param name="center1">원1의 중심 좌표</param>
    /// <param name="r1">원1의 반지름</param>
    /// <param name="center2">원2의 중심 좌표</param>
    /// <param name="r2">원2의 반지름</param>
    /// <returns></returns>
    public static bool IsCollisionCircle(Vec2 center1, float r1, Vec2 center2, float r2)
    {
        //벡터 차
        Vec2 diff = center1- center2;

        //반지름 합
        float sumRadius = r2 + r1;
        float sumRadius2 = sumRadius * sumRadius;

        //중심간 거리 제곱
        float diffCenter2 = diff.x*diff.x+diff.y*diff.y;

        //충돌 검사
        if (diffCenter2 < sumRadius2) return true;
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
    /// AABB박스와 원과의 충돌 체크
    /// </summary>
    /// <param name="box">AABB박스</param>
    /// <param name="circleCenter">원의 중심</param>
    /// <param name="radius">원의 반지름</param>
    /// <returns></returns>
    public static bool IsColliderCircle_AABB(CustomCollider2D box, Vec2 circleCenter, float radius)
    {
        //원과 가장 가까운점 찾기
        var boxMinPos = box.minPosition();
        var boxMaxPos = box.maxPosition();

        float closestX = MathUtility.ClampValue(circleCenter.x, boxMinPos.x, boxMaxPos.x);
        float closestY = MathUtility.ClampValue(circleCenter.y, boxMinPos.y, boxMaxPos.y);

        //가장 가까운 점과 원의 중심간 거리 비교
        float dist2 = (closestX - circleCenter.x) * (closestX - circleCenter.x) 
            + (closestY - circleCenter.y) * (closestY - circleCenter.y);

        //반지름보다 더 작으면 충돌
        return dist2 < radius*radius;
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
    public static void ResolvePlaneCollision2D(CustomRigidbody2D aabb, Vec2 planeNormal, Vec2 planePoint)
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

        if (penetration > 0f)
        {
            // 위치 보정: Plane 위로 이동
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

        ContactInfo contactInfo = new ContactInfo();

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
        if (velAlongNormal > 0.0f && contact.penetration <= slop) return;

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

        // ------------------ Positional correction ------------------
        // Baumgarte-style correction with slop and percent
        //위치 보정
        const float percent = 0.2f; // how much penetration to correct this step (0..1)
        float penetrationToFix = MathUtility.Max(contact.penetration - slop, 0f);

        // 첫 프레임 penetration이 너무 크면 제한
        const float maxInitialPenetration = 0.1f;
        penetrationToFix = MathUtility.Min(penetrationToFix, maxInitialPenetration);

        if (penetrationToFix > 0f)//겹침이 있을 경우 위치 보정이 필요함
        {
            float D = penetrationToFix * percent;
            Vec3 correction = normal * D;

            // distribute correction by inverse-mass ratio
            if (!isAStatic) rbA.currentState.position -= correction * (invMassA / invMassSum);
            if (!isBStatic) rbB.currentState.position += correction * (invMassB / invMassSum);
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
}
