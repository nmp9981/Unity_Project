using UnityEngine;

public struct RigidbodyState
{
    public Vec3 position;
    public Vec3 velocity;
    public Vec3 accel;
}

public class CustomRigidBody : MonoBehaviour
{
    [Header("Physical Properties")]
    public Vec3 velocity = new Vec3(0,0,0);//속도
    public Vec3 acceleration = new Vec3(0, 0, 0);//가속도
    public Mass mass = new Mass();//질량
    public Vec3 gravity3D = new Vec3(0, -9.81f,0);
    public float linearDrag = 0.1f;     // 공기 저항
    public float angularDrag = 0.05f;
    public CustomCollider3D col;   // 연결된 collider 참조

    [Header("Ground")]
    public bool isGrounded;          //땅에 닿았는가?
    public float groundedThreshold = 0.05f;//접지 허용 오차

    [Header("Force")]
    public Vec3 externalForce = VectorMathUtils.ZeroVector3D();//외력
    public Vec3 accumulatedForce = VectorMathUtils.ZeroVector3D();//내부 힘
    
    [Header("Option")]
    public bool useGravity = true;

    //위치 보정
    public RigidbodyState previousState;//이전 상태
    public RigidbodyState currentState;//현재 상태

    //적분 방식
    public enum Integrator { ForwardEuler, SemiImplicitEuler, Verlet }
    [Header("Integration")]
    public Integrator integrator = Integrator.ForwardEuler;

    //결과 값
    public Vec3 deltaPosition = VectorMathUtils.ZeroVector3D();


    private void Start()
    {
        InitializePosition();
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
        totalForce += currentState.velocity * -linearDrag;//저항힘

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
            ClearForces();
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
            ClearForces();
        }
        else if(integrator == Integrator.Verlet)
        {
            RigidbodyState newState= IntegrationUtility.VelocityVerlet(dt, currentState, totalForce, mass.value);
            deltaPosition = newState.position - currentState.position;
            currentState = newState;
        }
      

        //Nan방지
        Sanitize();
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
    /// 힘 초기화
    /// </summary>
    public void ClearForces()
    {
        externalForce = VectorMathUtils.ZeroVector3D();
        accumulatedForce = VectorMathUtils.ZeroVector3D();
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
}
