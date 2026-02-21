using UnityEngine;

public class DOF2Rigidbody : MonoBehaviour
{
    float delta;
    float vy,Ux;
    float currentUx,prevUx;
    float a, b,r;
    float h, L, m;
    float FzFront, FzRear, staticFront, staticRear;
    float mu, Cf, Cr;
    float vy_dot, r_dot,Iz;
    Vector3 local;

    float escThreshold = 0.2f;  // rad/s
    float steerThreshold = 0.02f;   // rad
    float minSpeed = 5f;        // m/s
    float escGain = 2500f;
    float maxEscYaw = 4000f;

    CustomRigidBody rb;

    private void Start()
    {
        Vector3 velocity = new Vector3(rb.velocity.x, rb.velocity.y, rb.velocity.z);
        local = transform.InverseTransformDirection(velocity);
        vy = local.y;
        r = rb.angularVelocity.y;
    }
    private void FixedUpdate()
    {
        DOF2Flow(Time.fixedDeltaTime);
    }
    /// <summary>
    /// 28주차 기록용
    /// </summary>
    /// <param name="dt"></param>
    void DOF2Flow(float dt)
    {
        //1.현재 속도 추출
        Vector3 velocity = new Vector3(rb.velocity.x, rb.velocity.y, rb.velocity.z);
        local = transform.InverseTransformDirection(velocity);
        Ux = local.z;
        prevUx = currentUx;
        currentUx = Ux;
        //2.슬립각 계산
        float U = Mathf.Max(Mathf.Abs(Ux), 0.1f);
        float alphaF = delta - (vy + a * r) / U;
        float alphaR = (b * r - vy) / U;
        //3.하중 이동 계산
        float ax = (currentUx - prevUx) / dt;
        float transfer = (h / L) * m * ax;

        FzFront = staticFront + transfer;
        FzFront = MathUtility.Max(FzFront, 0);
        FzRear = staticRear - transfer;
        FzRear = MathUtility.Max(FzRear,0);

        //4.타이어 횡력 계산(비선형)
        float FyF = mu * FzFront * MathUtility.Tanh(Cf * alphaF / (mu * FzFront));
        float FyR = mu * FzRear * MathUtility.Tanh(Cr * alphaR / (mu * FzRear));
        //5.차량 횡력 / 요 모멘트 계산
        vy_dot = (FyF + FyR) / m - U * r;
        r_dot = (a * FyF - b * FyR) / Iz;
        //6.상태 미분 계산
        //7.적분(Euler 또는 RK2)
        vy += vy_dot * dt;
        r += r_dot * dt;
        //8.Rigidbody에 반영
        Vec3 localVel = new Vec3(0, vy, Ux);
        Vector3 vel = transform.TransformDirection(new Vector3(localVel.x, localVel.y, localVel.z));
        rb.velocity = new Vec3(vel.x, vel.y,vel.z);
        rb.angularVelocity = new Vec3(0, r, 0);
    }
    /// <summary>
    /// 29주차 이후
    /// </summary>
    /// <param name="dt"></param>
    void VehicleStep(float dt)
    {
        //종력 계산
        float Fx = throttle * maxDriveForce - brake * maxBrakeForce;

        // 1. 현재 속도 읽기
        Vec3 localVel = transform.InverseTransformDirection(rb.velocity);
        float Ux = localVel.z;
        float Vy = localVel.y;
        float r = rb.angularVelocity.y;

        //구동력
        float FxDriveFront = 0f;
        float FxDriveRear = throttle * maxDriveForce;

        //Brake 6:4분배
        float brakeForce = brake * maxBrakeForce;

        float FxBrakeFront = -0.6f * brakeForce;
        float FxBrakeRear = -0.4f * brakeForce;

        // 2. 슬립각 계산
        float U = Mathf.Max(Mathf.Abs(Ux), 0.1f);
        float alphaF = delta - (Vy + a * r) / U;
        float alphaR = (b * r - Vy) / U;

        // 3. 하중 계산
        float ax = Fx / rb.mass.value; // 또는 힘 기반 계산
        float transfer = (h / L) * Fx;

        float FzF = staticFront + transfer;
        float FzR = staticRear - transfer;
        FzF = MathUtility.Max(0f, FzF);
        FzR = MathUtility.Max(0f, FzR);

        // 4. 횡력
        float FyF = mu * FzF * MathUtility.Tanh(Cf * alphaF / (mu * FzF));
        float FyR = mu * FzR * MathUtility.Tanh(Cr * alphaR / (mu * FzR));

        //후륜 구동 분배
        float FxFront = FxDriveFront + FxBrakeFront;
        float FxRear = FxDriveRear + FxBrakeRear;

        //u-circle 전륜
        float FmaxF = mu * FzF;//원의 반지름
        float magF = MathUtility.Root(FxFront * FxFront + FyF * FyF);
        if (FmaxF > 0.0001f && magF > FmaxF)
        {
            float scale = FmaxF / magF;
            FxFront *= scale;
            FyF *= scale;
        }
        //u-circle 후륜
        float FmaxR = mu * FzR;
        float magR = MathUtility.Root(FxRear * FxRear + FyR * FyR);
        if (FmaxR > 0.0001f && magR > FmaxR)
        {
            float scale = FmaxR / magR;
            FxRear *= scale;
            FyR *= scale;
        }

        //힘 조립
        float FxTotal = FxFront + FxRear;
        float FyTotal = FyF + FyR;

        // 6. Force 변환
        Vec3 forceLocal = new Vec3(0,FyTotal,FxTotal);

        Vec3 forceWorld = transform.TransformDirection(forceLocal);

        rb.AddForce(forceWorld);

        // 7. 요 모멘트
        float r_ref = (Ux / L) * delta;//이론 값 
        float r_error = r - r_ref;//실제 값과의 오차

        bool oversteer = (r - r_ref) > escThreshold && MathUtility.Abs(delta) > steerThreshold && Ux > minSpeed;
        if (oversteer)//오버스티어인 경우
        {
            float escYaw = -escGain * r_error;
            escYaw = MathUtility.ClampValue(escYaw, -maxEscYaw, maxEscYaw);
            rb.AddTorque(new Vec3(0, escYaw, 0));
        }
    }
}
