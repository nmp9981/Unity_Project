using UnityEngine;

public class EulerIntegrator : MonoBehaviour
{
    public Vector3 velocity;
    public Vector3 acceleration;
    public float mass = 1f;
    public Vector3 externalForces = Vector3.zero;
    public float linearDrag = 0f;

    //초기 위치, 초기 속도
    public Vector3 initialPosition;
    public Vector3 initialVelocity;

    void Start()
    {
        transform.position = initialPosition;
        velocity = initialVelocity;
    }
    private void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        //알짜힘 구하기
        Vector3 gravityForce = Physics.gravity * mass;//중력
        Vector3 dragForce = -velocity * linearDrag;//저항힘
        Vector3 totalForce = externalForces + gravityForce + dragForce;//알짜힘

        //총 가속도
        acceleration = totalForce / mass;

        //전진 오일러
        // x_{n+1} = x_n + v_n * dt
        // v_{n+1} = v_n + a_n * dt
        //transform.position += velocity * dt;
        //velocity += acceleration * dt;

        // Semi-implicit (symplectic) Euler:
        // v_{n+1} = v_n + a_n * dt
        // x_{n+1} = x_n + v_{n+1} * dt
        velocity += acceleration * dt;
        transform.position += velocity * dt;

        Debug.DrawRay(transform.position, velocity, Color.green);
        Debug.DrawRay(transform.position, acceleration, Color.red);
    }
}
