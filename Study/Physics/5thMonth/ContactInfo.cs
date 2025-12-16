/// <summary>
/// 충돌 정보
/// </summary>
public class ContactInfo
{
    public bool isCollide;//충돌 여부
    public Vec3 normal;//어느 방향으로 밀어낼지
    public float penetration;//겹침량
    public Vec3 tangent;//탄젠트 벡터

    public float normalImpulse;   // 누적 Jn
    public float tangentImpulse;  // 누적 Jt

    public float invMassSum;
}
