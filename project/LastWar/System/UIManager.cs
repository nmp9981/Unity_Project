using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using static LastWar.BulletAAuthoting;
using static LastWar.ConfigAuthoring;

namespace LastWar
{
    public class UIManager : MonoBehaviour
    {
        [Header("Pivot")]
        [SerializeField] private RectTransform m_Pivot;
        [SerializeField] private float m_Offset;

        [Header("HP")]
        [SerializeField] private Slider playerHP;//플레이어 HP바
        [SerializeField] private TMPro.TextMeshProUGUI textPlayerHP;

        [Header("EXP/LV")]
        [SerializeField] private TMPro.TextMeshProUGUI m_TextLevel;//레벨
        [SerializeField] private Slider m_SliderExpGauge;//경험치 바
        [SerializeField] private TMPro.TextMeshProUGUI m_TextExp;

        public TMPro.TextMeshProUGUI hitDamageText;//피격 데미지

        private Canvas m_Canvas;
        private RectTransform m_Rectransform;


        void Awake()
        {
            m_Canvas = GetComponent<Canvas>();
            m_Rectransform = GetComponent<RectTransform>();
           
            if (playerHP != null) playerHP.value = 0f;
            if (textPlayerHP != null) textPlayerHP.text = string.Empty;
            if (m_TextLevel != null) m_TextLevel.text = string.Empty;
            if (m_SliderExpGauge != null) m_SliderExpGauge.value = 0f;
            if (m_TextExp != null) m_TextExp.text = string.Empty;

            hitDamageText.text = string.Empty;
        }

        //캐릭터의 정보 수정은 액션 마친 뒤 여기서 진행
        void LateUpdate()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var query = entityManager.CreateEntityQuery(typeof(Config),typeof(ECSPlayerData));
            if (!query.IsEmpty)//쿼리를 싱글톤으로
            {
                var entity = query.GetSingletonEntity();
                var configData = entityManager.GetComponentData<Config>(entity);
                var ecsPlayerData = entityManager.GetComponentData<ECSPlayerData>(entity);

                if (m_Pivot != null)//피벗 위치 고정
                {
                    m_Pivot.anchoredPosition = Vector2.zero;
                }

                if (playerHP != null) playerHP.value = (float) ecsPlayerData.HP/ecsPlayerData.maxHP;
                if (textPlayerHP != null) textPlayerHP.text = string.Format("{0}/{1}", ecsPlayerData.HP, ecsPlayerData.maxHP);
                if (m_TextLevel != null) m_TextLevel.text = string.Format("Lv.{0}", ecsPlayerData.Lv);
                if (m_SliderExpGauge != null) m_SliderExpGauge.value = (float)ecsPlayerData.Exp/ecsPlayerData.maxExp;
                if (m_TextExp != null) m_TextExp.text = string.Format("{0}/{1}", ecsPlayerData.Exp, ecsPlayerData.maxExp);
            }

        }
        public void DisplayHitDamage(int damage, float3 mobTransform)
        {
            hitDamageText.transform.position = mobTransform;
            hitDamageText.text = damage.ToString();
            Invoke("EraseHitDamageText",1.5f);
        }
        public void EraseHitDamageText()
        {
            hitDamageText.text = string.Empty;
        }
    }

}
