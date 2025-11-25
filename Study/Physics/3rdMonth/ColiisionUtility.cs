using System;
using System.Numerics;

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
    public static bool IsCollisionCircle(Vector2 center1, float r1, Vector2 center2, float r2)
    {
        //벡터 차
        Vector2 diff = center1- center2;

        //반지름 합
        float sumRadius = r2 + r1;
        float sumRadius2 = sumRadius * sumRadius;

        //중심간 거리 제곱
        float diffCenter2 = diff.X*diff.X+diff.Y*diff.Y;

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
    public static bool IsCollisionCircle(Vector3 center1, float r1, Vector3 center2, float r2)
    {
        //벡터 차
        Vector3 diff = center1 - center2;

        //반지름 합
        float sumRadius = r2 + r1;
        float sumRadius2 = sumRadius * sumRadius;

        //중심간 거리 제곱
        float diffCenter2 = diff.X*diff.X + diff.Y*diff.Y+diff.Z*diff.Z;

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
    public static bool IsColliderCircle_AABB(CustomCollider2D box, Vector2 circleCenter, float radius)
    {
        //원과 가장 가까운점 찾기
        var boxMinPos = box.minPosition();
        var boxMaxPos = box.maxPosition();

        float closestX = MathUtility.ClampValue(circleCenter.X, boxMinPos.x, boxMaxPos.x);
        float closestY = MathUtility.ClampValue(circleCenter.Y, boxMinPos.y, boxMaxPos.y);

        //가장 가까운 점과 원의 중심간 거리 비교
        float dist2 = (closestX - circleCenter.X) * (closestX - circleCenter.X) 
            + (closestY - circleCenter.Y) * (closestY - circleCenter.Y);

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
    public static bool IsColliderSphere_AABB(CustomCollider3D box, Vector3 sphereCenter, float radius)
    {
        //원과 가장 가까운점 찾기
        var boxMinPos = box.minPosition();
        var boxMaxPos = box.maxPosition();
        
        float closestX = MathUtility.ClampValue(sphereCenter.X, boxMinPos.x, boxMaxPos.x);
        float closestY = MathUtility.ClampValue(sphereCenter.Y, boxMinPos.y, boxMaxPos.y);
        float closestZ = MathUtility.ClampValue(sphereCenter.Z, boxMinPos.z, boxMaxPos.z);

        //가장 가까운 점과 원의 중심간 거리 비교
        float dist2 = (closestX - sphereCenter.X) * (closestX - sphereCenter.X)
            + (closestY - sphereCenter.Y) * (closestY - sphereCenter.Y)
            + (closestZ - sphereCenter.Z) * (closestZ - sphereCenter.Z);

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
    //충돌 응답
    public static void ResponseCollision3D(CustomCollider3D colA, CustomCollider3D colB, ContactInfo contact)
    {

    }

}
