/// <summary>
/// 상태 정의
/// </summary>
public struct VehicleState
{
    public float Ux;   // longitudinal velocity
    public float Vy;   // lateral velocity
    public float r;    // yaw rate
}

/// <summary>
/// 입력 정의
/// </summary>
public struct VehicleInput
{
    public float throttle;  // 0~1
    public float brake;     // 0~1
    public float steering;  // radians
}

public class VehicleDynamicsCore
{
    // 차량 파라미터
    float mass = 1500f;
    float Iz = 2500f;
    float a = 1.2f;
    float b = 1.6f;

    float rho = 1.225f;
    float Cd = 0.3f;
    float A = 2.2f;
    float Crr = 0.015f;

    float maxDriveForce = 4000f;
    float maxBrakeForce = 8000f;

    public VehicleState state;

    public void Step(VehicleInput input, float dt)
    {
        float delta = input.steering;

        // 1️⃣ 종력 계산
        float FxEngine = input.throttle * maxDriveForce;
        float FxBrake = input.brake * maxBrakeForce;
        float FxTire = FxEngine - FxBrake;

        // 2️⃣ 저항
        float Fdrag = 0.5f * rho * Cd * A * state.Ux * state.Ux;
        float Froll = Crr * mass * 9.81f;

        // 3️⃣ 횡력 (임시 선형 모델)
        float Cf = 80000f;
        float Cr = 80000f;

        float alphaF = (state.Vy + a * state.r) / MathUtility.Max(state.Ux, 0.1f) - delta;
        float alphaR = (state.Vy - b * state.r) / MathUtility.Max(state.Ux, 0.1f);

        float Fyf = -Cf * alphaF;
        float Fyr = -Cr * alphaR;

        // 4️⃣ 종방향
        float FxTotal =
            FxTire
            - Fdrag
            - Froll
            + (-Fyf * MathUtility.Sin(delta));

        float Ux_dot = FxTotal / mass;

        // 5️⃣ 횡방향
        float Vy_dot =
            (Fyf * MathUtility.Cos(delta) + Fyr) / mass
            - state.Ux * state.r;

        // 6️⃣ 요 운동
        float r_dot =
            (a * Fyf * MathUtility.Cos(delta)
            - b * Fyr) / Iz;

        // 7️⃣ 적분
        state.Ux += Ux_dot * dt;
        state.Vy += Vy_dot * dt;
        state.r += r_dot * dt;
    }
    /// <summary>
    /// 종력 계산 함수
    /// </summary>
    /// <param name="throttle"></param>
    /// <param name="brake"></param>
    /// <returns></returns>
    float ComputeLongitudinalForce(float throttle, float brake)
    {
        float FxEngine = throttle * maxDriveForce;
        float FxBrake = brake * maxBrakeForce;
        return FxEngine - FxBrake;
    }
    /// <summary>
    /// 저항 계산
    /// </summary>
    /// <param name="Ux"></param>
    /// <returns></returns>
    float ComputeDrag(float Ux)
    {
        return 0.5f * rho * Cd * A * Ux * Ux;
    }

    float ComputeRollingResistance()
    {
        return Crr * mass * 9.81f;
    }
}
