using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using LastWar;
using static LastWar.ConfigAuthoring;
using static LastWar.EnemyAAuthoring;

[UpdateBefore(typeof(TransformSystemGroup))]
public partial struct EnemySpawnerSystem : ISystem
{
    int spawnCount;
    float coolTime;//몬스터 생성 쿨타임
    float nowTime;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        state.RequireForUpdate<EnemySpawner>();
        coolTime = 3.0f;
        nowTime = 0f;
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        if (config.spawnMonsterCount > 0) return;
        
        foreach (var configEntity in SystemAPI.Query<RefRW<Config>>())
        {
            if (configEntity.ValueRO.spawnMonsterCount == 0)
            {
                nowTime+=Time.deltaTime;
                if (nowTime < coolTime) continue;//쿨타임이 안옴

                for (int i = -4; i <= 4; i++)
                {
                    var monster = state.EntityManager.Instantiate(config.EnemyAPrefab);//몬스터 생성
                                                                                       //몬스터 생성 위치 : 캐릭터의 위치
                    state.EntityManager.SetComponentData(monster, new LocalTransform
                    {
                        Position = new float3
                        {
                            x = i,
                            y = 0.5f,
                            z = i * 0.5f
                        },
                        Scale = 1,
                        Rotation = quaternion.identity
                    });
                    configEntity.ValueRW.spawnMonsterCount += 1;
                }
                nowTime = 0;//시간 초기화
            }
        }
        /*
        for (int i = -4; i <= 4; i++)
        {
            var monster = state.EntityManager.Instantiate(config.EnemyAPrefab);//몬스터 생성
                                                                               //몬스터 생성 위치 : 캐릭터의 위치
            state.EntityManager.SetComponentData(monster, new LocalTransform
            {
                Position = new float3
                {
                    x = i,
                    y = 0.5f,
                    z = i * 0.5f
                },
                Scale = 1,
                Rotation = quaternion.identity
            });
            foreach(var configEntity in SystemAPI.Query<RefRW<Config>>())
            {
                configEntity.ValueRW.spawnMonsterCount += 1;
            }
        }
        spawnCount++;
       */
    }
}
