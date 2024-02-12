using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;
using static LastWar.BulletAAuthoting;
using static LastWar.CharacterAuthoring;
using static LastWar.ConfigAuthoring;

namespace LastWar
{
    public class UIManager : MonoBehaviour
    {
        [Header("HP")]
        [SerializeField] private Slider playerHP;//플레이어 HP바
        [SerializeField] private TMPro.TextMeshProUGUI textPlayerHP;

        [Header("EXP/LV")]
        [SerializeField] private TMPro.TextMeshProUGUI m_TextLevel;//레벨
        [SerializeField] private Slider m_SliderExpGauge;//경험치 바
        [SerializeField] private TMPro.TextMeshProUGUI m_TextExp;

        //결과
        public static int resultLv;
        public static int resultExp;
        public static int resultMaxExp;
        void Awake()
        {

            playerHP.value = 1f;
            textPlayerHP.text = string.Empty;
            m_TextLevel.text = string.Empty;
            m_SliderExpGauge.value = 0f;
            m_TextExp.text = string.Empty;

        }

        //캐릭터의 정보 수정은 액션 마친 뒤 여기서 진행
        void LateUpdate()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var query = entityManager.CreateEntityQuery(typeof(ECSPlayerData));
            var entity = query.GetSingletonEntity();
            var ecsPlayerData = entityManager.GetComponentData<ECSPlayerData>(entity);

            playerHP.value = (float)ecsPlayerData.HP / ecsPlayerData.maxHP;
            textPlayerHP.text = string.Format("HP {0}/{1}", ecsPlayerData.HP, ecsPlayerData.maxHP);
            m_TextLevel.text = string.Format("Lv.{0}", ecsPlayerData.Lv);
            m_SliderExpGauge.value = (float)ecsPlayerData.Exp / ecsPlayerData.maxExp;
            m_TextExp.text = string.Format("EXP {0}/{1}", ecsPlayerData.Exp, ecsPlayerData.maxExp);

            resultLv = ecsPlayerData.Lv;
            resultExp = ecsPlayerData.Exp;
            resultMaxExp = ecsPlayerData.maxExp;
        }
    }
}
