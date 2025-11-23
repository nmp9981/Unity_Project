using UnityEngine;

public class CustomCollider3D : MonoBehaviour
{
    public Vector3 size = Vector3.one;

    public Bounds GetBounds()
    {
        return new Bounds(transform.position,size);
    }
    /// <summary>
    /// 중심점 찾기
    /// </summary>
    /// <returns></returns>
    public Vector3 CenterPosition()
    {
        return transform.position;
    }
    /// <summary>
    /// 최소 AABB좌표
    /// </summary>
    /// <returns></returns>
    public Vector3 minPosition()
    {
        float xMin = transform.position.x - size.x * 0.5f;
        float yMin = transform.position.y - size.y * 0.5f;
        float zMin = transform.position.z - size.z * 0.5f;
        return new Vector3(xMin, yMin,zMin);
    }
    /// <summary>
    /// 최대 AABB좌표
    /// </summary>
    /// <returns></returns>
    public Vector3 maxPosition()
    {
        float xMax = transform.position.x + size.x * 0.5f;
        float yMax = transform.position.y + size.y * 0.5f;
        float zMax = transform.position.z + size.z * 0.5f;
        return new Vector3(xMax, yMax,zMax);
    }
}
