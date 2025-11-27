using UnityEngine;

public struct RigidbodyState2D
{
    public Vec3 position;
}

public class CustomRigidbody2D : MonoBehaviour
{
    [Header("Physical Properties")]
    public Vec2 velocity = new Vec2(0,0);//속도
    public Vec2 acceleration = new Vec2(0,0);//가속도
    public Mass mass = new Mass();//질량
    public Vec2 gravity2D = new Vec2(0, -9.81f);

    [Header("Material")]
    public CustomPhysicsMaterial physicMaterial;//재질

    [Header("Force")]
    public Vec2 externalForce = VectorMathUtils.ZeroVector2D();//외력
    public Vec2 accumulatedForce = VectorMathUtils.ZeroVector2D();//내부 힘

    [Header("Option")]
    public bool useGravity = true;

    //위치 보정
    public RigidbodyState previousState;//이전 상태
    public RigidbodyState currentState;//현재 상태

    //적분 방식
    public enum Integrator { ForwardEuler, SemiImplicitEuler }
    [Header("Integration")]
    public Integrator integrator = Integrator.ForwardEuler;

    //결과 값
    public Vec3 deltaPosition = VectorMathUtils.ZeroVector3D();

    private void Awake()
    {
        InitializePosition();
    }

    /// <summary>
    /// 초기 위치 설정
    /// </summary>
    void InitializePosition()
    {
        Vec3 startPos = new Vec3(transform.position.x, transform.position.y, 0);
        currentState.position = startPos;
        previousState.position = startPos;

        deltaPosition = VectorMathUtils.ZeroVector3D();
        velocity = VectorMathUtils.ZeroVector2D();
        acceleration = VectorMathUtils.ZeroVector2D();
    }

    //Physics Step → Collision → Resolve → Commit → Render Step(Interpolation)
    public void PhysicsStep(float dt)
    {
        //이전 상태 저장
        previousState.position = currentState.position;

        //알짜힘 구하기
        accumulatedForce += useGravity ? gravity2D * mass.value : VectorMathUtils.ZeroVector2D();//중력
        accumulatedForce += (velocity*(-1)) * physicMaterial.linearDrag;//저항힘
        Vec2 totalForce = externalForce + accumulatedForce;//알짜힘

        //총 가속도
        acceleration = totalForce / mass.value;

        //적분방식에 따른 속도, 위치
        if (integrator == Integrator.ForwardEuler)
        {
            //이동
            IntegratePosition(velocity, dt);
            //속도
            IntegrateVelocity(acceleration, dt);
        }
        else
        {
            //속도
            IntegrateVelocity(acceleration, dt);
            //이동
            IntegratePosition(velocity, dt);
        }
        //힘 초기화
        ClearForces();

        //현재 상태 업데이트
        currentState.position += new Vec3(deltaPosition.x, deltaPosition.y, 0);

        //Nan 방지
        Sanitize();
    }

    /// <summary>
    /// 외력 추가
    /// </summary>
    /// <param name="f"></param>
    public void AddForce(Vec2 f)
    {
        externalForce += f;
    }

    /// <summary>
    /// 내부 힘 추가
    /// </summary>
    /// <param name="f"></param>
    public void AddInternalForce(Vec2 f)
    {
        accumulatedForce += f;
    }

    /// <summary>
    /// 힘 초기화
    /// </summary>
    public void ClearForces()
    {
        externalForce = VectorMathUtils.ZeroVector2D();
        accumulatedForce = VectorMathUtils.ZeroVector2D();
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
    void IntegrateVelocity(Vec2 acceleration, float dt)
    {
        velocity += (acceleration * dt);
    }
    /// <summary>
    /// 위치 적분
    /// </summary>
    /// <param name="acceleration">속도</param>
    /// <param name="dt">시간 간격</param>
    void IntegratePosition(Vec2 velocity, float dt)
    {
        Vec3 velocity3D = new Vec3(velocity.x, velocity.y, 0);
        deltaPosition += (velocity3D * dt);
    }

    /// <summary>
    /// Nan 방지
    /// </summary>
    void Sanitize()
    {
        if (float.IsNaN(velocity.x) || float.IsInfinity(velocity.x)) velocity.x = 0;
        if (float.IsNaN(velocity.y) || float.IsInfinity(velocity.y)) velocity.y = 0;
       
        if (float.IsNaN(acceleration.x) || float.IsInfinity(acceleration.x)) acceleration.x = 0;
        if (float.IsNaN(acceleration.y) || float.IsInfinity(acceleration.y)) acceleration.y = 0;
       
        if (float.IsNaN(deltaPosition.x) || float.IsInfinity(deltaPosition.x)) deltaPosition.x = 0;
        if (float.IsNaN(deltaPosition.y) || float.IsInfinity(deltaPosition.y)) deltaPosition.y = 0;
        if (float.IsNaN(deltaPosition.z) || float.IsInfinity(deltaPosition.z)) deltaPosition.z = 0;
    }
}
