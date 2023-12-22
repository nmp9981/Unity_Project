using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace LastWar
{
    public class EnemyAAuthoring : MonoBehaviour
    {
        public int HP = 5;
        class Baker: Baker<EnemyAAuthoring>
        {
            public override void Bake(EnemyAAuthoring authoring)
            {
                //데이터 초기화
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new EnemAObject
                {
                    HP = authoring.HP
                }) ;
                AddComponent<EnemAObject>(entity);
            }
        }

        public struct EnemAObject : IComponentData
        {
            public int HP;
        }
    }

}
