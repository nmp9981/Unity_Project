using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using static CollisionTest.CubeAuthoting;
using static CollisionTest.CubeObjectAuthring;
using static LastWar.BulletAAuthoting;
using static LastWar.CharacterAuthoring;
using static LastWar.ConfigAuthoring;
using static LastWar.EnemyAAuthoring;
using static UnityEngine.EventSystems.EventTrigger;
using Unity.Rendering;
using Unity.Physics;
using LastWar;
using Unity.Physics.Systems;

namespace CollisionTest
{
    [UpdateInGroup(typeof(AfterPhysicsSystemGroup))]//물리 충돌 이후 
    public partial struct CollisionSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {

            state.RequireForUpdate<ConfigCube>();
            state.RequireForUpdate<CubeObj>();
        }
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            /*
            var configCube = SystemAPI.GetSingleton<ConfigCube>();
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            var minDistSq = 14;
          
            foreach (var (transform, color) in
                SystemAPI.Query<RefRO<LocalTransform>,
                                RefRW<URPMaterialPropertyBaseColor>>()
                                .WithAll<CubeObj>())//성벽 위치
            {
                foreach (var (otherTransform, otherColor) in
                 SystemAPI.Query<RefRO<LocalTransform>,
                                 RefRW<URPMaterialPropertyBaseColor>>()
                                 .WithAll<CubeObj>())
                {
                    UnityEngine.Debug.Log(math.distancesq(otherTransform.ValueRO.Position, transform.ValueRO.Position));
                    if (math.distancesq(otherTransform.ValueRO.Position, transform.ValueRO.Position) <= minDistSq)
                    {
                        color.ValueRW.Value = math.float4(1, 0, 0, 1);
                        otherColor.ValueRW.Value = math.float4(1, 0, 0, 1);
                    }
                    else
                    {
                        color.ValueRW.Value = math.float4(0, 0, 0, 1);
                        otherColor.ValueRW.Value = math.float4(0, 0, 0, 1);
                    }
                    break;
                }
            }
             */
            
            // Raycast를 하기 위해 PhysicsWorldSingleton이 필요하다.
            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

            foreach (var (transform, color) in
                SystemAPI.Query<RefRO<LocalTransform>,
                                RefRW<URPMaterialPropertyBaseColor>>()
                                .WithAll<CubeObj>())
            {
                UnityEngine.Debug.Log("11");
                // Ray 생성
                var ray = new RaycastInput()
                {
                    Start = transform.ValueRO.TransformPoint(math.float3(0f, 0f, 1f)),
                    End = transform.ValueRO.TransformPoint(math.float3(0f, 0f, 9f)),
                    Filter = CollisionFilter.Default
                };

                // Raycast
                if (physicsWorld.CastRay(ray, out var hit))
                {
                    // Entity의 Material 색상 변경
                    color.ValueRW.Value = math.float4(1, 0, 0, 1);
                }
                else
                {
                    color.ValueRW.Value = math.float4(0, 0, 1, 1);
                }
            }
            
        }

    }

}
