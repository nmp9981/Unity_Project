using UnityEngine;

/// <summary>
/// 두 스프링 정보
/// A--B
/// </summary>
public struct TwoMassSpring
{
    public float m1, m2;//두 물체의 질량
    public Vector3 x1, x2;//두 물체의 위치
    public float k, L;//스프링 상수, 최대 길이
}

/// <summary>
/// 세 스프링 정보
/// A---B---C
/// </summary>
public struct ThreeMassSpring
{
    public float m1, m2, m3;//세 물체의 질량
    public Vector3 x1, x2, x3;//세 물체의 위치
    public float k1, k2;//두 스프링의 스프링 상수
}


public static class EquationSolveUtility
{
    /// <summary>
    /// 두 스프링 사이에 작용하는 가속도
    /// </summary>
    /// <param name="sys"></param>
    /// <param name="a1">물체1의 가속도</param>
    /// <param name="a2">물체2의 가속도</param>
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

    /// <summary>
    /// 세 스프링 사이에 작용하는 가속도
    /// </summary>
    /// <param name="sys"></param>
    /// <param name="a1">물체1의 가속도</param>
    /// <param name="a2">물체2의 가속도</param>
    /// <param name="a3">물체3의 가속도</param>
    public static void SolveThreeBodySpring(ref ThreeMassSpring sys, out Vector3 a1, out Vector3 a2, out Vector3 a3)
    {
        //위치 차이
        Vector3 dirAB = sys.x2 - sys.x1;
        Vector3 dirBC = sys.x3 - sys.x2;

        //알짜힘
        Vector3 F1 = sys.k1*dirAB;
        Vector3 F2 = sys.k2*dirBC-sys.k1*dirAB;
        Vector3 F3 = -sys.k2*dirBC;

        a1 = F1 / sys.m1;
        a2 = F2 / sys.m2;
        a3 = F3 / sys.m3;
    }
}
