using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static LastWar.BulletAAuthoting;
using static LastWar.ConfigAuthoring;
using static LastWar.EnemyAAuthoring;

namespace LastWar
{
    public partial struct CharacterAttackSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Config>();
            state.RequireForUpdate<CharacterAttack>();
            state.RequireForUpdate<EnemyAObject>();
        }
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var config = SystemAPI.GetSingleton<Config>();

            //이동
            var movement = new float3(0, 0, 1) * SystemAPI.Time.DeltaTime * config.bulletAMoveSpeed;
            if (movement.Equals(float3.zero)) return;//이동을 안함

            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            var minDistSq = 3;

            //각 개체마다 이동
            foreach(var(transform, entity)in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<BulletAObject>().WithEntityAccess())
            {
                transform.ValueRW.Position += movement;//이동 후 위치

                //적의 위치
                foreach (var (enemyTransform, enemyInfo, EnemyEntity) in SystemAPI.Query<RefRW<LocalTransform >, RefRW < EnemyAObject >> ().WithAll<EnemyAObject>().WithEntityAccess())
                {
                    //적에 닿으면 파괴
                    if (math.distancesq(enemyTransform.ValueRO.Position, transform.ValueRO.Position) <= minDistSq)
                    {
                        enemyInfo.ValueRW.HP = enemyInfo.ValueRO.HP - 1;
                        if (enemyInfo.ValueRO.HP <= 0) ecb.DestroyEntity(EnemyEntity);//적 파괴
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
