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
    public SphereCollider3D collider;
}
/// <summary>
/// BoxInfo
/// </summary>
public struct Box
{
    public Vec3 center;
    public Vec3 halfExtent;
    public Mat3 rotation;
    public BoxCollider3D collider;

    // World → Local (Point)
    public Vec3 WorldToLocalPoint(Vec3 worldPoint)
    {
        // 중심 기준으로 이동
        Vec3 p = worldPoint - center;

        // 회전 역변환
        CustomQuaternion rotQuat = QuaternionUtility.Mat3ToQuaternion(rotation);
        CustomQuaternion inv = QuaternionUtility.Inverse(rotQuat);
        return inv * p;
    }

    // World → Local (Vector)
    public Vec3 WorldToLocalVector(Vec3 worldVector)
    {
        // 벡터는 위치 개념이 없으므로 translation 제거
        CustomQuaternion rotQuat = QuaternionUtility.Mat3ToQuaternion(rotation);
        CustomQuaternion inv = QuaternionUtility.Inverse(rotQuat);
        return inv * worldVector;
    }

    // Local → World (Point)
    public Vec3 LocalToWorldPoint(Vec3 localPoint)
    {
        return rotation * localPoint + center;
    }

    // Local → World (Vector)
    public Vec3 LocalToWorldVector(Vec3 localVector)
    {
        return rotation * localVector;
    }
}
public class ShapeUtility
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
