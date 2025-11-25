using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class VectorBasic : MonoBehaviour
{
    public Transform slope; // any point on slope
    Rigidbody rb;
    Vec3 g = new Vec3(0, -9.81f, 0);

    void Start() => rb = GetComponent<Rigidbody>();

    void FixedUpdate()
    {
        // assume contact normal = slope.up (or use raycast to get actual normal)
        Vec3 normal = new Vec3(slope.position.x, slope.position.y, slope.position.z);

        // perpendicular (normal) component: project g onto normal
        Vec3 g_perp = VectorMathUtils.Project(g, normal);

        // parallel component: g - g_perp (or ProjectOnPlane)
        Vec3 g_parallel = g - g_perp;
    }

    /// <summary>
    /// 2*2행렬 방정식 풀이
    /// </summary>
    (float x, float y) SolveEquipment(float a, float b, float c, float d, float x0, float y0)
    {
        //행렬식
        float det = a * d - b * c;

        //해
        float x = (d * x0 - b * y0)/det;
        float y = (-c * x0 + a * y0)/det;
        return (x, y);
    }
}
