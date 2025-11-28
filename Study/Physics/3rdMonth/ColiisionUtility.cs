using System;
using System.Numerics;
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
        if (contact == null) return;

        //rigidbody가 null일 수 있다.
        var rbA = colA.rigidBody;
        var rbB = colB.rigidBody;

        //RigidBody가 없으면 정적 충돌체 (ex : 벽, 바닥 등)
        bool isAStatic = (rbA == null) ? true : false;
        bool isBStatic = (rbB == null) ? true : false;

        //정적 콜라이더 여부에 따른 속도 설정
        Vec3 velA = isAStatic ? VectorMathUtils.ZeroVector3D() : rbA.velocity;
        Vec3 velB = isBStatic ? VectorMathUtils.ZeroVector3D() : rbB.velocity;

        //상대 속도 - 물체 B가 A 기준으로 얼마나 빠르게 움직이는지
        Vec3 relativeVelocity = velB-velA;
        float velAlongNormal = Vec3.Dot(relativeVelocity, contact.normal);

        //서로 멀어지면 응답 X, 서로 멀어지는 중
        if (velAlongNormal > 0.0f) return;

        //반발 계수, 둘 중 더 작은값이 충돌의 전체 성질을 결정
        //e=0 : 완전 비탄성, e=1 : 완전 탄성
        
        float eA = colA.material.bounciness;
        float eB = colB.material.bounciness;
        float e = MathUtility.Min(eA, eB);

        //질량 역수
        float invMassA = isAStatic ? 0 : 1 / rbA.mass.value;
        float invMassB = isBStatic ? 0 : 1 / rbB.mass.value;
        
        //충격량 크기
        float j = -(1 + e) * velAlongNormal;
        j /= (invMassA+invMassB);//둘다 정적 오브젝트가 아니라서 괜찮음

        //충격 벡터 = 크기 * 밀리는 방향
        Vec3 impulse = contact.normal*j;

        //서로 반대방향으로 밀림
        if(!isAStatic) rbA.velocity -= impulse / rbA.mass.value;
        if(!isBStatic) rbB.velocity += impulse / rbB.mass.value;

        //Jitter 방지, 위치 보정
        const float percent = 0.4f;
        const float slop = 0.001f;
        float correctionMag = Math.Max(contact.penetration - slop, 0f) * percent;
        Vec3 correction =  contact.normal*correctionMag;
        if (!isAStatic) rbA.currentState.position -= correction * invMassA;
        if (!isBStatic) rbB.currentState.position += correction * invMassB;
    }
}
