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
        coolTime = 8.0f;
        nowTime = 7.0f;
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        //if (config.spawnMonsterCount > 0) return;

        var rand = new Unity.Mathematics.Random(100);//랜덤 시드
        foreach (var configEntity in SystemAPI.Query<RefRW<Config>>())
        {
            if (configEntity.ValueRO.spawnMonsterCount >= 0)
            {
                nowTime+=Time.deltaTime;
                if (nowTime < coolTime) continue;//쿨타임이 안옴

                int minSpawn = config.spawnMonsterMax * config.spawnMonsterRatio / 100;
                for (int i = -UnityEngine.Random.Range(minSpawn,config.spawnMonsterMax); i <= UnityEngine.Random.Range(minSpawn,config.spawnMonsterMax); i++)
                {
                    int spawnRand = UnityEngine.Random.Range(1, 13);
                    switch (spawnRand%2)
                    {
                        case 0:
                            var monster = state.EntityManager.Instantiate(config.EnemyAPrefab);//몬스터 생성
                                                                                               //몬스터 생성 위치 : 캐릭터의 위치
                            state.EntityManager.SetComponentData(monster, new LocalTransform
                            {
                                Position = new float3
                                {
                                    x = i * UnityEngine.Random.Range(0.8f, 2.0f),
                                    y = 0.5f,
                                    z = UnityEngine.Random.Range(2.0f, 7.0f)
                                },
                                Scale = 1,
                                Rotation = quaternion.RotateX(-90)
                            }) ;
                            break;
                        case 1:
                            var monster2 = state.EntityManager.Instantiate(config.EnemyBPrefab);//몬스터 생성
                                                                                               //몬스터 생성 위치 : 캐릭터의 위치
                            state.EntityManager.SetComponentData(monster2, new LocalTransform
                            {
                                Position = new float3
                                {
                                    x = i * UnityEngine.Random.Range(1.0f, 2.0f),
                                    y = 0.5f,
                                    z = UnityEngine.Random.Range(2.0f, 7.0f)
                                },
                                Scale = 1,
                                Rotation = quaternion.RotateX(-90)
                            });
                            break;
                    }
                    
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
