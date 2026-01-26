using System;
using UnityEngine;

public abstract class CustomCollider2D : MonoBehaviour
{
    public CustomRigidbody2D rigidBody;
    public CustomPhysicsMaterial material;
    public Vec2 size = VectorMathUtils.OneVector2D();

    public int layer;//레이어 번호

    [Header("Material")]
    public CustomPhysicsMaterial physicMaterial;//재질

    private void Awake()
    {
        size = new Vec2(transform.localScale.x, transform.localScale.y);
    }
    
    public Bounds GetBounds()
    {
        Vec2 centerPosition = new Vec2(transform.position.x, transform.position.y);
        Vector3 boundCenterPosition = new Vector3(centerPosition.x, centerPosition.y, 0);
        Vector3 size3 = new Vector3(size.x, size.y,0);
        return new Bounds(boundCenterPosition, size3);
    }
    public Vec2 CenterPosition()
    {
        return new Vec2(transform.position.x,transform.position.y);
    }
    public Vec2 minPosition()
    {
        float xMin = transform.position.x - size.x * 0.5f;
        float yMin = transform.position.y - size.y * 0.5f;
        return new Vec2(xMin, yMin);
    }
    public Vec2 maxPosition()
    {
        float xMax = transform.position.x + size.x * 0.5f;
        float yMax = transform.position.y + size.y * 0.5f;
        return new Vec2(xMax, yMax);
    }

    /// <summary>
    /// Narraw Coliider 판정
    /// 대략적인 충돌 범위내에 들어가는지 판정
    /// </summary>
    /// <param name="ray"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="maxT"></param>
    /// <param name="tEnter"></param>
    /// <returns></returns>
    public bool RaycastAABB(Ray2D ray, Vec2 min, Vec2 max, float maxT, out float tEnter)
    {
        tEnter = 0.0f;
        float tExit = maxT;

        for (int i = 0; i < 2; i++)
        {
            float origin = ray.origin.Array[i];
            float dir = ray.dir.Array[i];

            if (Math.Abs(dir) < MathUtility.EPSILON)
            {
                // Ray가 slab과 평행
                if (origin < min.Array[i] || origin > max.Array[i])
                    return false;
            }
            else
            {
                float invD = 1.0f / dir;
                float t1 = (min.Array[i] - origin) * invD;
                float t2 = (max.Array[i] - origin) * invD;

                if (t1 > t2)
                {
                    float tmp = t1;
                    t1 = t2;
                    t2 = tmp;
                }

                tEnter = Math.Max(tEnter, t1);
                tExit = Math.Min(tExit, t2);

                if (tEnter > tExit)
                    return false;
            }
        }
        return true;
    }
    public abstract bool RayCast2D(Ray2D ray, float maxT, out RaycastHit2D hit);
}
