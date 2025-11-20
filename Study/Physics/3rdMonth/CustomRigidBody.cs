using UnityEngine;

public class CustomRigidBody : MonoBehaviour
{
    [Header("Physical Properties")]
    public Vector3 velocity;//속도
    public Vector3 acceleration;//가속도
    public Mass mass = new Mass();//질량

    [Header("Material")]
    public CustomPhysicsMaterial phsicMaterial;//재질

    [Header("Force")]
    public Vector3 externalForce = Vector3.zero;//외력
    public Vector3 accumulatedForce = Vector3.zero;//내부 힘
    
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
        accumulatedForce += useGravity?Physics.gravity * mass.value:Vector3.zero;//중력
        accumulatedForce += -velocity * phsicMaterial.linearDrag;//저항힘
        Vector3 totalForce = externalForce + accumulatedForce;//알짜힘

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
    public void AddForce(Vector3 f)
    {
        externalForce += f;
    }

    /// <summary>
    /// 내부 힘 추가
    /// </summary>
    /// <param name="f"></param>
    public void AddInternalForce(Vector3 f)
    {
        accumulatedForce += f;
    }

    /// <summary>
    /// 힘 초기화
    /// </summary>
    public void ClearForces()
    {
        externalForce = Vector3.zero;
        accumulatedForce = Vector3.zero;
    }

    /// <summary>
    /// 속도 적분
    /// </summary>
    /// <param name="acceleration">가속도</param>
    /// <param name="dt">시간 간격</param>
    void IntegrateVelocity(Vector3 acceleration, float dt)
    {
        velocity += (acceleration * dt);
    }
    /// <summary>
    /// 위치 적분
    /// </summary>
    /// <param name="acceleration">속도</param>
    /// <param name="dt">시간 간격</param>
    void IntegratePosition(Vector3 velocity, float dt)
    {
        transform.position += (velocity * dt);
    }
}
