using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static LastWar.EnemyBAuthoring;

namespace LastWar
{
    public class CharacterAuthoring : MonoBehaviour
    {
        public int stepCount;//몇발
        public int widthCount;//가로 몇발
        public int Attack;

        public int HP;
        public int maxHP;

        public int Lv;
        public int Exp;
        public int maxExp;
        class Baker : Baker<CharacterAuthoring>
        {
            public override void Bake(CharacterAuthoring authoring)
            {
                //데이터 초기화
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<CharacterObject>(entity);
                AddComponent(entity, new ECSPlayerData()
                {
                    stepCount = 1,
                    widthCount = 1,
                    Attack = 1,
                    HP = authoring.HP,
                    maxHP = authoring.maxHP,
                    Lv = authoring.Lv,
                    Exp = authoring.Exp,
                    maxExp = authoring.maxExp
                }) ;
            }
        }
        public struct CharacterObject : IComponentData
        {
           
        }
        public struct ECSPlayerData : IComponentData
        {
            public int stepCount;//몇발
            public int widthCount;//가로 몇발
            public int Attack;

            public int HP;
            public int maxHP;

            public int Lv;
            public int Exp;
            public int maxExp;
        }
    }

}
