using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using static ShipExample.A10Authoring;
using static ShipExample.A20Authoring;
using static ShipExample.A30Authoring;
using static ShipExample.C30Authoring;


namespace ShipExample
{
    public partial struct RaycastSystem : ISystem
    {
        int ClickFlag;
        public void OnCreate(ref SystemState state)
        {
           
        }

        public void OnDestroy(ref SystemState state)
        {
        }
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (!Input.GetMouseButton(0)) return;//마우스 우클릭
        
            // 레이캐스트를 사용하기 위해 PysicsWorldSingleton이 필요하다.
            PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();


            //Physics.Raycast rayStart = Camera.main.ScreenPointToRay(Input.mousePosition);
            foreach (var (transform, color, entity) in SystemAPI.Query<RefRO<LocalTransform>,RefRW<URPMaterialPropertyBaseColor>,RefRW<C30Object>>())//해당 엔티티의 변수 접근
            {
                var ray = new RaycastInput()
                {
                    Start = transform.ValueRO.TransformPoint(Input.mousePosition),
                    End = transform.ValueRO.TransformPoint(Input.mousePosition+Vector3.forward*100),
                    Filter = new CollisionFilter
                    {
                        GroupIndex = 0,

                        BelongsTo = 1u << 0,
                        CollidesWith = 1u<<1
                    }
                };

                // 레이캐스트 발사 후 hit가 존재하면 if 실행
                if (physicsWorld.CastRay(ray, out var hit))
                {
                    if (ClickFlag % 2 == 0)//클릭시 색상 및 오브젝트 상태 변경
                    {
                        entity.ValueRW.isClick = true;
                        color.ValueRW.Value = new float4(1, 0, 0, 1);
                    }
                    else
                    {
                        entity.ValueRW.isClick = false;
                        color.ValueRW.Value = new float4(0, 0, 0, 1);
                    }
                    ClickFlag++;
                }
                
            }
        }
    }
}
