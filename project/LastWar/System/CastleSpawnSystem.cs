using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static LastWar.ConfigAuthoring;

namespace LastWar
{
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial struct CastleSpawnSystem : ISystem
    {
        int castleSpawnCount;
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Config>();
            state.RequireForUpdate<CastleSpawner>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (castleSpawnCount > 0) return;//이미 생성
            var config = SystemAPI.GetSingleton<Config>();
            var castle = state.EntityManager.Instantiate(config.castlePrefab);//성벽 생성

            state.EntityManager.SetComponentData(castle, new LocalTransform
            {
                Position = new float3
                {
                    x = 0,
                    y = 0f,
                    z = -15
                },
                Scale = 1f,
                Rotation = quaternion.identity
            });
            castleSpawnCount++;
        }
    }

}
