using UnityEngine;

//진자 운동
public class PendulumEnergy : MonoBehaviour
{
    Rigidbody rb;
    float mass;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mass = rb.mass;
    }

    void FixedUpdate()
    {
        float height = transform.position.y;
        float kinetic = 0.5f * mass * rb.linearVelocity.sqrMagnitude;
        float potential = mass * Physics.gravity.magnitude * height;
        Debug.DrawRay(transform.position, Vector3.up * potential / 200f, Color.green);
        Debug.DrawRay(transform.position, rb.linearVelocity, Color.red);
    }

//스프링
public Transform anchor;
SpringJoint spring;
Rigidbody rb;

void Start()
{
    spring = GetComponent<SpringJoint>();
    rb = GetComponent<Rigidbody>();
}

void FixedUpdate()
{
    Vector3 force = spring.spring * (anchor.position - transform.position);
    Debug.DrawLine(anchor.position, transform.position, Color.yellow);
    Debug.DrawRay(transform.position, force.normalized, Color.cyan);
}
}
