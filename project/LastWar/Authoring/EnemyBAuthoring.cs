using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using static LastWar.EnemyAAuthoring;

namespace LastWar
{
    public class EnemyBAuthoring : MonoBehaviour
    {
        public int HP;
        public int Exp;
        public GameObject hpBar;
        class Baker : Baker<EnemyBAuthoring>
        {
            public override void Bake(EnemyBAuthoring authoring)
            {
                //데이터 초기화
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new EnemyBObject
                {
                    HP = authoring.HP,
                    Exp = authoring.Exp,
                    hpBar = GetEntity(authoring.hpBar, TransformUsageFlags.None)
                });
            }
        }

        public struct EnemyBObject : IComponentData
        {
            public int HP;
            public int Exp;
            public Entity hpBar;
        }
    }

}
