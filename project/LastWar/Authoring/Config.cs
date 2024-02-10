using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using UnityEngine.UI;
using UnityEditor.PackageManager;

namespace LastWar
{
    public class ConfigAuthoring : MonoBehaviour
    {
        //캐릭터 오브젝트
        public GameObject chracterPrefab;
        //성 오브젝트
        public GameObject castlePrefab;
        //몬스터 오브젝트
        public GameObject EnemyAPrefab;
        public GameObject EnemyBPrefab;
        
        //총알 오브젝트
        public GameObject BulletAPrefab;
        public GameObject BulletBPrefab;

        //텍스트 오브젝트
        public TMPro.TextMeshProUGUI enemyHitDamageText; 

        //스테이지 변수
        public int stageNumber;
        //캐릭터 관련 변수
        public float characterMoveSpeed;
        //총알 관련 변수
        public float bulletAMoveSpeed;
        public float bulletBMoveSpeed;
        //몬스터 관련 변수
        public int spawnMonsterCount;
        public int spawnMonsterMax;//최대 몬스터 스폰 수
        public int spawnMonsterRatio;//몬스터 스폰 비율
        public int monsterSpeed;//몬스터 속도

        //UI 요소
        public TMPro.TextMeshProUGUI damageText;//데미지


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
                    castlePrefab = GetEntity(authoring.castlePrefab, TransformUsageFlags.Dynamic),
                    BulletAPrefab = GetEntity(authoring.BulletAPrefab, TransformUsageFlags.Dynamic),
                    BulletBPrefab = GetEntity(authoring.BulletBPrefab, TransformUsageFlags.Dynamic),
                    enemyHitDamageText = GetEntity(authoring.enemyHitDamageText, TransformUsageFlags.None),
                    stageNumber = authoring.stageNumber,
                    characterMoveSpeed = authoring.characterMoveSpeed,
                    bulletAMoveSpeed = authoring.bulletAMoveSpeed,
                    bulletBMoveSpeed = authoring.bulletBMoveSpeed,
                    spawnMonsterCount = 0,
                    spawnMonsterMax = 8,
                    spawnMonsterRatio = 50,
                    monsterSpeed = authoring.monsterSpeed,
                }) ;
                AddComponent<EnemySpawner>(entity);
                AddComponent<ChatacterSpawner>(entity);
                AddComponent<CastleSpawner>(entity);
                AddComponent<CharacterMove>(entity);
                AddComponent<CharacterAttack>(entity);
                AddComponent<BulletSpawner>(entity);
                AddComponent<EnemyMove>(entity);
            }
        }
        public struct Config : IComponentData
        {
            public Entity chracterPrefab;
            public Entity castlePrefab;
            public Entity EnemyAPrefab;
            public Entity EnemyBPrefab;
            public Entity BulletAPrefab;
            public Entity BulletBPrefab;
            public Entity enemyHitDamageText;

            public int stageNumber;
            public float characterMoveSpeed;
            public float bulletAMoveSpeed;
            public float bulletBMoveSpeed;
            public int spawnMonsterCount;
            public int spawnMonsterMax;
            public int spawnMonsterRatio;
            public int monsterSpeed;

        }

        public struct EnemySpawner : IComponentData
        {

        }
        public struct ChatacterSpawner : IComponentData
        {

        }
        public struct CastleSpawner : IComponentData
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
        public struct EnemyMove : IComponentData
        {

        }
    }

}
