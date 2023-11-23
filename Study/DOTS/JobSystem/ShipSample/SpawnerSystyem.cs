using ShipExample;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static ShipExample.A10Authoring;

namespace ShipExample
{
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial struct SpawnerSystem : ISystem
    {

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
            if (Input.GetKeyDown(KeyCode.Alpha1)) ResenShipObject(config, ref state,1);
            else if(Input.GetKeyDown(KeyCode.Alpha2)) ResenShipObject(config, ref state, 2);
            else if (Input.GetKeyDown(KeyCode.Alpha3)) ResenShipObject(config, ref state, 3);
            else if (Input.GetKeyDown(KeyCode.Alpha4)) ResenShipObject(config, ref state, 4);
            /*
            for (int i = 0; i <= config.regenSize; i++)
            {
                var A10Obj = state.EntityManager.Instantiate(config.DM_MP_A10_Piece);//몬스터 생성
                                                                                     //몬스터 생성 위치 : 캐릭터의 위치
                state.EntityManager.SetComponentData(A10Obj, new LocalTransform
                {
                    Position = new float3
                    {
                        x = i * config.regenOffset,
                        y = 10f,
                        z = -config.regenOffset
                    },
                    Scale = 1,
                    Rotation = quaternion.identity
                });
            }
            */
        }

        void ResenShipObject(Config config, ref SystemState state,int keyInput)
        {
            switch (keyInput)
            {
                case 1:
                    var resenObj = state.EntityManager.Instantiate(config.DM_MP_A10_Piece);
                    state.EntityManager.SetComponentData(resenObj, new LocalTransform
                    {
                        Position = new float3
                        {
                            x = config.regenOffset,
                            y = 10f,
                            z = config.regenOffset
                        },
                        Scale = 1,
                        Rotation = quaternion.identity
                    });
                    break;
                case 2:
                    var resenObj2 = state.EntityManager.Instantiate(config.DM_MP_A20_Piece);
                    state.EntityManager.SetComponentData(resenObj2, new LocalTransform
                    {
                        Position = new float3
                        {
                            x = config.regenOffset,
                            y = -10f,
                            z = config.regenOffset
                        },
                        Scale = 1,
                        Rotation = quaternion.identity
                    });
                    break;
                case 3:
                    var resenObj3 = state.EntityManager.Instantiate(config.DM_MP_A30_Piece);
                    state.EntityManager.SetComponentData(resenObj3, new LocalTransform
                    {
                        Position = new float3
                        {
                            x = -config.regenOffset,
                            y = 10f,
                            z = config.regenOffset
                        },
                        Scale = 1,
                        Rotation = quaternion.identity
                    });
                    break;
                case 4:
                    var resenObj4 = state.EntityManager.Instantiate(config.DM_MP_C30_Piece);
                    state.EntityManager.SetComponentData(resenObj4, new LocalTransform
                    {
                        Position = new float3
                        {
                            x = -config.regenOffset,
                            y = -10f,
                            z = config.regenOffset
                        },
                        Scale = 1,
                        Rotation = quaternion.identity
                    });
                    break;
            }
        }
    }
}


