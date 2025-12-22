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

public class IslandSleepSystem
{
    public static void UpdateSleeping(List<Island> islands)
    {
        foreach (var island in islands)
        {
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
    /// Sleep 해제 로직 진입
    /// </summary>
    /// <param name="island"></param>
    static void TryWake(Island island)
    {
        foreach (var body in island.bodies)
        {
            //외력이 들어옴
            if (body.hasExternalForce)
            {
                WakeIsland(island);
                return;
            }
        }

        // 새 contact / manifold 변화
        foreach (var manifold in island.manifolds)
        {
            //새로운 접촉점
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
