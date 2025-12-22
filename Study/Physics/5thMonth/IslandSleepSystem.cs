using System;
using System.Collections.Generic;

static class SleepConfig
{
    //속도, 각속도 epsilon
    public const float linearVelEps = 0.01f * 0.01f; // 제곱값
    public const float angularVelEps = 0.01f * 0.01f;

    //충격량, 플에미 epsilon
    public const float impulseEps = 0.001f;
    public const int sleepFrames = 60; // 1초 (60Hz)
}
/// <summary>
/// 다음 프레임을 계산할 가치가 있는가?
/// </summary>
public class IslandSleepSystem
{
    /// <summary>
    /// sleeping 판정
    /// </summary>
    /// <param name="islands"></param>
    public static void UpdateSleeping(List<Island> islands)
    {
        foreach (var island in islands)
        {
            //자면 깨워보기, 깨면 재워보기
            if (island.isSleeping)
                TryWake(island);
            else
                TrySleep(island);
        }
    }

    /// <summary>
    /// Sleep 판정 함수
    /// deltaImpulse : 이전 프레임 - 이번 프레임
    /// </summary>
    /// <param name="island"></param>
    /// <returns></returns>
    public static bool CanSleep(Island island)
    {
        // 1. Body 안정성
        foreach (var body in island.bodies)
        {
            //이동함
            if (body.velocity.Square > SleepConfig.linearVelEps)
                return false;

            //회전함
            if (body.angularVelocity.Square > SleepConfig.angularVelEps)
                return false;

            //외력이 있는가?
            if (body.hasExternalForce) // 네가 이미 쓰고 있는 구조
                return false;
        }

        // 2. Contact 안정성 (Impulse 변화량)
        foreach (var manifold in island.manifolds)
        {
            foreach (var cp in manifold.points)
            {
                //노말, 접선 모두 변화가 없는가?
                //매 프레임 변하고 있다면 아직 수렴중이므로 잠들면 안됨
                if (Math.Abs(cp.deltaNormalImpulse) > SleepConfig.impulseEps)
                    return false;

                if (Math.Abs(cp.deltaTangentImpulse) > SleepConfig.impulseEps)
                    return false;
            }
        }
        return true;
    }
    /// <summary>
    /// Sleep 진입 로직
    /// </summary>
    /// <param name="island"></param>
    static void TrySleep(Island island)
    {
        //Sleep진입 조건 미달
        if (!CanSleep(island))
        {
            island.sleepCounter = 0;
            return;
        }

        //수치 오차로 잠깐 멈춘 것과 물리적으로 안정된 상태를 구분
        //오차로 잠깐 멈춘거일수도 있으므로 일정 프레임을 기다린다.
        island.sleepCounter++;
        if (island.sleepCounter < SleepConfig.sleepFrames)
            return;

        //Sleeping 상태
        island.isSleeping = true;

        foreach (var body in island.bodies)
        {
            //찐 속도, 각속도 정지
            body.isSleepling = true;
            body.velocity = VectorMathUtils.ZeroVector3D();
            body.angularVelocity = VectorMathUtils.ZeroVector3D();
        }
    }
    /// <summary>
    /// Sleep 해제 로직 진입 -> 해제를 위한 조건 검사
    /// </summary>
    /// <param name="island"></param>
    static void TryWake(Island island)
    {
        foreach (var body in island.bodies)
        {
            //외력이 들어옴 -> 더 이상 sleep 상태가 아님
            if (body.hasExternalForce)
            {
                WakeIsland(island);
                return;
            }
        }

        // 새 contact / manifold 변화
        foreach (var manifold in island.manifolds)
        {
            //새로운 접촉점 -> 더 이상 sleep 상태가 아님
            // ex,박스가 새로 닿는 경우
            if (manifold.hasNewContact) // UpdateManifolds에서 설정
            {
                WakeIsland(island);
                return;
            }
        }
    }
    /// <summary>
    /// Sleep 해제 -> 여기서 상태 해제
    /// </summary>
    /// <param name="island"></param>
    static void WakeIsland(Island island)
    {
        island.isSleeping = false;
        island.sleepCounter = 0;

        foreach (var body in island.bodies)
            body.isSleepling = false;
    }
}
