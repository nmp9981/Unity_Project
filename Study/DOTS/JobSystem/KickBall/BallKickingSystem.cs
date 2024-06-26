using Tutorials.Kickball.Execute;
using Tutorials.Kickball.Step1;
using Tutorials.Kickball.Step2;
using Tutorials.Kickball.Step3;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Tutorials.Kickball.Step5
{
    // UpdateBefore BallMovementSystem so that the ball movement is affected by a kick in the same frame.
    [UpdateBefore(typeof(BallMovementSystem))]
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial struct BallKickingSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BallKicking>();
            state.RequireForUpdate<Config>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var config = SystemAPI.GetSingleton<Config>();

            if (!Input.GetKeyDown(KeyCode.Space))
            {
                return;
            }

            // For every player, add an impact velocity to every ball in kicking range.
            foreach (var playerTransform in
                     SystemAPI.Query<RefRO<LocalTransform>>()
                         .WithAll<Player>())
            {
                foreach (var (ballTransform, velocity) in
                         SystemAPI.Query<RefRO<LocalTransform>, RefRW<Velocity>>()
                             .WithAll<Ball>())
                {
                    float distSQ = math.distancesq(playerTransform.ValueRO.Position, ballTransform.ValueRO.Position);//플레이어 공 간 거리 계산

                    if (distSQ <= config.BallKickingRangeSQ)//범위 내
                    {
                        var playerToBall = ballTransform.ValueRO.Position.xz - playerTransform.ValueRO.Position.xz;//방향
                        
                        velocity.ValueRW.Value += math.normalizesafe(playerToBall) * config.BallKickForce;
                    }
                }
            }
        }
    }
}
