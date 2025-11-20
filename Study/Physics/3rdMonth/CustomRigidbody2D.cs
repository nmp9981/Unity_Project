using UnityEngine;

public class CustomRigidbody2D : MonoBehaviour
{
    [Header("Physical Properties")]
    public Vector2 velocity;//속도
    public Vector2 acceleration;//가속도
    public Mass mass = new Mass();//질량
    public Vector2 gravity2D = new Vector2(0, -9.81f);

    [Header("Material")]
    public CustomPhysicsMaterial physicMaterial;//재질

    [Header("Force")]
    public Vector2 externalForce = Vector2.zero;//외력
    public Vector2 accumulatedForce = Vector2.zero;//내부 힘

    [Header("Option")]
    public bool useGravity = true;

    //적분 방식
    public enum Integrator { ForwardEuler, SemiImplicitEuler }
    [Header("Integration")]
    public Integrator integrator = Integrator.ForwardEuler;

    void FixedUpdate()
    {
        Step(Time.fixedDeltaTime);
    }

    void Step(float dt)
    {
        //알짜힘 구하기
        accumulatedForce += useGravity ? gravity2D * mass.value : Vector2.zero;//중력
        accumulatedForce += -velocity * physicMaterial.linearDrag;//저항힘
        Vector2 totalForce = externalForce + accumulatedForce;//알짜힘

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
    public void AddForce(Vector2 f)
    {
        externalForce += f;
    }

    /// <summary>
    /// 내부 힘 추가
    /// </summary>
    /// <param name="f"></param>
    public void AddInternalForce(Vector2 f)
    {
        accumulatedForce += f;
    }

    /// <summary>
    /// 힘 초기화
    /// </summary>
    public void ClearForces()
    {
        externalForce = Vector2.zero;
        accumulatedForce = Vector2.zero;
    }

    /// <summary>
    /// 속도 적분
    /// </summary>
    /// <param name="acceleration">가속도</param>
    /// <param name="dt">시간 간격</param>
    void IntegrateVelocity(Vector2 acceleration, float dt)
    {
        velocity += (acceleration * dt);
    }
    /// <summary>
    /// 위치 적분
    /// </summary>
    /// <param name="acceleration">속도</param>
    /// <param name="dt">시간 간격</param>
    void IntegratePosition(Vector2 velocity, float dt)
    {
        Vector3 velocity3D = new Vector3(velocity.x, velocity.y, 0);
        transform.position += (velocity3D * dt);
    }
}
