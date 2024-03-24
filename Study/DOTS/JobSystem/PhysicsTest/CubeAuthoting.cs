using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static LastWar.ConfigAuthoring;

namespace CollisionTest
{
    public class CubeAuthoting : MonoBehaviour
    {
        public GameObject cubePrefab;
        public int cubeCount;
        public int spawnRange;

        class Baker : Baker<CubeAuthoting>
        {
            public override void Bake(CubeAuthoting authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);//엔티티 화
                AddComponent(entity, new ConfigCube()
                {
                    cubePrefab = GetEntity(authoring.cubePrefab, TransformUsageFlags.Dynamic),
                    cubeCount = authoring.cubeCount,
                    spawnRange = authoring.spawnRange
                });
                AddComponent<CubeSpawnSystem>(entity);
                AddComponent<CollisionSystem>(entity);
            }
        }
        public struct ConfigCube : IComponentData
        {
            public Entity cubePrefab;
            public int cubeCount;
            public int spawnRange;
        }

        public struct CubeSpawnSystem : IComponentData
        {

        }
        public struct CollisionSystem : IComponentData
        {

        }
    }

}
