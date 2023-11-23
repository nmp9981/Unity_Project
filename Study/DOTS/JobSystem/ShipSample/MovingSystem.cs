using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static ShipExample.A10Authoring;
using static ShipExample.A20Authoring;
using static ShipExample.A30Authoring;
using static ShipExample.C30Authoring;

namespace ShipExample
{
    public partial struct MovingSystem : ISystem
    {
        int mobCount;
       
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Config>();
            state.RequireForUpdate<DM_MP_A10_Spawner>();
            state.RequireForUpdate<DM_MP_A20_Spawner>();
            state.RequireForUpdate<DM_MP_A30_Spawner>();
            state.RequireForUpdate<DM_MP_C30_Spawner>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var config = SystemAPI.GetSingleton<Config>();

            //이동
            float horiaontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            var moveAmount = new float3(horiaontal, vertical, 0) * SystemAPI.Time.DeltaTime*config.movingSpeed;

            if (moveAmount.Equals(float3.zero)) return;//이동을 안함

            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);


            var query = SystemAPI.QueryBuilder().WithAll<A10Object>().Build();

            //각 개체마다 이동
            foreach (var (transform, entity) in SystemAPI.Query<RefRW<LocalTransform>,RefRW<A10Object>>())//해당 엔티티의 변수 접근
            {
                entity.ValueRW.isClick = true;
                transform.ValueRW.Position += moveAmount;//이동 후 위치

            }
            
            foreach (var (transform, entity) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<A20Object>>())
            {
                entity.ValueRW.isClick = true;
                transform.ValueRW.Position += moveAmount;//이동 후 위치

            }
            foreach (var (transform, entity) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<A30Object>>())
            {
                entity.ValueRW.isClick = true;
                transform.ValueRW.Position += moveAmount;//이동 후 위치

            }
            foreach (var (transform, entity) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<C30Object>>())
            {
                entity.ValueRW.isClick = true;
                transform.ValueRW.Position += moveAmount;//이동 후 위치

            }
        }
    }
}
