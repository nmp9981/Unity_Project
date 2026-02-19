using NUnit.Framework;
using System.Data;
using UnityEngine;

public class DOF2Rigidbody : MonoBehaviour
{
    float delta;
    float vy,ax;
    float a, b, r;
    float Ux;
    float h, L, m;
    float FzFront, FzRear, staticFront, staticRear;
    float mu, Cf, Cr;
    float vy_dot, r_dot,Iz;

    CustomRigidBody rb;
   
    private void FixedUpdate()
    {
        DOF2Flow(Time.fixedDeltaTime);
    }
    void DOF2Flow(float dt)
    {
        //1.현재 속도 추출
        //2.슬립각 계산
        float alphaF = delta - (vy + a * r) / Ux;
        float alphaR = (b * r - vy) / Ux;
        //3.하중 이동 계산
        float transfer = (h / L) * m * ax;

        FzFront = staticFront + transfer;
        FzRear = staticRear - transfer;

        //4.타이어 횡력 계산(비선형)
        float FyF = mu * FzFront * MathUtility.Tanh(Cf * alphaF / (mu * FzFront));
        float FyR = mu * FzRear * MathUtility.Tanh(Cr * alphaF / (mu * FzRear));
        //5.차량 횡력 / 요 모멘트 계산
        vy_dot = (FyF + FyR) / m - Ux * r;
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
