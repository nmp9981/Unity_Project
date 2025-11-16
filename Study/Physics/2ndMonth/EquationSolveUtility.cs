using UnityEngine;

/// <summary>
/// 두 스프링 정보
/// </summary>
struct TwoMassSpring
{
    public float m1, m2;//두 물체의 질량
    public Vector3 x1, x2;//두 물체의 위치
    public float k, L;//스프링 상수, 최대 길이
}

public static class EquationSolveUtility
{
    public static void SolveTwoBodySpring(ref TwoMassSpring sys, out Vector3 a1, out Vector3 a2)
    {
        //위치 차이
        Vector3 dir = sys.x2 - sys.x1;
        float dist = dir.magnitude;
        //위치 변화량 0
        if (dist < 1e-4f)
        {
            a1 = a2 = Vector3.zero;
            return;
        }

        //힘의 방향(정규화)
        Vector3 n = dir / dist;
        //훅의 법칙 F=-kx
        float f = sys.k * (dist - sys.L);
        //알짜힘
        Vector3 F = f * n;

        a1 = F / sys.m1;
        a2 = -F / sys.m2;
    }
}
