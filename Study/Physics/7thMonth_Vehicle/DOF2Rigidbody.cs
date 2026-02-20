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

        // 2. 슬립각 계산
        float U = Mathf.Max(Mathf.Abs(Ux), 0.1f);
        float alphaF = delta - (Vy + a * r) / U;
        float alphaR = (b * r - Vy) / U;

        // 3. 하중 계산
        float ax = Fx / rb.mass.value; // 또는 힘 기반 계산
        float transfer = (h * rb.mass.value / L) * ax;

        float FzF = staticFront + transfer;
        float FzR = staticRear - transfer;

        // 4. 횡력
        float FyF = mu * FzF * MathUtility.Tanh(Cf * alphaF / (mu * FzF));
        float FyR = mu * FzR * MathUtility.Tanh(Cr * alphaR / (mu * FzR));

        //후륜 구동 분배
        float FxFront = 0f;              // 전륜구동 아니면 0
        float FxRear = Fx;              // 후륜구동 가정

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
        float yawMoment = a * FyF - b * FyR;
        rb.AddTorque(new Vec3(0, yawMoment, 0));
    }
}
