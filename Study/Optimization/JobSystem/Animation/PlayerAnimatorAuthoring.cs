using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace AnimTuto
{
    public struct PlayerGameObjectPrefab : IComponentData
    {
        public Entity playerGameObjectPrefab;
    }
    public class PlayerAnimationReference : ICleanupComponentData
    {
        public Animator Value;
    }

    public class PlayerAnimatorAuthoring : MonoBehaviour
    {
        public GameObject playerGameObjectPrefab;

        public class Baker : Baker<PlayerAnimatorAuthoring>
        {
            public override void Bake(PlayerAnimatorAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new PlayerGameObjectPrefab { 
                    playerGameObjectPrefab = GetEntity(authoring.playerGameObjectPrefab)
                });
                
            }
        }
    }

}

