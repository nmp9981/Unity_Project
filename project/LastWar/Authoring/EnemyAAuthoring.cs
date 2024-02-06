using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace LastWar
{
    public class EnemyAAuthoring : MonoBehaviour
    {
        public int HP;
        public int Exp;
        public GameObject hpBar;
        class Baker : Baker<EnemyAAuthoring>
        {
            public override void Bake(EnemyAAuthoring authoring)
            {
                //데이터 초기화
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new EnemyAObject
                {
                    HP = authoring.HP,
                    Exp = authoring.Exp,
                    hpBar = GetEntity(authoring.hpBar, TransformUsageFlags.None)
                });
            }
        }
        
        public struct EnemyAObject : IComponentData
        {
            public int HP;
            public int Exp;
            public Entity hpBar;
        }
    }

}
