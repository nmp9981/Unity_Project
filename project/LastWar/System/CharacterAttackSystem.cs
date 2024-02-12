using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;
using TMPro.EditorUtilities;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using static LastWar.BulletAAuthoting;
using static LastWar.CharacterAuthoring;
using static LastWar.ConfigAuthoring;
using static LastWar.EnemyAAuthoring;
using static LastWar.EnemyBAuthoring;

namespace LastWar
{
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial struct CharacterAttackSystem : ISystem { 
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            
            state.RequireForUpdate<Config>();
            state.RequireForUpdate<CharacterAttack>();
            state.RequireForUpdate<EnemyAObject>();
            //uiManager = new UIManager();//UI클래스
           
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var config = SystemAPI.GetSingleton<Config>();
            //var playerInfo = SystemAPI.GetSingleton<ECSPlayerData>();
            
            //var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            //이동
            var movement = new float3(0, 0, 1) * SystemAPI.Time.DeltaTime * config.bulletAMoveSpeed;
            if (movement.Equals(float3.zero)) return;//이동을 안함

            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            var minDistSq = 2;

            //각 개체마다 이동 : Enemy A
            foreach(var(transform,bulletInfo, entity)in SystemAPI.Query<RefRW<LocalTransform>, RefRW<BulletAObject>>().WithAll<BulletAObject>().WithEntityAccess())
            {
                transform.ValueRW.Position += movement;//이동 후 위치

                //적의 위치
                foreach (var (enemyTransform, enemyInfo, EnemyEntity) in SystemAPI.Query<RefRW<LocalTransform >, RefRW < EnemyAObject >> ().WithAll<EnemyAObject>().WithEntityAccess())
                {
                    //적에 닿으면 파괴
                    if (math.distancesq(enemyTransform.ValueRO.Position, transform.ValueRO.Position) <= minDistSq)
                    {
                        //피격 데미지
                        int playerAttack = 0;
                        foreach (var playData in SystemAPI.Query<RefRW<ECSPlayerData>>())
                        {
                            playerAttack = playData.ValueRO.Attack;//여기서 값이 변경
                        }
                       
                        enemyInfo.ValueRW.HP = enemyInfo.ValueRO.HP - (bulletInfo.ValueRO.Atk+playerAttack);//총알 공격력 만큼 데미지

                        if (enemyInfo.ValueRO.HP <= 0)
                        {
                            ecb.DestroyEntity(EnemyEntity);//적 파괴
                            //값 변경하기 , OnUpdate문에서 작성하는 것이어서 프레임마다 1씩 증가
                            foreach (var playData in SystemAPI.Query<RefRW<ECSPlayerData>>())
                            {
                                playData.ValueRW.Exp += enemyInfo.ValueRO.Exp;//여기서 값이 변경
                            }
                            
                            
                            //몬스터 수 감소
                            foreach (var configEntity in SystemAPI.Query<RefRW<Config>>())
                            {
                                configEntity.ValueRW.spawnMonsterCount -= 1;
                            }
                        }
                        
                        ecb.DestroyEntity(entity);//총알 파괴
                        break;
                    }
                }
                foreach (var (enemyTransform, enemyInfo, EnemyEntity) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<EnemyBObject>>().WithAll<EnemyBObject>().WithEntityAccess())
                {
                    //적에 닿으면 파괴
                    if (math.distancesq(enemyTransform.ValueRO.Position, transform.ValueRO.Position) <= minDistSq)
                    {
                        enemyInfo.ValueRW.HP = enemyInfo.ValueRO.HP - bulletInfo.ValueRO.Atk;//총알 공격력 만큼 데미지
                        if (enemyInfo.ValueRO.HP <= 0)
                        {
                            ecb.DestroyEntity(EnemyEntity);//적 파괴
                            foreach (var playData in SystemAPI.Query<RefRW<ECSPlayerData>>())
                            {
                                playData.ValueRW.Exp += enemyInfo.ValueRO.Exp;//여기서 값이 변경
                            }
                            //몬스터 수 감소
                            foreach (var configEntity in SystemAPI.Query<RefRW<Config>>())
                            {
                                configEntity.ValueRW.spawnMonsterCount -= 1;
                            }
                        }

                        ecb.DestroyEntity(entity);//총알 파괴
                        break;
                    }
                }
                /*
                //적의 위치
                foreach (var (enemyTransform, EnemyEntity) in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<EnemyAObject>().WithEntityAccess())
                {
                    //적에 닿으면 파괴
                    if(math.distancesq(enemyTransform.ValueRO.Position, transform.ValueRO.Position) <= minDistSq)
                    {
                        foreach(var obj in SystemAPI.Query<RefRW<EnemyAObject>>())
                        {
                            obj.ValueRW.HP = obj.ValueRO.HP-1;
                            if(obj.ValueRO.HP <=0) ecb.DestroyEntity(EnemyEntity);//적 파괴
                        }

                        ecb.DestroyEntity(entity);//총알 파괴
                        break;
                    }
                }
                */
                //레벨업
                foreach (var playData in SystemAPI.Query<RefRW<ECSPlayerData>>())
                {
                    if(playData.ValueRO.Exp >= playData.ValueRO.maxExp)
                    {
                        //캐릭터 강화
                        playData.ValueRW.Lv += 1;
                        playData.ValueRW.Exp = playData.ValueRO.Exp - playData.ValueRO.maxExp;
                        playData.ValueRW.maxExp = (playData.ValueRO.maxExp+ playData.ValueRO.Lv) * 11 / 10;
                        playData.ValueRW.maxHP = (playData.ValueRO.maxHP+ playData.ValueRO.Lv*5) * 105 / 100;
                        playData.ValueRW.HP = playData.ValueRO.maxHP;
                        playData.ValueRW.Attack += (1+ playData.ValueRO.Lv/5);
                        
                        //적 강화
                        foreach (var (enemyTransform, enemyInfo, EnemyEntity) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<EnemyAObject>>().WithAll<EnemyAObject>().WithEntityAccess())
                        {
                            enemyInfo.ValueRW.Attack = 2*playData.ValueRO.Lv;
                            enemyInfo.ValueRW.HP = 5+playData.ValueRO.Lv* (playData.ValueRO.Lv/2);
                            enemyInfo.ValueRW.Exp = 4+3*playData.ValueRO.Lv/2+ playData.ValueRO.Lv;
                        }
                        foreach (var (enemyTransform, enemyInfo, EnemyEntity) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<EnemyBObject>>().WithAll<EnemyBObject>().WithEntityAccess())
                        {
                            enemyInfo.ValueRW.Attack = 3 *playData.ValueRO.Lv;
                            enemyInfo.ValueRW.HP = 15+3*playData.ValueRO.Lv* (playData.ValueRO.Lv/4);
                            enemyInfo.ValueRW.Exp = 5 + 2 *playData.ValueRO.Lv+ 3*(playData.ValueRO.Lv/2);
                        }
                    }
                }
                //파괴
                if (transform.ValueRW.Position.z > 150)
                {
                    ecb.DestroyEntity(entity);
                    break;
                }
            }
        }
    }

}
