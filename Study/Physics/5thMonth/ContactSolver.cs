using System.Collections.Generic;
using System.Net.Mail;
using UnityEditor.SceneManagement;

public static class ContactSolver
{
    //Contact 매칭 거리
    const float CONTACT_MATCH_DIST = 0.02f;

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

    /// <summary>
    /// 지난 프레임에 이미 구해놓은 impulse를 이번 프레임 Solver 시작 전에 미리 적용
    /// 이미 계산된 값을 적용
    /// </summary>
    /// <param name="manifold"></param>
    /// <param name="cp"></param>
    public static void WarmStart(ContactManifold manifold, ContactPoint cp)
    {
        Vec3 totalImpulse =
            manifold.normal * cp.normalImpulse +
            manifold.tangent * cp.tangentImpulse;

        ApplyImpulse(totalImpulse, manifold, cp);
    }

    /// <summary>
    /// 지난 프레임 manifold + 이번 프레임 contact 후보들을 기반으로
    /// 새로운 manifold들을 구성한다 (Contact Persistence)
    /// </summary>
    public static void UpdateManifolds(
        List<ContactManifold> previousManifolds,
        List<ContactInfo> contactList,
        out List<ContactManifold> manifolds)
    {
        manifolds = new List<ContactManifold>();

        // 1. 이번 프레임 contact들을 manifold 단위로 묶는다
        foreach (var contact in contactList)
        {
            // --- Manifold 매칭 ---
            var manifold = FindManifold(
                contact.rigidA,
                contact.rigidB,
                manifolds);

            if (manifold == null)
            {
                manifold = new ContactManifold
                {
                    rigidA = contact.rigidA,
                    rigidB = contact.rigidB,
                    frictionValue = contact.frictionValue
                };
                manifolds.Add(manifold);
            }
            //매 프레임 normal은 최신값으로
            manifold.normal = contact.normal;
            manifold.frictionValue = contact.frictionValue;

            // --- 로컬 좌표 계산 ---
            Vec3 localA = contact.rigidA.WorldToLocal(contact.contactPoint);
            Vec3 localB = contact.rigidB.WorldToLocal(contact.contactPoint);

            // --- ContactPoint 유지 / 추가 ---
            UpdateContactPoint(
                manifold,
                localA,
                localB,
                previousManifolds);
        }

        // 2. contact 없는 오래된 cp 제거 (화요일 마무리 단계)
        foreach (var manifold in manifolds)
        {
            RemoveStaleContacts(manifold);
        }
    }

    /// <summary>
    /// Manifold 업데이트
    /// </summary>
    /// <param name="manifold"></param>
    /// <param name="newLocalA">새로운 A위치</param>
    /// <param name="newLocalB">새로운 B위치</param>
    public static void UpdateContactPoint(ContactManifold manifold,Vec3 newLocalA,Vec3 newLocalB, List<ContactManifold> previousManifolds)
    {
        // 지난 프레임 같은 manifold 찾기
        var oldManifold = FindManifold(
            manifold.rigidA,
            manifold.rigidB,
            previousManifolds);

        ContactPoint oldCp = null;

        if (oldManifold != null)
        {
            oldCp = FindMatchingContact(oldManifold, newLocalA);
        }

        if (oldCp != null)
        {
            // ✅ 유지 contact (impulse 계승)
            manifold.points.Add(new ContactPoint
            {
                localPointA = newLocalA,
                localPointB = newLocalB,
                normalImpulse = oldCp.normalImpulse,
                tangentImpulse = oldCp.tangentImpulse
            });
        }
        else
        {
            // ✅ 새 contact
            manifold.points.Add(new ContactPoint
            {
                localPointA = newLocalA,
                localPointB = newLocalB,
                normalImpulse = 0f,
                tangentImpulse = 0f
            });
        }
    }

    /// <summary>
    /// Manifold 찾기
    /// </summary>
    public static ContactManifold FindManifold(CustomRigidBody rigidA, CustomRigidBody rigidB, List<ContactManifold> manifolds)
    {
        foreach (var m in manifolds)
        {
            //두 물체가 같으면 같은 Manifold이다.
            if ((m.rigidA == rigidA && m.rigidB == rigidB) ||
                (m.rigidA == rigidB && m.rigidB == rigidA))
                return m;
        }
        return null;
    }

    /// <summary>
    /// contact point 매칭
    /// </summary>
    /// <param name="manifold"></param>
    /// <param name="newLocalA"></param>
    /// <returns></returns>
    public static ContactPoint FindMatchingContact(
    ContactManifold manifold,
    Vec3 newLocalA)
    {
        foreach (var cp in manifold.points)
        {
            //거리 차이가 거의 안난다면 같은 위치
            if ((cp.localPointA - newLocalA).Square
                < CONTACT_MATCH_DIST * CONTACT_MATCH_DIST)
                return cp;
        }
        return null;
    }
}
