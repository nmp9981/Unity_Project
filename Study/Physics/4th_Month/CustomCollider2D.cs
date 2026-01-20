using UnityEngine;

public abstract class CustomCollider2D : MonoBehaviour
{
    public CustomRigidbody2D rigidBody;
    public CustomPhysicsMaterial material;
    public Vec2 size = VectorMathUtils.OneVector2D();

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
    public abstract bool RayCast2D(Ray3D ray, float maxT, out RaycastHit3D hit);
}
