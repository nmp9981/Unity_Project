using UnityEngine;

public class CustomRigidbody2D : MonoBehaviour
{
    [Header("Physical Properties")]
    public Vec2 velocity;//속도
    public Vec2 acceleration;//가속도
    public Mass mass = new Mass();//질량
    public Vec2 gravity2D = new Vec2(0, -9.81f);

    [Header("Material")]
    public CustomPhysicsMaterial physicMaterial;//재질

    [Header("Force")]
    public Vec2 externalForce = VectorMathUtils.ZeroVector2D();//외력
    public Vec2 accumulatedForce = VectorMathUtils.ZeroVector2D();//내부 힘

    [Header("Option")]
    public bool useGravity = true;

    //적분 방식
    public enum Integrator { ForwardEuler, SemiImplicitEuler }
    [Header("Integration")]
    public Integrator integrator = Integrator.ForwardEuler;

    //결과 값
    public Vec3 deltaPosition = VectorMathUtils.ZeroVector3D();

    public void Step(float dt)
    {
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
}
