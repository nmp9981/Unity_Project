using System;
using UnityEngine;

public abstract class CustomCollider3D : MonoBehaviour
{
    public CustomRigidBody rigidBody;
    public CustomPhysicsMaterial material;
    public Vec3 size;

    public int layer;//레이어 번호

    public Transform3D transform3D;//물리 계산용 캐시

    [Header("Material")]
    public CustomPhysicsMaterial physicMaterial;//재질

    protected virtual void Awake()
    {
        transform3D = new Transform3D();
        SyncTransform();
        size = new Vec3(this.transform.localScale.x, this.transform.localScale.y, this.transform.localScale.z);
    }
    /// <summary>
    /// Transform 3D 세팅
    /// </summary>
    public void SyncTransform()
    {
        // Unity → Physics
        transform3D.position = new Vec3(
            transform.position.x,
            transform.position.y,
            transform.position.z
        );

        Vec3 quaternionVec = new Vec3(transform.rotation.x, transform.rotation.y, transform.rotation.z);
        CustomQuaternion q = new CustomQuaternion(transform.rotation.w, quaternionVec);
        transform3D.rotation = MatrixUtility.FromQuaternion(q);

        transform3D.scale = new Vec3(
            transform.localScale.x,
            transform.localScale.y,
            transform.localScale.z
        );

        transform3D.UpdateMatrices();
    }
    public Bounds GetBounds()
    {
        Vec3 center = transform3D.position;
        Vec3 extents = size * 0.5f;

        return new Bounds(
            new Vector3(center.x, center.y, center.z),
            new Vector3(
                extents.x * 2,
                extents.y * 2,
                extents.z * 2
            )
        );
    }
    /// <summary>
    /// 중심점 찾기
    /// </summary>
    /// <returns></returns>
    public Vec3 CenterPosition()
    {
        return new Vec3(transform.position.x, transform.position.y, transform.position.z);
    }
    /// <summary>
    /// 최소 AABB좌표
    /// </summary>
    /// <returns></returns>
    public Vec3 minPosition()
    {
        float xMin = transform.position.x - size.x * 0.5f;
        float yMin = transform.position.y - size.y * 0.5f;
        float zMin = transform.position.z - size.z * 0.5f;
        return new Vec3(xMin, yMin,zMin);
    }
    /// <summary>
    /// 최대 AABB좌표
    /// </summary>
    /// <returns></returns>
    public Vec3 maxPosition()
    {
        float xMax = transform.position.x + size.x * 0.5f;
        float yMax = transform.position.y + size.y * 0.5f;
        float zMax = transform.position.z + size.z * 0.5f;
        return new Vec3(xMax, yMax,zMax);
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
    public bool RaycastAABB(Ray3D ray,Vec3 min,Vec3 max,float maxT,out float tEnter, out Vec3 normal)
    {
        tEnter = 0.0f;
        float tExit = maxT;
        normal = VectorMathUtils.ZeroVector3D();

        for (int i = 0; i < 3; i++)
        {
            float origin = ray.origin.Array[i];
            float dir = ray.dir.Array[i];

            if (MathUtility.Abs(dir) < MathUtility.EPSILON)
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

                //진입점, 탈출점
                float enter = MathUtility.Min(t1, t2);
                float exit = MathUtility.Max(t1, t2);

                // 들어오는 면 normal
                Vec3 axisNormal = VectorMathUtils.ZeroVector3D();
                axisNormal.Array[i] = (t1 < t2) ? -1.0f : 1.0f;

                if (enter > tEnter)
                {
                    tEnter = enter;
                    normal = axisNormal;
                }

                tExit = MathUtility.Min(tExit, exit);

                //충돌 X
                if (tEnter > tExit)
                    return false;
            }
        }
        return true;
    }

    public abstract bool RayCast(Ray3D ray, float maxT, out RaycastHit3D hit);

}
