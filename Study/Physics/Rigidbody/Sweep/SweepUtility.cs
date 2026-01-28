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
    Vec3 a;//Start Point
    Vec3 b;//End Point
    float r;//Raduis
}
/// <summary>
/// 공용 수식/헬퍼
/// </summary>
public class SweepUtility
{
    
}
