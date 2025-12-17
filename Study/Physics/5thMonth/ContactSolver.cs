using System.Security.Cryptography;

public static class ContactSolver
{
    /// <summary>
    /// 노말벡터 계산
    /// </summary>
    public static void SolveContactNormal(ContactInfo contact)
    {
        if (contact == null || contact.rigidA.col == null || contact.rigidB.col == null) return;

        //rigidbody가 null일 수 있다.
        var rbA = contact.rigidA;
        var rbB = contact.rigidB;

        //노말 벡터
        Vec3 normal = contact.normal;

        //상대 속도 - 물체 B가 A 기준으로 얼마나 빠르게 움직이는지
        Vec3 relativeVelocity = contact.rigidB.velocity - contact.rigidA.velocity;
        float velAlongNormal = Vec3.Dot(relativeVelocity, normal);

        //제약식>0 이면 충돌하지 않으니까 충돌 검사의미 없음, 이미 분리중
        if (velAlongNormal > 0.0f) return;

        //반발 계수, 둘 중 더 작은값이 충돌의 전체 성질을 결정
        //e=0 : 완전 비탄성, e=1 : 완전 탄성
        float eA = (contact.rigidA.col.material != null) ? contact.rigidA.col.material.bounciness : 0f;
        float eB = (contact.rigidB.col.material != null) ? contact.rigidB.col.material.bounciness : 0f;
        float restitution = MathUtility.Min(eA, eB);
        restitution = MathUtility.ClampValue(restitution, 0f, 1f);

        //둘다 정적 물체
        if (contact.invMassSum <= 0f)
        {
            return;
        }

        //delta impulse
        float dJn = -(1f + restitution) * velAlongNormal;
        dJn /= contact.invMassSum;

        //누적+clamp
        float oldJt = contact.normalImpulse;
        contact.normalImpulse = MathUtility.Max(oldJt + dJn, 0f);
        float deltaJn = contact.normalImpulse - oldJt;

        //보정값 적용
        ApplyImpulse(normal * deltaJn, contact);
        
    }

    /// <summary>
    /// 마찰 계산
    /// </summary>
    public static void SolveContactFriction(ContactInfo contact)
    {
        Vec3 relativeVelocity = contact.rigidB.velocity - contact.rigidA.velocity;
        Vec3 vt = relativeVelocity - contact.normal * Vec3.Dot(relativeVelocity, contact.normal);

        float dJt = Vec3.Dot(vt,contact.tangent) / contact.invMassSum;
        dJt *= (-1);

        //최대 마찰력(마찰계수*수직항력)
        float maxFriction = contact.frictionValue * contact.normalImpulse;

        float oldJt = contact.tangentImpulse;
        contact.tangentImpulse = MathUtility.ClampValue(oldJt + dJt, -maxFriction, maxFriction);

        dJt = contact.tangentImpulse - oldJt;

        ApplyImpulse(contact.tangent * dJt, contact);
    }

    /// <summary>
    /// delta impulse를 받아서 두 rigidbody의 velocity를 수정
    /// </summary>
    /// <param name="delNomalImpulseAmount"></param>
    /// <param name="contact"></param>
    public static void ApplyImpulse(Vec3 deltaImpulse, ContactInfo contact)
    {
        var rigidA = contact.rigidA;    
        var rigidB = contact.rigidB;

        //RigidBody가 없으면 정적 충돌체 (ex : 벽, 바닥 등)
        bool isAStatic = (rigidA == null) || (rigidA.mass == null) || (rigidA.mass.value <= 0f);
        bool isBStatic = (rigidB == null) || (rigidB.mass == null) || (rigidB.mass.value <= 0f);

        //질량 역수
        float invMassA = isAStatic ? 0 : 1 / rigidA.mass.value;
        float invMassB = isBStatic ? 0 : 1 / rigidB.mass.value;

        if (invMassA > 0f)
        {
            rigidA.velocity -= deltaImpulse * invMassA;
        }
        if (invMassB > 0f)
        {
            rigidB.velocity += deltaImpulse * invMassB;
        }
    }
}
