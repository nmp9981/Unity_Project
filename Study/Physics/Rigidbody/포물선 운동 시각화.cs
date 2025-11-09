using UnityEngine;

public class ProjectTile : MonoBehaviour
{
    public float initialSpeed = 10f;
    public float angle = 45f;

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        float rad = angle * Mathf.Deg2Rad;
        Vector3 dir = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0);
        rb.linearVelocity = dir * initialSpeed;
    }

    void FixedUpdate()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 velocity = rb.linearVelocity;
        Vector3 acceleration = Physics.gravity;

        Debug.DrawRay(transform.position, velocity, Color.green);
        Debug.DrawRay(transform.position, acceleration, Color.red);
    }
}
