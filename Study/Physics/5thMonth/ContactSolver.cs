using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public static class ContactSolver
{
    //Contact 매칭 거리
    const float CONTACT_MATCH_DIST = 0.02f;

    /// <summary>
    /// 노말벡터 계산
    /// </summary>
    public static void SolveContactNormal(ContactManifold contact, ContactPoint cp)
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
        float invMassSum = (1.0f / rbA.mass.value) + (1.0f / rbB.mass.value);
        if (invMassSum <= 0f)
        {
            return;
        }

        //delta impulse
        float dJn = -(1f + restitution) * velAlongNormal;
        dJn /= invMassSum;

        //누적+clamp
        float oldJt = cp.normalImpulse;
        cp.normalImpulse = MathUtility.Max(oldJt + dJn, 0f);
        float deltaJn = cp.normalImpulse - oldJt;

        //보정값 적용
        ApplyImpulse(normal * deltaJn, contact, cp);
        
    }

    /// <summary>
    /// 마찰 계산
    /// </summary>
    public static void SolveContactFriction(ContactManifold contact, ContactPoint cp)
    {
        Vec3 relativeVelocity = contact.rigidB.velocity - contact.rigidA.velocity;
        Vec3 vt = relativeVelocity - contact.normal * Vec3.Dot(relativeVelocity, contact.normal);

        //질량 역수의 합
        float invMassSum = (1.0f / contact.rigidA.mass.value) + (1.0f / contact.rigidB.mass.value);

        contact.tangent = vt.Normalized;
        float dJt = Vec3.Dot(vt,contact.tangent) / invMassSum;
        dJt *= (-1);

        //최대 마찰력(마찰계수*수직항력)
        float maxFriction = contact.frictionValue * cp.normalImpulse;

        float oldJt = cp.tangentImpulse;
        cp.tangentImpulse = MathUtility.ClampValue(oldJt + dJt, -maxFriction, maxFriction);

        dJt = cp.tangentImpulse - oldJt;

        ApplyImpulse(contact.tangent * dJt, contact, cp);
    }

    /// <summary>
    /// delta impulse를 받아서 두 rigidbody의 velocity를 수정
    /// </summary>
    /// <param name="delNomalImpulseAmount"></param>
    /// <param name="contact"></param>
    public static void ApplyImpulse(Vec3 deltaImpulse, ContactManifold contact, ContactPoint cp)
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
        float invMassA = manifold.rigidA.isStatic?0.0f: (1.0f / manifold.rigidA.mass.value);
        float invMassB = manifold.rigidB.isStatic ? 0.0f : (1.0f / manifold.rigidB.mass.value);

        //외력 여부
        if (manifold.hasNewContact)
        {
            cp.normalImpulse = 0;
            cp.tangentImpulse = 0;
        }

        //운동량 P
        Vec3 totalImpulse =
             cp.contactNormal* cp.normalImpulse +
            cp.contactTangent * cp.tangentImpulse;

        //선속도
        cp.linearVelocityA -= totalImpulse* invMassA;
        cp.linearVelocityB += totalImpulse* invMassB;
        //가속도
        cp.angularVelocityA -= Vec3.Cross(cp.rotationA, totalImpulse) * (1.0f / cp.IMomentA);
        cp.angularVelocityB += Vec3.Cross(cp.rotationB, totalImpulse) * (1.0f/cp.IMomentB);
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

            //외력이 있음 -> 새 접촉점이 있는가?
            if (manifold.hasNewContact)
            {

            }

            // --- 로컬 좌표 계산 ---
            Vec3 localA = contact.rigidA.WorldToLocal(contact.contactPoint);
            Vec3 localB = contact.rigidB.WorldToLocal(contact.contactPoint);

            // --- ContactPoint(접촉점) 유지 / 추가 ---
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

    /// <summary>
    /// 위치 보정 Solver, 임시 보정량을 계산해줌
    /// </summary>
    /// <param name="manifolds"></param>
    /// <param name="dt"></param>
    public static void SolvePositionConstraints(List<ContactManifold> manifolds, float dt)
    {
        const float slop = 0.01f;//허용 침투량
        const float beta = 0.2f; // Baumgarte 계수, 보정 계수
        int positionIterations = 5;
        for (int i = 0; i < positionIterations; i++)
        {
            foreach (var manifold in manifolds)
            {
                foreach (var cp in manifold.points)
                {
                    //질량 역수의 합
                    float invMassSum = (1.0f / manifold.rigidA.mass.value) + (1.0f / manifold.rigidB.mass.value);

                    //둘다 정적 물체(ex, 땅)
                    if (invMassSum <= 0f) continue;

                    // 단순 Position Projection (금요일 단계)
                    // split impulse 형태는 다음 주에 적용
                    float correction =cp.penetrationDepth - slop;

                    //침투가 안일어남
                    if (correction <= 0f) continue;

                    //얼마나 밀어낼 것인가? 비율
                    float bias = beta * correction;

                    //effectiveMass 계산
                    float invMassA = 1.0f / (manifold.rigidA.mass.value);
                    float invMassB = 1.0f / (manifold.rigidB.mass.value);
                    float rotationValue_A = Vec3.Dot(Vec3.Cross(cp.rotationA, cp.contactNormal), Vec3.Cross(cp.rotationA, cp.contactNormal))* (1 / cp.IMomentA);
                    float rotationValue_B = Vec3.Dot(Vec3.Cross(cp.rotationB, cp.contactNormal), Vec3.Cross(cp.rotationB, cp.contactNormal)) * (1 / cp.IMomentB);
                    float k = invMassA + invMassB + rotationValue_A + rotationValue_B;
                    float effectiveMass = (k>0)? 1.0f / k:0f;

                    float lambda = -bias * effectiveMass;
                    float old = cp.positionalImpulse;
                    cp.positionalImpulse = MathUtility.Max(0, old + lambda);//음수X, 밀어내기만
                    float delta = cp.positionalImpulse - old;

                    //위치에만 적용
                    Vec3 Pos = cp.contactNormal * delta;
                    //Position
                    manifold.rigidA.position -= Pos * invMassA;
                    manifold.rigidB.position += Pos * invMassB;
                    //Orientation
                    float angularCorrectionA = Vec3.Cross(cp.rotationA, Pos).z * (1 / cp.IMomentA);
                    float angularCorrectionB = Vec3.Cross(cp.rotationB, Pos).z * (1 / cp.IMomentB);
                    manifold.rigidA.orientation -= angularCorrectionA;
                    manifold.rigidB.orientation += angularCorrectionB;
                }
            }
        }
    }
    /// <summary>
    /// 위치 충격량을 더해줌
    /// 여기서 위치보정을 한다.
    /// </summary>
    /// <param name="manifold"></param>
    /// <param name="cp"></param>
    /// <param name="impulse"></param>
    public static void ApplyPositionImpulse(ContactManifold manifold, ContactPoint cp, float impulse)
    {
        Vec3 n = manifold.normal;

        float invMassA = 1.0f/manifold.rigidA.mass.value;
        float invMassB = 1.0f / manifold.rigidB.mass.value;
        if (invMassA > 0f)
        {
            manifold.rigidA.position -=
                n * impulse * invMassA;
        }

        if (invMassB > 0f)
        {
            manifold.rigidB.position +=
                n * impulse * invMassB;
        }
    }

    /// <summary>
    /// Contact Solver (GS Iteration)
    /// 1. 접촉점 상대속도 계산
    ///2. 노말 방향 상대속도 추출
    ///3. 제약 위반 여부 판단
    ///4. effectiveMass 계산
    ///5. normal impulse 계산
    ///6. 누적 + clamp
    ///7. 두 물체 속도에 반영
    /// </summary>
    /// <param name="manifolds"></param>
    public static void ContactSolverNormal(ContactPoint cp, ContactManifold manifold)
    {
        //1. 접촉점 상대속도 계산
        //접촉점 속도 계산
        Vec3 velocity_A_Contact = cp.linearVelocityA + Vec3.Cross(cp.angularVelocityA, cp.rotationA);
        Vec3 velocity_B_Contact = cp.linearVelocityB + Vec3.Cross(cp.angularVelocityB, cp.rotationB);
        //상대 속도
        Vec3 velocity_Rel = velocity_B_Contact - velocity_A_Contact;

        //2. 노말 방향 상대 속도
        float velocity_Normal = Vec3.Dot(velocity_Rel, cp.contactNormal);

        //3. 제약 위반 여부 판단
        //이게 음수면 파고든다.(해결해야함, 양수면 이미 분리로 impulse=0)
        if (velocity_Normal >= 0.0f) return;

        //4. effectiveMass 계산
        float invMassA = 1.0f / (manifold.rigidA.mass.value);
        float invMassB = 1.0f / (manifold.rigidB.mass.value);
        float rotationValue_A = Vec3.Dot(Vec3.Cross(cp.rotationA, cp.contactNormal), Vec3.Cross(cp.rotationA, cp.contactNormal)) / cp.IMomentA;
        float rotationValue_B = Vec3.Dot(Vec3.Cross(cp.rotationB, cp.contactNormal), Vec3.Cross(cp.rotationB, cp.contactNormal)) / cp.IMomentB;
        float kNormal = invMassA + invMassB + rotationValue_A + rotationValue_B;
        float effectiveMass = 1.0f / kNormal;

        //5. normal impulse 계산
        float e = cp.restitution;
        if (velocity_Normal < -cp.restitution)
            e = cp.restitution;
        else
            e = 0.0f;
        float addImpulse = -(1.0f + e) * velocity_Normal * effectiveMass;



        //6. 누적 + clamp, normalImpulse는 음수X
        float oldclamp = cp.normalImpulse;
        cp.normalImpulse = MathUtility.Max(0, oldclamp + addImpulse);
        float deltaImpulse = cp.normalImpulse - oldclamp;//Normal Impulse변화량

        //7. 두 물체 속도에 반영(찐 움직임), V'=V+J/m
        Vec3 JImpulse = cp.contactNormal * deltaImpulse;
        //선속도
        cp.linearVelocityA -= JImpulse * invMassA;
        cp.linearVelocityB += JImpulse * invMassB;
        //각속도
        cp.angularVelocityA -= Vec3.Cross(cp.rotationA, JImpulse) * (1.0f / cp.IMomentA);
        cp.angularVelocityB += Vec3.Cross(cp.rotationB, JImpulse) * (1.0f / cp.IMomentB);
    }

    /// <summary>
    /// Contact Tangent Solver (접선 방향)
    /// 1. 접촉점 상대속도 계산
    ///2. 노말 방향 상대속도 추출
    ///3. 정규화
    ///4. effectiveMass 계산
    ///5. tangent impulse 계산
    ///6. 마찰 한계
    ///7. 누적 + clamp
    ///8. 두 물체 속도에 반영
    /// </summary>
    /// <param name="manifolds"></param>
    public static void ContactSolverTangent(ContactPoint cp, ContactManifold manifold)
    {
        //1. 접촉점 상대속도 계산
        //접촉점 속도 계산
        Vec3 velocity_A_Contact = cp.linearVelocityA + Vec3.Cross(cp.angularVelocityA, cp.rotationA);
        Vec3 velocity_B_Contact = cp.linearVelocityB + Vec3.Cross(cp.angularVelocityB, cp.rotationB);
        //상대 속도
        Vec3 velocity_Rel = velocity_B_Contact - velocity_A_Contact;

        //2. 노말 성분 제거
        float velocity_Normal = Vec3.Dot(velocity_Rel, cp.contactNormal);
        Vec3 velocity_Rel_Tangent = velocity_Rel - cp.contactNormal*velocity_Normal;

        //3. 정규화
        Vec3 tangentNormal;
        if (velocity_Rel_Tangent.Square > float.Epsilon)
        {
            tangentNormal = velocity_Rel_Tangent.Normalized;
        }
        else tangentNormal = VectorMathUtils.ZeroVector3D();

        //4. effectiveMass 계산
        float invMassA = 1.0f / (manifold.rigidA.mass.value);
        float invMassB = 1.0f / (manifold.rigidB.mass.value);
        float rotationValue_A = Vec3.Dot(Vec3.Cross(cp.rotationA, tangentNormal), Vec3.Cross(cp.rotationA, tangentNormal)) / cp.IMomentA;
        float rotationValue_B = Vec3.Dot(Vec3.Cross(cp.rotationB, tangentNormal), Vec3.Cross(cp.rotationB, tangentNormal)) / cp.IMomentB;
        float kTangent = invMassA + invMassB + rotationValue_A + rotationValue_B;
        float effectiveMass = 1.0f / kTangent;

        //5. tangent impulse 계산
        float velocityTangent = Vec3.Dot(velocity_Rel, tangentNormal);
        float addImpulse = -velocityTangent * effectiveMass;

        //6. 마찰 한계(마찰계수*수직항력)
        float maxFriction = manifold.frictionValue * cp.normalImpulse;

        //7. 누적 + clamp, normalImpulse는 음수X
        float oldclamp = cp.tangentImpulse;
        cp.tangentImpulse = MathUtility.ClampValue(oldclamp + addImpulse, -maxFriction, maxFriction);
        float deltaImpulse = cp.tangentImpulse - oldclamp;//Normal Impulse변화량

        //8. 두 물체 속도에 반영(찐 움직임), V'=V+J/m
        Vec3 JImpulse = tangentNormal * deltaImpulse;
        //선속도
        cp.linearVelocityA -= JImpulse * invMassA;
        cp.linearVelocityB += JImpulse * invMassB;
        //각속도
        cp.angularVelocityA -= Vec3.Cross(cp.rotationA, JImpulse) * (1.0f / cp.IMomentA);
        cp.angularVelocityB += Vec3.Cross(cp.rotationB, JImpulse) * (1.0f / cp.IMomentB);
    }
}
