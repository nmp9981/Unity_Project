using UnityEngine;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using static SimpleDefence.MovementAuthoring;

namespace SimpleDefence
{
    public partial struct MonsterMovementSystem : ISystem
    {
        int mobCount;
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MonsterMovement>();
            state.RequireForUpdate<Config>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var config = SystemAPI.GetSingleton<Config>();
            
            //회전
            float deltaTime = SystemAPI.Time.DeltaTime;
            
            foreach (var (transform, speed) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<Monster>>())
            {
                transform.ValueRW = transform.ValueRO.RotateY(speed.ValueRO.RadiansPerSecond * deltaTime);
            }


            //이동
            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");
            var moveAmount = new float3(0, 0, 1) * SystemAPI.Time.DeltaTime * config.monsterMoveSpeed;


            if (moveAmount.Equals(float3.zero)) return;//이동을 안함

            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            //각 개체마다 이동
            foreach (var (transform,entity) in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<Monster>().WithEntityAccess())
            {
                transform.ValueRW.Position += moveAmount;
                if (transform.ValueRO.Position.z > 80)
                {
                    ecb.DestroyEntity(entity);

                    mobCount++;
                    Debug.Log(mobCount);
                    //defenseUI.GetComponent<DefenceUI>().monsterCount++;

                }
            }

        }
    }
   
}
