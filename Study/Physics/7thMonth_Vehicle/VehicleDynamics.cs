using UnityEngine;

/// <summary>
/// 선형 Bicycle Model 기반 이론 검증용 동역학 계산기
/// 기존 VehicleRigidBody와 연결하지 않음
/// </summary>
public class VehicleDynamics : MonoBehaviour 
{
    [Header("Basic Vehicle Parameters")]
    public float mass = 1500f;          // kg
    public float wheelBase = 2.6f;      // m
    public float a = 1.2f;              // CoM -> Front axle
    public float b = 1.4f;              // CoM -> Rear axle
    public float yawInertia = 2500f;    // kg*m^2

    [Header("Tire Cornering Stiffness")]
    public float Cf = 80000f;           // Front cornering stiffness (N/rad)
    public float Cr = 90000f;           // Rear cornering stiffness (N/rad)

    [Header("Input State")]
    public float speed = 15f;           // m/s
    public float steerAngleDeg = 5f;    // degrees

    [ContextMenu("Run Vehicle Dynamics Debug")]
    public void RunDynamics()
    {
        float delta = steerAngleDeg * Mathf.Deg2Rad;
        float V = Mathf.Max(speed, 0.1f); // 0 나눗셈 방지

        // ==============================
        // 1️⃣ Understeer Gradient
        // Ku = (m / (Cf * Cr * L)) * (bCr - aCf)
        // ==============================

        float Ku = (mass / (Cf * Cr * wheelBase)) * (b * Cr - a * Cf);

        // ==============================
        // 2️⃣ Steady-State Yaw Rate
        // r = V / (L + Ku V^2) * delta
        // ==============================

        float yawRate = (V / (wheelBase + Ku * V * V)) * delta;

        // ==============================
        // 3️⃣ Lateral Acceleration
        // ay = V * r
        // ==============================

        float lateralAccel = V * yawRate;

        // ==============================
        // 4️⃣ Slip Angles (이론적)
        // alpha_f = delta - (a r / V)
        // alpha_r = - (b r / V)
        // ==============================

        float alphaF = delta - (a * yawRate / V);
        float alphaR = -(b * yawRate / V);

        // ==============================
        // 5️⃣ Tire Lateral Forces
        // Fyf = -Cf * alphaF
        // Fyr = -Cr * alphaR
        // ==============================

        float Fyf = -Cf * alphaF;
        float Fyr = -Cr * alphaR;

        Debug.Log(
            "===== Vehicle Dynamics Debug =====\n" +
            $"Speed: {V:F2} m/s\n" +
            $"Steer Angle: {steerAngleDeg:F2} deg\n\n" +

            $"Understeer Gradient (Ku): {Ku:F6}\n\n" +

            $"Yaw Rate (r): {yawRate:F4} rad/s\n" +
            $"Lateral Accel (ay): {lateralAccel:F4} m/s^2\n\n" +

            $"Front Slip Angle: {alphaF * Mathf.Rad2Deg:F3} deg\n" +
            $"Rear Slip Angle: {alphaR * Mathf.Rad2Deg:F3} deg\n\n" +

            $"Front Lateral Force: {Fyf:F2} N\n" +
            $"Rear Lateral Force: {Fyr:F2} N\n" +

            "==================================="
        );
    }
}
