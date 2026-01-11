using UnityEngine;
using static UnityEditor.ShaderData;

public struct RigidbodyState
{
    public Vec3 position;
    public Vec3 velocity;
    public Vec3 accel;
}

public class CustomRigidBody : MonoBehaviour
{
    public int id;//고유 식별 번호

    [Header("Physical Properties")]
    public Mass mass = new Mass();//질량
    public Vec3 gravity3D = new Vec3(0, -9.81f,0);
    public float linearDrag = 0.1f;     // 공기 저항
    public float angularDrag = 0.05f;
    public CustomCollider3D col;   // 연결된 collider 참조

    // 상태는 position만 관리
    public Vec3 position;//위치
    public Vec3 prevPosition;//이전 위치

    //회전량
    public CustomQuaternion rotation;//변환, Transform
    public float orientation;//현재 회전 상태 state, 이 접촉점에서 밀린 만큼, 물체가 얼마나 돌아야 하는가?

    // 보조 물리량
    public Vec3 velocity = new Vec3(0, 0, 0);//속도
    public Vec3 acceleration = new Vec3(0, 0, 0);//가속도
    public Vec3 angularVelocity = new Vec3(0, 0, 0);//각속도
    // 예측 위치
    public Vec3 predictedPosition = new Vec3(0,0,0);

    [Header("Ground")]
    public bool isGrounded;          //땅에 닿았는가?, Commit이후 확정
    public float groundedThreshold = 0.05f;//접지 허용 오차

    // Solve 단계에서만 쓰는 임시 정보
    public bool hasGroundContact;      // 이번 프레임에 지면 접촉이 있었는가?
    public Vec3 groundNormal = new Vec3(0,0,0);          // 가장 신뢰할 수 있는 접촉 normal

    //외력 여부
    public bool hasExternalForce;

    //멈춤 상태 여부
    public bool isSleepling = false;

    //자신이 속한 Island
    public Island island;

    //정적 물체 여부
    public bool isStatic;

    //질량 역수
    public float invMass;
    //관성 모먼트
    public float inertia;
    public float invInertia;

    [Header("Force")]
    public Vec3 externalForce = VectorMathUtils.ZeroVector3D();//외력
    public Vec3 accumulatedForce = VectorMathUtils.ZeroVector3D();//내부 힘
    
    [Header("Option")]
    public bool useGravity = true;

    //위치 보정
    public RigidbodyState previousState;//이전 상태
    public RigidbodyState currentState;//현재 상태

    //적분 방식
    public enum Integrator { ForwardEuler, SemiImplicitEuler, Verlet, RK4 }
    [Header("Integration")]
    public Integrator integrator = Integrator.ForwardEuler;

    //결과 값
    public Vec3 deltaPosition = VectorMathUtils.ZeroVector3D();

    private void Start()
    {
        InitializePosition();
        SetMass();
    }

    /// <summary>
    /// 초기 위치 설정
    /// </summary>
    void InitializePosition()
    {
        Vec3 startPos = new Vec3(transform.position.x, transform.position.y, transform.position.z);
        currentState.position = startPos;
        currentState.velocity = VectorMathUtils.ZeroVector3D();
        currentState.accel = VectorMathUtils.ZeroVector3D();
        previousState.position = currentState.position;
        previousState.velocity = VectorMathUtils.ZeroVector3D();
        previousState.accel = VectorMathUtils.ZeroVector3D();

        deltaPosition = VectorMathUtils.ZeroVector3D();

        currentState.velocity = new Vec3(5, 8, 0);
    }
    /// <summary>
    /// 질량 세팅
    /// </summary>
    void SetMass()
    {
        if (isStatic)
        {
            invMass = 0f;
            invInertia = 0f;
        }
        else
        {
            invMass = 1f / mass.value;
            invInertia = 1f / inertia;
        }
    }

    //Physics Step → Collision → Resolve → Commit → Render Step(Interpolation)
    public void PhysicsStep(float dt)
    {
        //이전 상태 저장
        previousState = currentState;

        //알짜힘 구하기
        Vec3 totalForce = VectorMathUtils.ZeroVector3D();
        if (useGravity) totalForce += gravity3D * mass.value;//중력
        totalForce += externalForce;//외력
        totalForce += accumulatedForce;//자체힘
        //totalForce += currentState.velocity * -linearDrag;//저항힘

        //적분방식에 따른 속도, 위치
        if (integrator == Integrator.ForwardEuler)
        {
            //총 가속도
            acceleration = totalForce / mass.value;

            //이동
            IntegratePosition(currentState.velocity, dt);
            //속도
            IntegrateVelocity(acceleration, dt);
            // 현재 상태에 deltaPosition 적용
            currentState.position += deltaPosition;
            //힘 초기화
            Commit(dt);
        }
        else if(integrator == Integrator.SemiImplicitEuler)
        {
            //총 가속도
            acceleration = totalForce / mass.value;

            //속도
            IntegrateVelocity(acceleration, dt);
            //이동
            IntegratePosition(currentState.velocity, dt);
            // 현재 상태에 deltaPosition 적용
            currentState.position += deltaPosition;
            //힘 초기화
            Commit(dt);
        }
        else if(integrator == Integrator.Verlet)
        {
            RigidbodyState newState= IntegrationUtility.VelocityVerlet(dt, currentState, totalForce, mass.value);
           
            previousState = currentState;
            //렌더용 이동량
            deltaPosition = newState.position - currentState.position;
            currentState = newState;
            Commit(dt);
        }
        else if(integrator == Integrator.RK4)
        {
            RigidbodyState newState = IntegrationUtility.IntegrateRK4(currentState,totalForce,mass.value,dt);

            previousState = currentState;
            Vec3 oldPos = currentState.position;
            currentState = newState;

            //렌더용 이동량
            deltaPosition = newState.position - oldPos;
            
            Commit(dt);
        }
      
        //Nan방지
        Sanitize();
    }

    /// <summary>
    /// 다음 상태 예측
    /// 읽기 전용
    /// </summary>
    public void PredictState(float dt)
    {
        // 이전 상태 저장
        prevPosition = position;

        //알짜힘 구하기
        Vec3 totalForce = VectorMathUtils.ZeroVector3D();
        if (useGravity) totalForce += gravity3D * mass.value;//중력
        totalForce += externalForce;//외력
        totalForce += accumulatedForce;//자체힘
        //totalForce += currentState.velocity * -linearDrag;//저항힘

        // 외력 → 가속도
        acceleration = totalForce / mass.value;

        // Semi-Implicit Euler
        velocity += acceleration * dt;
        predictedPosition = position + velocity * dt;
    }

    /// <summary>
    /// 외력 추가
    /// </summary>
    /// <param name="f"></param>
    public void AddForce(Vec3 f)
    {
        externalForce += f;
    }

    /// <summary>
    /// 내부 힘 추가
    /// </summary>
    /// <param name="f"></param>
    public void AddInternalForce(Vec3 f)
    {
        accumulatedForce += f;
    }

    /// <summary>
    /// 커밋 단계 - 이번 프레임의 물리 계산이 끝났다.
    /// 1. position 확정
    /// 2. velocity 재계산
    /// 3. 누적된 groundNormal 기반 속도 차단
    /// 4. grounded 판정 확정
    /// 5. 힘 초기화
    /// </summary>
    public void Commit(float dt)
    {

        velocity = (predictedPosition - prevPosition) / dt;
        position = predictedPosition;

        //누적된 groundNormal 기반 속도 차단, 법선 방향 이동 차단
        if (hasGroundContact)
        {
            float vn = Vec3.Dot(velocity, groundNormal);

            if (vn < 0f)
            {
                velocity -= groundNormal*vn;
            }
        }

        //지면 판정
        if (hasGroundContact && velocity.y <= groundedThreshold)
        {
            isGrounded = true;
            velocity.y = 0f;
        }
        else
        {
            isGrounded = false;
        }

        //마찰 계산, 마찰은 접촉 상태에서만 존재
        float staticThreshold = 0.05f;

        if (hasGroundContact)
        {
            //속도를 접선, 법선성분으로 분해
            //groundNormal * Vec3.Dot(velocity, groundNormal)는 groundNormal에 투영한 법선 속도로서
            //방향*크기로 구한다.
            //접선 속도 = 전체 − 법선, 접선 속도가 바닥을 따라 미끄러지는 속도
            Vec3 tangentVel = velocity - groundNormal * Vec3.Dot(velocity, groundNormal);

            if (tangentVel.Magnitude < staticThreshold)
            {
                // 정지 마찰
                velocity -= tangentVel;
            }
            else
            {
                // 동적 마찰
                float friction = 0.8f;
                velocity -= tangentVel * friction;
            }
        }

        externalForce = VectorMathUtils.ZeroVector3D();
        accumulatedForce = VectorMathUtils.ZeroVector3D();
        hasGroundContact = false;
        groundNormal = VectorMathUtils.ZeroVector3D();
    }
    /// <summary>
    /// 위치 초기화
    /// </summary>
    public void ClearPosition()
    {
        currentState.position += deltaPosition;
        deltaPosition = VectorMathUtils.ZeroVector3D();
    }

    /// <summary>
    /// 속도 적분
    /// </summary>
    /// <param name="acceleration">가속도</param>
    /// <param name="dt">시간 간격</param>
    public void IntegrateVelocity(Vec3 acceleration, float dt)
    {
        currentState.velocity += (acceleration * dt);
    }
    /// <summary>
    /// 위치 적분
    /// </summary>
    /// <param name="acceleration">속도</param>
    /// <param name="dt">시간 간격</param>
    public void IntegratePosition(Vec3 velocity, float dt)
    {
        deltaPosition = (velocity * dt);
    }

    /// <summary>
    /// Nan 방지
    /// </summary>
    void Sanitize()
    {
        if (float.IsNaN(currentState.velocity.x) || float.IsInfinity(currentState.velocity.x)) currentState.velocity.x = 0;
        if (float.IsNaN(currentState.velocity.y) || float.IsInfinity(currentState.velocity.y)) currentState.velocity.y = 0;
        if (float.IsNaN(currentState.velocity.z) || float.IsInfinity(currentState.velocity.z)) currentState.velocity.z = 0;

        if (float.IsNaN(currentState.accel.x) || float.IsInfinity(currentState.accel.x)) currentState.accel.x = 0;
        if (float.IsNaN(currentState.accel.y) || float.IsInfinity(currentState.accel.y)) currentState.accel.y = 0;
        if (float.IsNaN(currentState.accel.z) || float.IsInfinity(currentState.accel.z)) currentState.accel.z = 0;

        if (float.IsNaN(deltaPosition.x) || float.IsInfinity(deltaPosition.x)) deltaPosition.x = 0;
        if (float.IsNaN(deltaPosition.y) || float.IsInfinity(deltaPosition.y)) deltaPosition.y = 0;
        if (float.IsNaN(deltaPosition.z) || float.IsInfinity(deltaPosition.z)) deltaPosition.z = 0;
    }

    /// <summary>
    /// 월드 좌표 -> 로컬 좌표
    /// </summary>
    /// <param name="world"></param>
    /// <returns></returns>
    public Vec3 WorldToLocal(Vec3 world)
    {
        // world offset
        Vec3 diff = world - position;

        // vector → quaternion
        CustomQuaternion qv =
            new CustomQuaternion(0.0f, diff);

        // inverse rotation
        CustomQuaternion inv = rotation.Conjugate;

        // rotate back: q^-1 * v * q
        CustomQuaternion local =
            inv * qv * rotation;

        return local.vec;
    }
    /// <summary>
    /// 월드 회전 -> 로컬 회전
    /// </summary>
    /// <param name="worldDir"></param>
    /// <returns></returns>
    public Vec3 WorldToLocalDirection(Vec3 worldDir)
    {
        CustomQuaternion qInv = QuaternionUtility.Inverse(rotation);
        return Vec3.Rotation3DVec(qInv, worldDir);
    }
    /// <summary>
    /// 로컬 회전 -> 월드 회전
    /// </summary>
    /// <param name="localDir"></param>
    /// <returns></returns>
    public Vec3 LocalToWorldDirection(Vec3 localDir)
    {
        CustomQuaternion vQ = new CustomQuaternion(0.0f, localDir);
        CustomQuaternion worldQ = rotation * vQ * QuaternionUtility.Inverse(rotation);
        return worldQ.vec;
    }
    /// <summary>
    /// RigidBody의 로컬 앵커 좌표를 현재 월드 위치 + 회전 상태 기준으로 변환
    /// </summary>
    /// <param name="localAnchor"></param>
    /// <returns></returns>
    public Vec3 LocalToWorld(Vec3 localAnchor)
    {
        // local vector as quaternion (0, v)
        CustomQuaternion qv =
            new CustomQuaternion(0.0f, localAnchor);

        // rotate: q * v * q^-1
        CustomQuaternion rotated =
            rotation * qv * rotation.Conjugate;

        // translate
        return position + rotated.vec;
    }
}
