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

        //x축 비교
        if (aBox.GetBounds().max.x < bBox.GetBounds().min.x || aBox.GetBounds().min.x > bBox.GetBounds().max.x)
        {
            isCollisionX = false;
        }
        //y축 비교
        if (aBox.GetBounds().max.y < bBox.GetBounds().min.y || aBox.GetBounds().min.y > bBox.GetBounds().max.y)
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

        //x축 비교
        if (aBox.GetBounds().max.x < bBox.GetBounds().min.x || aBox.GetBounds().min.x > bBox.GetBounds().max.x)
        {
            isCollisionX = false;
        }
        //y축 비교
        if (aBox.GetBounds().max.y < bBox.GetBounds().min.y || aBox.GetBounds().min.y > bBox.GetBounds().max.y)
        {
            isCollisionY = false;
        }
        //z축 비교
        if (aBox.GetBounds().max.z < bBox.GetBounds().min.z || aBox.GetBounds().min.z > bBox.GetBounds().max.z)
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
