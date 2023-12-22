using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using static LastWar.Config;
using static LastWar.EnemyAAuthoring;

namespace LastWar
{
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial struct SpawnSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ConfigData>();
            state.RequireForUpdate<EnemyAAuthoring>();
            state.RequireForUpdate<EnemyBAuthoring>();
        }
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var config = SystemAPI.GetSingleton<ConfigData>();
            for (int i = 0; i <= config.monsterOffset; i++)
            {
                var EnemyA = state.EntityManager.Instantiate(config.EnemyAObj);//몬스터 생성
                                                                                     //몬스터 생성 위치 : 캐릭터의 위치
                state.EntityManager.SetComponentData(EnemyA, new LocalTransform
                {
                    Position = new float3
                    {
                        x = i * config.monsterOffset,
                        y = 10f,
                        z = -config.monsterOffset
                    },
                    Scale = 1,
                    Rotation = quaternion.identity
                });
            }
            
        }
    }

}

