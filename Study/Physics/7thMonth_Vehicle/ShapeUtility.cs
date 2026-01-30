/// <summary>
/// 평면 정보
/// </summary>
public struct Plane
{
    public Vec3 normal;//평면의 노멀 벡터
    public float d;//Plane으로부터의 거리
}
/// <summary>
/// Capsule Info
/// </summary>
public struct Capsule
{
    public Vec3 a;//Start Point
    public Vec3 b;//End Point
    public float r;//Raduis
}
/// <summary>
/// Shpere Info
/// </summary>
public struct Sphere
{
    public Vec3 center;
    public float radius;
}
/// <summary>
/// BoxInfo
/// </summary>
public struct Box
{
    public Vec3 center;
    public Vec3 halfExtent;
    public Mat3 rotation;
}
public class ShapeUtility : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
