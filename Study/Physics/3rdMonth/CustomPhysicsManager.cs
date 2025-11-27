using System.Collections.Generic;
using UnityEngine;

public class CustomPhysicsManager : MonoBehaviour
{
    List<CustomRigidbody2D> rigidBodies2D = new List<CustomRigidbody2D>();
    List<CustomRigidBody> rigidBodies3D = new List<CustomRigidBody>();
    List<CustomCollider3D> colliders3D = new List<CustomCollider3D>();

    private void Awake()
    {
        rigidBodies2D.AddRange(FindObjectsByType<CustomRigidbody2D>(FindObjectsSortMode.None));
        rigidBodies3D.AddRange(FindObjectsByType<CustomRigidBody>(FindObjectsSortMode.None));
        colliders3D.AddRange(FindObjectsByType<CustomCollider3D>(FindObjectsSortMode.None));
    }

    private void Update()
    {
        RenderStep(Time.deltaTime);
    }
    private void FixedUpdate()
    {
        PhysicsStep(Time.fixedDeltaTime);
    }

    /// <summary>
    /// 물리 스텝
    /// Physics Step → Collision → Resolve → Commit → Render Step(Interpolation)
    /// </summary>
    /// <param name="dt"></param>
    void PhysicsStep(float dt)
    { 
        //모든 물리 step
        foreach (var rb in rigidBodies2D)
        {
            rb.PhysicsStep(dt);
        }
        foreach (var rb in rigidBodies3D)
        {
            rb.PhysicsStep(dt);
        }

        //충돌 체크
        Handle3DCollisions();

        //초기화
        foreach (var rb in rigidBodies2D)
        {
            rb.ClearPosition();
        }
        foreach (var rb in rigidBodies3D)
        {
            rb.ClearPosition();
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
        for(int i = 0; i < colliders3D.Count; i++)
        {
            for(int j = i + 1; j < colliders3D.Count; j++)
            {
                var collA = colliders3D[i];
                var collB = colliders3D[j];

                //충돌 여부 판정
                if (!ColiisionUtility.IsCollisionAABB3D(collA, collB)) continue;

                //충돌 정보
                ContactInfo contactInfo = ColiisionUtility.GetContactAABB3D(collA, collB);

                //충돌 응답
                ColiisionUtility.ResponseCollision3D(collA, collB, contactInfo);

                //위치 보정
                PositionalCorrection(collA.rigidBody, collB.rigidBody, contactInfo);
            }
        }
    }
    /// <summary>
    /// 위치 상태 업데이트
    /// </summary>
    void PositionUpdateState()
    {
        float alpha = (Time.time - Time.fixedTime) / Time.fixedDeltaTime;
        float alpha01 = MathUtility.ClampValue(alpha,0,1);//보간 값

        foreach (var rb in rigidBodies2D)
        {
            Vec3 lerped = Vec3.Lerp(rb.previousState.position, rb.currentState.position, alpha01);

            rb.transform.position = new Vector3(lerped.x, lerped.y, 0);
        }
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
        float correctionPercent = 0.5f; // 양쪽 50%씩
        float slop = 0.01f;             // 안정화 값 (penetration이 너무 작으면 무시)

        float correction = Mathf.Max(contact.penetration - slop, 0f);
        Vec3 correctionVec =  contact.normal*correction;

        Vec3 rigidAPos = correctionVec * correctionPercent;
        Vec3 rigidBPos = correctionVec * correctionPercent;
        rigidA.deltaPosition -= rigidAPos;
        rigidB.deltaPosition += rigidBPos;
    }
}
