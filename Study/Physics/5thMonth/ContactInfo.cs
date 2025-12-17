/// <summary>
/// 충돌 정보
/// </summary>
public class ContactInfo
{
    //두 충돌체
    public CustomRigidBody rigidA;
    public CustomRigidBody rigidB;

    public bool isCollide;//충돌 여부
    public Vec3 normal;//어느 방향으로 밀어낼지
    public float penetration;//겹침량
    public Vec3 tangent;//탄젠트 벡터
    public Vec3 contactPoint;// 회전

    public float normalImpulse;   // 누적 Jn
    public float tangentImpulse;  // 누적 Jt

    public float invMassSum;//두 물체의 질량 역수의 합
    public float frictionValue;//마찰 계수
}
