using UnityEngine;

public class BicycleModel2DOF : MonoBehaviour
{
    // ===== 차량 파라미터 =====
    public float m = 1500f;      // mass
    public float Iz = 2500f;     // yaw inertia
    public float a = 1.2f;       // CG → front
    public float b = 1.6f;       // CG → rear
    public float Cf = 80000f;    // front cornering stiffness
    public float Cr = 80000f;    // rear cornering stiffness

    public float U = 15f;        // longitudinal speed (m/s)

    // ===== 상태변수 =====
    public float beta;   // side slip angle
    public float r;      // yaw rate

    public float steerInput; // δ (rad)

    private void Awake()
    {
        steerInput = 5 * MathUtility.Deg2Rad;
    }

    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        StepModel(dt);
    }

    void StepModel(float dt)
    {
        float mU = m * U;
        float IzU = Iz * U;

        float A11 = -(Cf + Cr) / mU;
        float A12 = (-a * Cf + b * Cr) / mU - U;

        float A21 = (-a * Cf + b * Cr) / IzU;
        float A22 = -(a * a * Cf + b * b * Cr) / IzU;

        float B1 = Cf / mU;
        float B2 = a * Cf / Iz;

        // k1
        float k1_beta = A11 * beta + A12 * r + B1 * steerInput;
        float k1_r = A21 * beta + A22 * r + B2 * steerInput;

        float beta_mid = beta + k1_beta * dt * 0.5f;
        float r_mid = r + k1_r * dt * 0.5f;

        // k2
        float k2_beta = A11 * beta_mid + A12 * r_mid + B1 * steerInput;
        float k2_r = A21 * beta_mid + A22 * r_mid + B2 * steerInput;

        beta += k2_beta * dt;
        r += k2_r * dt;
    }
}
