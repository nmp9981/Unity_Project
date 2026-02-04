using System.Collections.Generic;
using UnityEngine;

public class PhysicsWorld : MonoBehaviour
{
 //싱글톤
 public static PhysicsWorld Instance { get; private set; }
 
    [SerializeField]
    List<CustomRigidBody> rigidBodies3D = new List<CustomRigidBody>();
    [SerializeField]
    List<CustomCollider3D> colliders3D = new List<CustomCollider3D>();

    //물리 작동 여부
    public bool physicsPaused = false;

    //지면 판정
    public float groundThreshold;

    //충돌 정보 모음
    public List<ContactInfo> contactList = new List<ContactInfo>();//이건 사용하지 않음
    //Manifold 리스트, 이게 충돌 정보
    public List<ContactManifold> manifolds = new();
    
    private void Update()
    {
        // P 버튼으로 물리만 멈춤/재시작
        if (Input.GetKeyDown(KeyCode.P))
        {
            physicsPaused = !physicsPaused;
        }

        if (!physicsPaused)
            PhysicsStep(Time.deltaTime);

        RenderStep();
    }

    #region Physics Step Pipeline
    /// <summary>
    /// 물리 스텝
    /// Physics Step → Collision → Resolve → Commit -> Sleep → Render Step(Interpolation) 
    /// </summary>
    /// <param name="dt"></param>
    public void PhysicsStep(float dt)
    {
        PredictBodies(dt);
        DetectCollisions();
        BuildManifolds();
        AccumulateGroundContacts();
        SolveIslands(dt);
        CommitBodies(dt);
        UpdateSleeping();
    }
    #endregion

    /// <summary>
    /// 1. Predict : 예측
    /// </summary>
    /// <param name="dt"></param>
    void PredictBodies(float dt)
    {
        foreach (var rb in rigidBodies3D)
        {
            rb.isGrounded = false;
            rb.PredictState(dt);
        }
    }
    /// <summary>
    /// 2. Collision Detection : 충돌 체크
    /// contactList 생성, 내부에서 PositionalCorrection만 수행
    /// </summary>
    void DetectCollisions()
    {
        contactList.Clear();

        for (int i = 0; i < colliders3D.Count; i++)
        {
            for (int j = i + 1; j < colliders3D.Count; j++)
            {
                var colA = colliders3D[i];
                var colB = colliders3D[j];

                if (IslandSleepSystem.CanSkipCollision(colA.rigidBody, colB.rigidBody))
                    continue;

                if (!ColiisionUtility.IsCollisionAABB3D(colA, colB))
                    continue;

                contactList.Add(
                    ColiisionUtility.GetContactAABB3D(colA, colB)
                );
            }
        }
    }
    /// <summary>
    /// 3. Manifold Update
    /// </summary>
    void BuildManifolds()
    {
        ContactSolver.UpdateManifolds(manifolds,contactList,out manifolds);
    }
    /// <summary>
    /// 4.Ground 증거 수집
    /// </summary>
    void AccumulateGroundContacts()
    {
        foreach (var manifold in manifolds)
        {
            foreach (var cp in manifold.points)
            {
                if (cp.contactNormal.y > groundThreshold)
                {
                    manifold.rigidA.hasGroundContact = true;

                    if (cp.contactNormal.y > manifold.rigidA.groundNormal.y)
                        manifold.rigidA.groundNormal = cp.contactNormal;
                }

                if (cp.contactNormal.y < -groundThreshold)
                {
                    manifold.rigidB.hasGroundContact = true;

                    if (-cp.contactNormal.y > manifold.rigidB.groundNormal.y)
                        manifold.rigidB.groundNormal = cp.contactNormal * (-1);
                }
            }
        }
    }
   
    /// <summary>
    /// 5. Solve
    /// Solver는 Island 단위로 돌린다
    /// </summary>
    /// <param name="dt"></param>
    void SolveIslands(float dt)
    {
        //접촉점 그룹
        var islands = IslandBuilder.Build(rigidBodies3D, manifolds);

        foreach (var island in islands)
        {
            //sleeping 상태
            if (island.isSleeping)
                continue;

            WarmStartIsland(island, dt);
            SolveIsland(island, dt);
            //Impulse값 저장
            ContactSolver.SaveImpulse(island);
        }
    }
    /// <summary>
    /// WarmStart 시작 
    /// </summary>
    /// <param name="island"></param>
    /// <param name="dt"></param>
    void WarmStartIsland(Island island, float dt)
    {
        //지난 프레임에 이미 구해놓은 impulse를 이번 프레임 Solver 시작 전에 미리 적용
        foreach (var manifold in island.manifolds)
        {
            foreach (var cp in manifold.points)
            {
                //지난 프레임에 수렴한 해를 이번 프레임의 초기값으로 재사용
                if(!cp.isSpeculative)
                    ContactSolver.WarmStart(manifold, cp);
            }
        }
        // 🔥 Joint Warm Start
        foreach (var joint in island.joints)
        {
            joint.WarmStart(dt);
        }
    }
    /// <summary>
    /// 속도, 위치 Solver
    /// </summary>
    /// <param name="island"></param>
    /// <param name="dt"></param>
    void SolveIsland(Island island, float dt)
    {
        // 5. Velocity Solver, 속도 -> 위치
        ContactSolver.SolveVelocityConstraints(island.manifolds, dt);
        // 6. Contact Solver (GS Iteration)
        ContactSolver.SolvePositionConstraints(island.manifolds, dt);
    }
    /// <summary>
    /// 6. Commit
    /// </summary>
    /// <param name="dt"></param>
    void CommitBodies(float dt)
    {
        foreach (var rb in rigidBodies3D)
            rb.Commit(dt);
    }
   /// <summary>
   /// 7. Sleeping
   /// </summary>
    void UpdateSleeping()
    {
        IslandSleepSystem.UpdateSleeping(IslandBuilder.Build(rigidBodies3D, manifolds));
    }

    public void Create_ColliderAndRigid3D()
    {
        rigidBodies3D.AddRange(FindObjectsByType<CustomRigidBody>(FindObjectsSortMode.None));
        colliders3D.AddRange(FindObjectsByType<CustomCollider3D>(FindObjectsSortMode.None));
    }

    /// <summary>
    /// 렌더링 스텝
    /// 렌더링은 물리 상태를 읽기만 한다.
    /// 위치 상태 업데이트
    /// </summary>
    /// <param name="dt"></param>
    void RenderStep()
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
    /// RayCast 3D
    /// </summary>
    /// <param name="ray"></param>
    /// <param name="maxDistance"></param>
    /// <param name="hit"></param>
    /// <param name="layerMask"></param>
    /// <returns></returns>
    public bool Raycast(Ray3D ray,float maxDistance,out RaycastHit3D finalHit,int layerMask = ~0)
    {
        finalHit = default;
        float minT = float.MaxValue;
        bool hasHit = false;
        Vec3 normal = VectorMathUtils.ZeroVector3D();

        //NarraowCast
        float bestT = maxDistance;
        foreach (var collider in colliders3D)
        {
            //broadCast
            if (!collider.RaycastAABB(ray, collider.minPosition(), collider.maxPosition(), maxDistance, out minT,out normal))
                continue;

            //NarrowCast
            if (((1 << collider.layer) & layerMask) == 0)
                continue;

            if (!collider.RayCast(ray,bestT, out RaycastHit3D hit))
                continue;

            if (hit.t < minT)
            {
                minT = hit.t;
                finalHit = hit;
                hasHit = true;
            }
        }
      
        return hasHit;
    }
    /// <summary>
    /// 조건 만족하는 object전부 소환
    /// </summary>
    /// <param name="ray"></param>
    /// <param name="maxDistance"></param>
    /// <param name="results"></param>
    /// <param name="layerMask"></param>
    /// <returns></returns>
    public int RaycastAll(Ray3D ray,float maxDistance,List<RaycastHit3D> results, int layerMask = ~0)
    {
        results.Clear();

        foreach (var col in colliders3D)
        {
            //broadCast
            if (!col.RaycastAABB(ray, col.minPosition(), col.maxPosition(), maxDistance, out _, out _))
                continue;

            //NarrowCast
            if (((1 << col.layer) & layerMask) == 0)
                continue;

            if (col.RayCast(ray, maxDistance, out RaycastHit3D hit))
            {
                results.Add(hit);
            }
        }
        //거리순 정렬
        results.Sort((a, b) => a.t.CompareTo(b.t));

        return results.Count;
    }
    /// <summary>
    /// capsule 충돌 판정
    /// Sweep결과중 최소 t선택
    /// </summary>
    /// <param name="capsule"></param>
    /// <param name="capsuleVel"></param>
    /// <param name="maxT"></param>
    /// <param name="bestHit"></param>
    /// <returns></returns>
    bool SweepWorldCapsule(CapsuleCollider3D capsuleCol, Vec3 capsuleVel, float maxT,out SweepHit bestHit)
    {
        bestHit = default;
        bool hasHit = false;
        float bestT = maxT;

        Capsule capsule = capsuleCol.GetWorldCapsule();

        foreach (var collider in colliders3D)
        {
            if (collider == capsuleCol) continue;//자기 자신

            //Sphere or box
            if (collider is SphereCollider3D sphereCol)
            {
                //구 생성
                Sphere sphere = new Sphere
                {
                    center = sphereCol.transform3D.position,
                    radius = sphereCol.radius,
                    collider = sphereCol
                };

                Vec3 sphereVel = VectorMathUtils.ZeroVector3D();
                if (sphereCol.rigidBody != null)
                    sphereVel = sphereCol.rigidBody.velocity;

                if (SphereSweep.SweepCapsuleSphere(capsule, capsuleVel, sphere,sphereVel,maxT, out SweepHit hit))
                {
                    if (hit.t < bestT)
                    {
                        bestT = hit.t;
                        bestHit = hit;
                        hasHit = true;
                    }
                }
            }
            if (collider is BoxCollider3D boxCol)
            {
                Box box = new Box
                {
                    center = boxCol.transform3D.position,
                    rotation = boxCol.transform3D.rotation,
                    halfExtent = boxCol.halfExtent,
                    collider=boxCol
                };

                Vec3 boxVel = VectorMathUtils.ZeroVector3D();
                if (boxCol.rigidBody != null)
                    boxVel = boxCol.rigidBody.velocity;

                if (SphereSweep.SweepCapsuleOBB(capsule, capsuleVel,box,boxVel,maxT, out SweepHit hit))
                {
                    if (hit.t < bestT)
                    {
                        bestT = hit.t;
                        bestHit = hit;
                        hasHit = true;
                    }
                }
            }
        }
        return hasHit;
    }

    public bool SweepSphere(Vec3 center,
    float radius,
    Vec3 dir,
    float maxT,
    out SweepHit hit)
    {
        hit = default;
        bool hasHit = false;
        float bestT = maxT;

        foreach (var collider in colliders3D)
        {
            if (collider.SweepSphere(
                center, radius, dir, maxT,
                out SweepHit temp))
            {
                if (temp.t < bestT)
                {
                    bestT = temp.t;
                    hit = temp;
                    hasHit = true;
                }
            }
        }

        return hasHit;
    }
}
