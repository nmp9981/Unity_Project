using UnityEngine;

public class VectorBasic : MonoBehaviour
{
    Transform A;
    Transform B;
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
