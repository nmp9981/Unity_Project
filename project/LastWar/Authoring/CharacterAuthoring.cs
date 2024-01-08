using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static LastWar.EnemyBAuthoring;

namespace LastWar
{
    public class CharacterAuthoring : MonoBehaviour
    {
        class Baker : Baker<CharacterAuthoring>
        {
            public override void Bake(CharacterAuthoring authoring)
            {
                //데이터 초기화
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<CharacterObject>(entity);
            }
        }
        public struct CharacterObject : IComponentData
        {

        }
    }

}
