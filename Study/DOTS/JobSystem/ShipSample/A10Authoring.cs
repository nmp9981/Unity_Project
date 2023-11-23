using Unity.Entities;
using UnityEngine;
using static SimpleDefence.MovementAuthoring;

namespace ShipExample
{
    public class A10Authoring : MonoBehaviour
    {
        public bool isClickObject = false;
        class Baker : Baker<A10Authoring>
        {
            public override void Bake(A10Authoring authoring)
            {
                //데이터 초기화
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new A10Object
                {
                    isClick = authoring.isClickObject
                }); 
                AddComponent<A10Object>(entity);
            }
        }

        public struct A10Object : IComponentData
        {
            public bool isClick;
        }
    }
}
