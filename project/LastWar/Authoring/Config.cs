using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using UnityEditor.PackageManager;

namespace LastWar
{
    public class ConfigAuthoring : MonoBehaviour
    {
        //캐릭터 오브젝트
        public GameObject chracterPrefab;
        //몬스터 오브젝트
        public GameObject EnemyAPrefab;
        public GameObject EnemyBPrefab;
        
        //총알 오브젝트
        public GameObject BulletAPrefab;
        public GameObject BulletBPrefab;

        //캐릭터 관련 변수
        public float characterMoveSpeed;
        //총알 관련 변수
        public float bulletAMoveSpeed;
        public float bulletBMoveSpeed;
        //몬스터 관련 변수
        public int spawnMonsterCount;

        class Baker : Baker<ConfigAuthoring>
        {
            public override void Bake(ConfigAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);//엔티티 화
                AddComponent(entity, new Config
                {
                    EnemyAPrefab = GetEntity(authoring.EnemyAPrefab, TransformUsageFlags.Dynamic),
                    EnemyBPrefab = GetEntity(authoring.EnemyBPrefab, TransformUsageFlags.Dynamic),
                    chracterPrefab = GetEntity(authoring.chracterPrefab, TransformUsageFlags.Dynamic),
                    BulletAPrefab = GetEntity(authoring.BulletAPrefab, TransformUsageFlags.Dynamic),
                    BulletBPrefab = GetEntity(authoring.BulletBPrefab, TransformUsageFlags.Dynamic),
                    characterMoveSpeed = authoring.characterMoveSpeed,
                    bulletAMoveSpeed = authoring.bulletAMoveSpeed,
                    bulletBMoveSpeed = authoring.bulletBMoveSpeed,
                    spawnMonsterCount = 0
                });
                AddComponent<EnemySpawner>(entity);
                AddComponent<ChatacterSpawner>(entity);
                AddComponent<CharacterMove>(entity);
                AddComponent<CharacterAttack>(entity);
                AddComponent<BulletSpawner>(entity);
            }
        }
        public struct Config : IComponentData
        {
            public Entity chracterPrefab;
            public Entity EnemyAPrefab;
            public Entity EnemyBPrefab;
            public Entity BulletAPrefab;
            public Entity BulletBPrefab;
            
            public float characterMoveSpeed;
            public float bulletAMoveSpeed;
            public float bulletBMoveSpeed;
            public int spawnMonsterCount;
        }

        public struct EnemySpawner : IComponentData
        {

        }
        public struct ChatacterSpawner : IComponentData
        {

        }
        public struct CharacterMove : IComponentData
        {

        }
        public struct CharacterAttack : IComponentData
        {

        }
        public struct BulletSpawner : IComponentData
        {

        }
    }

}
