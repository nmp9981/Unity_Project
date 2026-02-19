using System.Data;
using UnityEngine;
using UnityEngine.SocialPlatforms;

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
}
