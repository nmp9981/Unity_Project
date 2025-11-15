using UnityEngine;

public class VectorBasic : MonoBehaviour
{
    Transform A;
    Transform B;

void Start() => rb = GetComponent<Rigidbody>();

void FixedUpdate()
{
    // gravity vector
    Vector3 g = Physics.gravity;

    // assume contact normal = slope.up (or use raycast to get actual normal)
    Vector3 normal = slope.up;

    // perpendicular (normal) component: project g onto normal
    Vector3 g_perp = Vector3.Project(g, normal);

    // parallel component: g - g_perp (or ProjectOnPlane)
    Vector3 g_parallel = g - g_perp;
    // or Vector3.ProjectOnPlane(g, normal) -> gives g_parallel

    // visual
    Debug.DrawRay(transform.position, g, Color.blue);          // gravity
    Debug.DrawRay(transform.position, g_perp, Color.red);      // normal comp
    Debug.DrawRay(transform.position, g_parallel, Color.yellow); // parallel comp
}
    
    public void OnDrawGizmos()
    {
        if (A == null || B == null) return;

        Vector3 a = A.position;
        Vector3 b = B.position;
        Vector3 ab = b - a;

        // draw points
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(a, 0.05f);
        Gizmos.DrawSphere(b, 0.05f);

        // draw vector AB
        Debug.DrawLine(a, b, Color.white);

        // normalized, magnitude
        Vector3 an = ab.normalized;
        float mag = ab.magnitude;

        // draw normalized direction scaled
        Debug.DrawRay(a, an * 1.0f, Color.green);

        // dot and cross
        float dot = Vector3.Dot(a.normalized, b.normalized);
        Vector3 cross = Vector3.Cross(a, b);
    }
    
}
