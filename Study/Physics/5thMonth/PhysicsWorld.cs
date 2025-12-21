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
    public List<ContactInfo> contactList = new List<ContactInfo>();//이건 사용하지 않음
    
    //Manifold 리스트, 이게 충돌 정보
    public List<ContactManifold> manifolds;
    
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
        // 1. 다음 상태 예측
        foreach (var rb in rigidBodies3D)
        {
            rb.isGrounded = false;//중력 체크 초기화
            rb.PredictState(dt);
        }

        contactList.Clear();

        Handle3DCollisionDetection();   // contactList 생성, 내부에서 PositionalCorrection만 수행

        // 3. Contact Persistence (화요일 핵심)
        var previousManifolds = manifolds;

        ContactSolver.UpdateManifolds(
            previousManifolds,
            contactList,
            out manifolds);
        //지난 프레임에 이미 구해놓은 impulse를 이번 프레임 Solver 시작 전에 미리 적용
        foreach (var manifold in manifolds)
        {
            foreach (var cp in manifold.points)
            {
                //지난 프레임에 수렴한 해를 이번 프레임의 초기값으로 재사용
                ContactSolver.WarmStart(manifold, cp);
            }
        }

        // 5. Velocity Solver, 속도 -> 위치
        SolveVelocityConstraints(manifolds, dt);

        // 6. Contact Solver (GS Iteration)
        ContactSolver.SolvePositionConstraints(manifolds, dt);

        //Ground 판정
        foreach (var cont in contactList)
        {
            AccumulateGroundContact(cont.rigidA, cont.rigidB, cont);
        }

        //커밋 단계
        foreach (var rb in rigidBodies3D) rb.Commit(dt);

        //Sleeping -> 정지 판정
        // PhysicsStep 마지막
        foreach (var rb in rigidBodies3D)
        {
            CheckSleeping(rb);
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
    void Handle3DCollisionDetection()
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
    /// Contact Solver (GS Iteration)
    /// </summary>
    /// <param name="manifolds"></param>
    void SolveVelocityConstraints(List<ContactManifold> manifolds, float dt)
    {
        for (int iter = 0; iter < solverIterations; iter++)
        {
            foreach (var manifold in manifolds)
            {
                foreach (var cp in manifold.points)
                {
                    //SolveContactPoint(manifold, cp);
                }
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

    /// <summary>
    /// Sleeping 판정
    /// </summary>
    /// <param name="rigid"></param>
    void CheckSleeping(CustomRigidBody rigid)
    {

    }
}
