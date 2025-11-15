using UnityEngine;

public class ReflectTester : MonoBehaviour
{
    public Vector3 velocity;
    public Transform wall; // use wall normal
    public float restitution = 0.8f; // bounciness (0..1)

    void Start()
    {
        // initial
    }

    void FixedUpdate()
    {
        transform.position += velocity * Time.fixedDeltaTime;
        Debug.DrawRay(transform.position, velocity, Color.green);
    }

    void OnCollisionEnter(Collision col)
    {
        ContactPoint contact = col.contacts[0];
        Vector3 n = contact.normal.normalized; // surface normal

        // reflect v about n: r = v - 2*(v·n)*n
        Vector3 r = Vector3.Reflect(velocity, n);
        // apply restitution (energy loss)
        velocity = r * restitution;
    }
}
