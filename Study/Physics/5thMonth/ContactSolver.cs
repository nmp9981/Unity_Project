using System.Security.Cryptography;

public static class ContactSolver
{
    /// <summary>
    /// 노말벡터 계산
    /// </summary>
    public static void SolveContactNormal(ContactInfo contact)
    {
        //노말 벡터
        Vec3 normal = contact.normal;

        //상대 속도 - 물체 B가 A 기준으로 얼마나 빠르게 움직이는지
        Vec3 relativeVelocity = velB - velA;
        float velAlongNormal = Vec3.Dot(relativeVelocity, normal);

        //서로 멀어지면 응답 X, 서로 멀어지는 중
        const float slop = 0.001f;
        //제약식>0 이면 충돌하지 않으니까 충돌 검사의미 없음
        if (velAlongNormal > 0.0f) return;

        //반발 계수, 둘 중 더 작은값이 충돌의 전체 성질을 결정
        //e=0 : 완전 비탄성, e=1 : 완전 탄성
        float eA = (colA.material != null) ? colA.material.bounciness : 0f;
        float eB = (colB.material != null) ? colB.material.bounciness : 0f;
        float restitution = MathUtility.Min(eA, eB);
        restitution = MathUtility.ClampValue(restitution, 0f, 1f);

        //질량 역수
        float invMassA = isAStatic ? 0 : 1 / rbA.mass.value;
        float invMassB = isBStatic ? 0 : 1 / rbB.mass.value;
        float invMassSum = invMassA + invMassB;

        //둘다 정적 물체
        if (invMassSum <= 0f)
        {
            return;
        }

        //서로 충돌중
        if (velAlongNormal < 0f)
        {
            //충격량 크기
            float j = -(1f + restitution) * velAlongNormal;
            j /= invMassSum; //둘다 정적 오브젝트가 아니라서 괜찮음

            //충격 벡터 = 크기 * 밀리는 방향
            Vec3 impulse = normal * j;

            //서로 반대방향으로 밀림
            if (!isAStatic) rbA.velocity -= impulse * invMassA;
            if (!isBStatic) rbB.velocity += impulse * invMassB;
        }

    }

    /// <summary>
    /// 마찰 계산
    /// </summary>
    public static void SolveContactFriction(ContactInfo contact)
    {
        Vec3 vt = relativeVelocity - normal * Dot(relativeVelocity, normal);

        float dJt = -Dot(vt, tangent) / c.invMassSum;

        float maxFriction = mu * c.normalImpulse;

        float oldJt = c.tangentImpulse;
        c.tangentImpulse = Clamp(oldJt + dJt, -maxFriction, maxFriction);

        dJt = c.tangentImpulse - oldJt;

        ApplyImpulse(tangent * dJt);
    }
}
