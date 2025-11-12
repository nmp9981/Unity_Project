using UnityEngine;

public class TimeCompare : MonoBehaviour
{
    [Header("Physical Properties")]
    public Vector3 velocity;//속도
    public Vector3 acceleration;//가속도
    public float mass = 1f;//질량

    [Header("Force")]
    public Vector3 externalForce = Vector3.zero;//외력
    public float linearDrag = 0.1f;//저항

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
        Vector3 gravityForce = Physics.gravity * mass;//중력
        Vector3 dragForce = -velocity * linearDrag;//저항힘
        Vector3 totalForce = externalForce+gravityForce + dragForce;//알짜힘

        //총 가속도
        acceleration = totalForce / mass;

        //적분방식에 따른 속도, 위치
        if(integrator == Integrator.ForwardEuler)
        {
            //속도
            velocity += (acceleration * dt);
            //이동
            transform.position += (velocity * dt);
        }
        else
        {
            //속도
            velocity += (acceleration * dt);
            //이동
            transform.position += (velocity * dt);
        }

        //debug
        Debug.Log($"{gameObject.name}의 위치 : {transform.position.y}");
    }

    //helper API
    public void ApplyForce(Vector3 f)
    {
        externalForce = f;
    }
    public void ClearForces()
    {
        externalForce = Vector3.zero;
    }
}
