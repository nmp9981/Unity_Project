using UnityEngine;

public class TimeCompare : MonoBehaviour
{
    public Vector3 velocity;//속도
    public Vector3 acceleration;//가속도
    public float mass = 1f;//질량

    //중력, 마찰
    public Vector3 externalForce = Vector3.zero;//외력
    public float linearDrag = 0.1f;//저항

    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        //중력
        Vector3 gravity = Physics.gravity * mass;

        //총 가속도
        Vector3 totalForce = externalForce+gravity-velocity*linearDrag;
        acceleration = totalForce / mass;

        //Forward Euler 적분 방식 구현
        //속도
        velocity += (acceleration*dt);
        //이동
        transform.position += (velocity*dt);

        //시각화
        Debug.DrawRay(transform.position, velocity, Color.green);
        Debug.DrawRay(transform.position, acceleration, Color.red);
    }
}
