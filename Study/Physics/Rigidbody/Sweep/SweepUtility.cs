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
/// 공용 수식/헬퍼
/// </summary>
public class SweepUtility
{
    
}
