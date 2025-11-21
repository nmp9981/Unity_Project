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
}
