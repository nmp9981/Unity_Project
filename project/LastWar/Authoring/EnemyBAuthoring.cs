using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace LastWar
{
    public class EnemyBAuthoring : MonoBehaviour
    {
        public int HP = 8;
        public int Exp = 4;
        class Baker : Baker<EnemyBAuthoring>
        {
            public override void Bake(EnemyBAuthoring authoring)
            {
                //데이터 초기화
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new EnemyBObject
                {
                    HP = authoring.HP,
                    Exp = authoring.Exp
                }) ;
                AddComponent<EnemyBObject>(entity);
            }
        }

        public struct EnemyBObject : IComponentData
        {
            public int HP;
            public int Exp;
        }
    }
}
