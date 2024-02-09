using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace LastWar
{
    public class BulletAAuthoting : MonoBehaviour
    {
        public int Atk;
        class Baker : Baker<BulletAAuthoting>
        {
            public override void Bake(BulletAAuthoting authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new BulletAObject
                {
                    Atk = authoring.Atk
                });
                
            }
        }
        public struct BulletAObject : IComponentData
        {
            public int Atk;
        }
    }
}
