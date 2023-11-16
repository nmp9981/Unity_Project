using UnityEngine;
using SimpleDefence;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateBefore(typeof(TransformSystemGroup))]
public partial struct MoxsterSpawnSystem : ISystem
{

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        state.RequireForUpdate<MonsterSpawner>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        //state.Enabled = false; => 이거때문에 입력을 못받는 거였다.

        var config = SystemAPI.GetSingleton<Config>();


        if (!Input.GetKeyDown(KeyCode.Space)) return; 
        
        var rand = new Unity.Mathematics.Random(100);//랜덤 시드

        //생성
        for (int i = 0; i < UnityEngine.Random.Range(2,config.monsterNumRows); i++)
        {
            for (int j = 0; j < UnityEngine.Random.Range(2, config.monsterNumCols); j++)
            {
                var monster = state.EntityManager.Instantiate(config.MonsterPrefab);//몬스터 생성
                
                //몬스터 생성 위치
                state.EntityManager.SetComponentData(monster, new LocalTransform
                {
                    Position = new float3
                    {
                        x = (i * config.monsterOffset) + rand.NextFloat(config.monsterOffset),
                        y = 0,
                        z = (j * config.monsterOffset) + rand.NextFloat(config.monsterOffset)
                    },
                    Scale = 1,
                    Rotation = quaternion.identity
                });
            }
        }

    }
}

