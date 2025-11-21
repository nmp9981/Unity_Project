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
}
