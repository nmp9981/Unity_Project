using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace LastWar
{
    public class CastleAuthoring : MonoBehaviour
    {
        class Baker : Baker<CastleAuthoring>
        {
            public override void Bake(CastleAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<CastleObject>(entity);
            }
        }
    }
    public struct CastleObject : IComponentData
    {

    }
}
