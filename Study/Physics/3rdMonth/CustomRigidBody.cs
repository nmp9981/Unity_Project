using UnityEngine;

public struct RigidbodyState
{
    public Vec3 position;
}

public class CustomRigidBody : MonoBehaviour
{
    [Header("Physical Properties")]
    public Vec3 velocity;//속도
    public Vec3 acceleration;//가속도
    public Mass mass = new Mass();//질량
    public Vec3 gravity3D = new Vec3(0, -9.81f,0);

    [Header("Material")]
    public CustomPhysicsMaterial physicMaterial;//재질

    [Header("Force")]
    public Vec3 externalForce = VectorMathUtils.ZeroVector3D();//외력
    public Vec3 accumulatedForce = VectorMathUtils.ZeroVector3D();//내부 힘
    
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

    //Physics Step → Collision → Resolve → Commit → Render Step(Interpolation)
    public void PhysicsStep(float dt)
    {
        //이전 상태 저장
        previousState.position = currentState.position;

        //알짜힘 구하기
        accumulatedForce += useGravity? gravity3D * mass.value: VectorMathUtils.ZeroVector3D();//중력
        accumulatedForce +=(velocity *(-1)) * physicMaterial.linearDrag;//저항힘
        Vec3 totalForce = externalForce + accumulatedForce;//알짜힘

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
        currentState.position = new Vec3(transform.position.x,transform.position.y,transform.position.z);
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
    void IntegrateVelocity(Vec3 acceleration, float dt)
    {
        velocity += (acceleration * dt);
    }
    /// <summary>
    /// 위치 적분
    /// </summary>
    /// <param name="acceleration">속도</param>
    /// <param name="dt">시간 간격</param>
    void IntegratePosition(Vec3 velocity, float dt)
    {
        deltaPosition = (velocity * dt);
    }
}
