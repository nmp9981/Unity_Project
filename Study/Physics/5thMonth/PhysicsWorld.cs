using System.Collections.Generic;
using System.Drawing;
using System.Security.Cryptography;
using UnityEngine;
using static UnityEditor.ShaderData;
using static UnityEngine.GraphicsBuffer;

public class PhysicsWorld : MonoBehaviour
{
    [SerializeField]
    List<CustomRigidBody> rigidBodies3D = new List<CustomRigidBody>();
    [SerializeField]
    List<CustomCollider3D> colliders3D = new List<CustomCollider3D>();

    //물리 작동 여부
    public bool physicsPaused = false;

    //적분 반복 횟수
    public int solverIterations = 10;

    //지면 판정
    public float groundThreshold;

    //충돌 정보 모음
    public List<ContactInfo> contactList = new List<ContactInfo>();

    private void Update()
    {
        // P 버튼으로 물리만 멈춤/재시작
        if (Input.GetKeyDown(KeyCode.P))
        {
            physicsPaused = !physicsPaused;
        }

        RenderStep(Time.deltaTime);
    }

    public void Create_ColliderAndRigid3D()
    {
        rigidBodies3D.AddRange(FindObjectsByType<CustomRigidBody>(FindObjectsSortMode.None));
        colliders3D.AddRange(FindObjectsByType<CustomCollider3D>(FindObjectsSortMode.None));
    }

    /// <summary>
    /// 물리 스텝
    /// Physics Step → Collision → Resolve → Commit → Render Step(Interpolation)
    /// </summary>
    /// <param name="dt"></param>
    public void PhysicsStep(float dt)
    {
        //모든 물리 step
        foreach (var rb in rigidBodies3D)
        {
            rb.isGrounded = false;//중력 체크 초기화
            rb.PredictState(dt);
        }

        //Contact 생성
        contactList.Clear();

        //충돌 체크
        Handle3DCollisions();// 내부에서 PositionalCorrection만 수행

        //충돌 뒤 제약 조건 체크
        SolveOtherConstraints();

        //Ground 판정
        foreach (var cont in contactList)
        {
            AccumulateGroundContact(cont.rigidA, cont.rigidB, cont);
        }

        //모든 물리 step
        foreach (var rb in rigidBodies3D)
        {
            //커밋 단계
            rb.Commit(dt);
        }
    }

    /// <summary>
    /// 렌더링 스텝
    /// 렌더링은 물리 상태를 읽기만 한다.
    /// </summary>
    /// <param name="dt"></param>
    void RenderStep(float dt)
    {
        PositionUpdateState();
    }

    /// <summary>
    /// 충돌 체크
    /// </summary>
    void Handle3DCollisions()
    {
        for (int i = 0; i < colliders3D.Count; i++)
        {
            for (int j = i + 1; j < colliders3D.Count; j++)
            {
                var collA = colliders3D[i];
                var collB = colliders3D[j];

                //충돌 여부 판정
                if (!ColiisionUtility.IsCollisionAABB3D(collA, collB)) continue;

                //충돌 정보
                ContactInfo contactInfo = ColiisionUtility.GetContactAABB3D(collA, collB);

                contactList.Add(contactInfo);
                //충돌 응답
                //ColiisionUtility.ResponseCollision3D(collA, collB, contactInfo);
            }
        }
    }
    /// <summary>
    /// 위치 상태 업데이트
    /// </summary>
    void PositionUpdateState()
    {
        float alpha = (float)(Time.timeAsDouble - Time.fixedTimeAsDouble) / Time.fixedDeltaTime;
        float alpha01 = MathUtility.ClampValue(alpha, 0, 1);//보간 값

        foreach (var rb in rigidBodies3D)
        {
            Vec3 lerped = Vec3.Lerp(rb.previousState.position, rb.currentState.position, alpha01);

            rb.transform.position = new Vector3(lerped.x, lerped.y, lerped.z);
        }
    }
    /// <summary>
    /// 위치 보정
    /// </summary>
    /// <param name="rigidA">물체 A</param>
    /// <param name="rigidB">물체 B</param>
    /// <param name="contact">충돌 정보</param>
    void PositionalCorrection(CustomRigidBody rigidA, CustomRigidBody rigidB, ContactInfo contact)
    {
        if (contact == null) return;

        float correctionPercent = 0.5f; // 양쪽 50%씩
        float slop = 0.01f;             // 안정화 값 (penetration이 너무 작으면 무시)

        float correction = Mathf.Max(contact.penetration - slop, 0f);
        if (correctionPercent <= 0f) return;

        float beta = 0.2f;//전체중에 이정도만 고치자
        Vec3 correctionVec = contact.normal * correction*correctionPercent*beta;

        // Case 1: 둘 다 rigidBody 있는 경우 → 50% 씩
        if (rigidA != null && rigidB != null)
        {
            // 분배: inverse mass 비율을 쓰려면 rigid에서 mass 접근 필요
            float invA = (rigidA.mass != null && rigidA.mass.value > 0f) ? 1f / rigidA.mass.value : 0f;
            float invB = (rigidB.mass != null && rigidB.mass.value > 0f) ? 1f / rigidB.mass.value : 0f;
            float invSum = invA + invB;
            if (invSum <= 0f) return;
            rigidA.predictedPosition -= correctionVec * (invA / invSum);
            rigidB.predictedPosition += correctionVec * (invB / invSum);
            return;
        }

        // Case 2: A만 rigidBody 있는 경우 → A가 100% 이동
        if (rigidA != null && rigidB == null)
        {
            rigidA.predictedPosition -= correctionVec;  // 100%
            return;
        }

        // Case 3: B만 rigidBody 있는 경우 → B가 100% 이동
        if (rigidA == null && rigidB != null)
        {
            rigidB.predictedPosition += correctionVec;  // 100%
            return;
        }

        // Case 4: 둘다 없으면 아무것도 안함
    }

    /// <summary>
    /// 제약 조건 처리
    /// </summary>
    void SolveOtherConstraints()
    {
        for (int i = 0; i < solverIterations; ++i)
        {
            foreach(var contactInfo in contactList)
            {
                ContactSolver.SolveContactNormal(contactInfo);//노멀 계산
                ContactSolver.SolveContactFriction(contactInfo);//마찰 계산
                PositionalCorrection(contactInfo.rigidA, contactInfo.rigidB, contactInfo);//위치 보정
            }
        }
    }
    /// <summary>
    /// Ground 증거 수집
    /// </summary>
    /// <param name="rbA"></param>
    /// <param name="rbB"></param>
    /// <param name="contact"></param>
    void AccumulateGroundContact(CustomRigidBody rbA,
                             CustomRigidBody rbB,
                             ContactInfo contact)
    {
        if (contact.normal.y > groundThreshold)
        {
            rbA.hasGroundContact = true;

            if (contact.normal.y > rbA.groundNormal.y)
                rbA.groundNormal = contact.normal;
        }

        if (contact.normal.y < -groundThreshold)
        {
            rbB.hasGroundContact = true;

            if (-contact.normal.y > rbB.groundNormal.y)
                rbB.groundNormal = contact.normal*(-1);
        }
    }
}
