using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace LastWar{
    public class Config : MonoBehaviour
    {
        GameObject EnemyAObj;//적A
        GameObject EnemyBObj;//적B
        public float monsterOffset;//몬스터 간격

        class Baker : Baker<Config>
        {
            public override void Bake(Config authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);//엔티티 화
                AddComponent(entity, new ConfigData
                {
                    monsterOffset = authoring.monsterOffset,
                    EnemyAObj = GetEntity(authoring.EnemyAObj, TransformUsageFlags.Dynamic),
                    EnemyBObj = GetEntity(authoring.EnemyBObj, TransformUsageFlags.Dynamic)
                });

                AddComponent<SpawnSystem>(entity);
            }
        }
        
        public struct ConfigData : IComponentData
        {
            public Entity EnemyAObj;
            public Entity EnemyBObj;
            public float monsterOffset;
        }

        public struct SpawnSystem : IComponentData
        {

        }
    }

}
