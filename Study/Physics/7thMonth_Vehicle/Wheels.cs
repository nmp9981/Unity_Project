using NUnit.Framework;
using UnityEngine;

/// <summary>
/// 바퀴 구조체
/// 차량–지면 인터페이스 상태 묶음(State Container)
/// 휠은 전부 계산용 가상 구조체
/// </summary>
public struct Wheel
{
    // === 고정 파라미터 (설계값) ===
    public Vec3 localPos;        // CoM 기준 휠 위치
    public Vec3 suspensionDir;  // 보통 (0, -1, 0)
    public float radius;        // 바퀴 반지름
    public float restLength;    // 서스펜션 기본 길이

    // === 프레임별 상태값 ===
    public bool isGrounded;      // 접촉 중인가?
    public float currentLength; // 현재 서스펜션 길이

    public Vec3 contactPoint;   // 접촉점
    public Vec3 contactNormal;  // 접촉 법선
}

public class Wheels : MonoBehaviour
{
    //차량 바퀴는 4개
    public Wheel[] wheels = new Wheel[4];

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void WheelRayCast()
    {
        foreach (var wheel in wheels)
        {
            Vec3 rayStart =
                body.WorldCenterOfMass +
                body.LocalToWorldDirection(wheel.localPos);

            Vec3 rayDir =
                body.LocalToWorldDirection(wheel.suspensionDir);

            float rayLength =
                wheel.restLength + wheel.radius;

            if (Raycast(rayStart, rayDir, rayLength, out hit))
            {
                wheel.isGrounded = true;
                wheel.contactPoint = hit.point;
                wheel.contactNormal = hit.normal;
                wheel.currentLength = hit.distance - wheel.radius;
            }
            else
            {
                wheel.isGrounded = false;
            }
        }
    }
}
