using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static LastWar.BulletAAuthoting;
using static LastWar.CharacterAuthoring;
using static LastWar.ConfigAuthoring;

namespace LastWar
{
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial struct BulletSpawnSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Config>();
            state.RequireForUpdate<BulletSpawner>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var config = SystemAPI.GetSingleton<Config>();

            //총알 생성  : 스페이스 바
            if (!Input.GetKeyDown(KeyCode.Space)) return;

            //총을 몇발 쏠지
            int rowShotCount = 0;
            int colShotCount = 0;
            foreach (var playData in SystemAPI.Query<RefRW<ECSPlayerData>>())
            {
                playData.ValueRW.stepCount = math.min(6, playData.ValueRO.Lv-79);
                playData.ValueRW.widthCount = 1;
                rowShotCount = playData.ValueRO.widthCount;
                colShotCount = playData.ValueRO.stepCount;
            }

            //캐릭터의 위치에 소환
            for (int i = 0; i < colShotCount; i++)
            {
                for(int j = 0; j < 1; j++)
                {
                    foreach (var (transform, entity) in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<CharacterObject>().WithEntityAccess())
                    {
                        var bullet = state.EntityManager.Instantiate(config.BulletAPrefab);
                        state.EntityManager.SetComponentData(bullet, new LocalTransform
                        {
                            Position = new float3
                            {
                                x = transform.ValueRO.Position.x + j,
                                y = 0.5f,
                                z = transform.ValueRO.Position.z+i*0.35f
                            },
                            Scale = 0.3f,
                            Rotation = Quaternion.identity
                        });
                    }
                }
            }
            
        }
    }

}
