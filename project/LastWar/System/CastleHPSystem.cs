using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.SceneManagement;
using static LastWar.CharacterAuthoring;
using static LastWar.ConfigAuthoring;
using static LastWar.EnemyAAuthoring;
using static LastWar.EnemyBAuthoring;
using static UnityEngine.EventSystems.EventTrigger;

namespace LastWar
{
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial struct CastleHPSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Config>();
            state.RequireForUpdate<CastleObject>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var config = SystemAPI.GetSingleton<Config>();
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            float minDistSq = 3;
            foreach (var castleTransform in
                    SystemAPI.Query<RefRW<LocalTransform>>()
                        .WithAll<CastleObject>())//성벽 위치
            {
                
                //적들 정보
                foreach (var (enemyTransform, enemyInfo, EnemyEntity) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<EnemyAObject>>().WithAll<EnemyAObject>().WithEntityAccess())
                {
                    
                    //적에 닿으면 파괴
                    if (math.distancesq(enemyTransform.ValueRO.Position.z, castleTransform.ValueRO.Position.z) <= minDistSq)
                    {
                        //피격 데미지
                        foreach (var playData in SystemAPI.Query<RefRW<ECSPlayerData>>())
                        {
                            playData.ValueRW.HP -= enemyInfo.ValueRO.Attack;
                            //사망 처리
                            if(playData.ValueRO.HP <= 0)
                            {
                                SceneManager.LoadScene("GameOver");
                            }
                            break;
                        }
                        ecb.DestroyEntity(EnemyEntity);//적 파괴
                        break;
                    }
                }
                
                foreach (var (enemyTransform, enemyInfo, EnemyBEntity) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<EnemyBObject>>().WithAll<EnemyBObject>().WithEntityAccess())
                {
                    //적에 닿으면 파괴
                    if (math.distancesq(enemyTransform.ValueRO.Position.z, castleTransform.ValueRO.Position.z) <= minDistSq)
                    {
                        //피격 데미지
                        foreach (var playData in SystemAPI.Query<RefRW<ECSPlayerData>>())
                        {
                            playData.ValueRW.HP -= enemyInfo.ValueRO.Attack;
                            //사망 처리
                            if (playData.ValueRO.HP <= 0)
                            {
                                SceneManager.LoadScene("GameOver");
                            }
                            break;
                        }
                        ecb.DestroyEntity(EnemyBEntity);//적 파괴
                        break;
                    }
                }
            }
        }
    }
}
