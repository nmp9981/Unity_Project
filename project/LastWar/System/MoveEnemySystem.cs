using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static LastWar.CharacterAuthoring;
using static LastWar.ConfigAuthoring;
using static LastWar.EnemyAAuthoring;
using static LastWar.EnemyBAuthoring;
using static UnityEngine.EventSystems.EventTrigger;

namespace LastWar
{
    public partial struct MoveEnemySystem : ISystem    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Config>();
            state.RequireForUpdate<EnemyMove>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var config = SystemAPI.GetSingleton<Config>();
            
            float moveSpeed = (float) config.monsterSpeed / 99;
            foreach (var playData in SystemAPI.Query<RefRW<ECSPlayerData>>())
            {
                moveSpeed += playData.ValueRO.Lv/55.0f;
            }
            var moveAmount = new float3(0, 0, -moveSpeed) * SystemAPI.Time.DeltaTime;

            foreach (var playerTransform in
                     SystemAPI.Query<RefRW<LocalTransform>>()
                         .WithAll<EnemyAObject>())
            {
                playerTransform.ValueRW.Position += moveAmount;//이동 후 위치

            }
            foreach (var playerTransform in
                     SystemAPI.Query<RefRW<LocalTransform>>()
                         .WithAll<EnemyBObject>())
            {
                playerTransform.ValueRW.Position += moveAmount;//이동 후 위치
            }
        }
    }

}
