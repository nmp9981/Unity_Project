using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace LastWar
{
    public class BulletBAuthoring : MonoBehaviour
    {
        class Baker : Baker<BulletBAuthoring>
        {
            public override void Bake(BulletBAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<BulletBObject>(entity);
            }
        }
        
    }
    public struct BulletBObject : IComponentData
    {

    }
}
