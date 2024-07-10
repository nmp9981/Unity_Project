using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillUIManager : MonoBehaviour
{
    [SerializeField] GameObject skillUI;
    [SerializeField] TextMeshProUGUI skillPointText;

    #region 각 스킬 레벨
    int kinEyesLv;
    int luckySevenLv;
    int masteryLv;
    int boosterLv;
    int criticalLv;
    int hasteLv;
    int mesoUpLv;
    int avengerLv;
    int shadowPartnerLv;
    int tripleThrowLv;

    [SerializeField] TextMeshProUGUI kinEyesLvText;
    [SerializeField] TextMeshProUGUI luckySevenLvText;
    [SerializeField] TextMeshProUGUI masteryLvText;
    [SerializeField] TextMeshProUGUI boosterLvText;
    [SerializeField] TextMeshProUGUI criticalLvText;
    [SerializeField] TextMeshProUGUI hasteLvText;
    [SerializeField] TextMeshProUGUI mesoUpLvText;
    [SerializeField] TextMeshProUGUI avengerLvText;
    [SerializeField] TextMeshProUGUI shadowPartnerLvText;
    [SerializeField] TextMeshProUGUI tripleThrowLvText;
    #endregion

    private void Awake()
    {
        SillLevelInit();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) SkillUIOnOff();
        SkillPointShow();
    }
    void SkillUIOnOff()
    {
        if (skillUI.activeSelf) skillUI.SetActive(false);
        else skillUI.SetActive(true);
    }
    void SkillPointShow()
    {
        skillPointText.text = $"{GameManager.Instance.SkillPoint}";

        kinEyesLvText.text = $"{kinEyesLv}";
        luckySevenLvText.text = $"{luckySevenLv}";
        masteryLvText.text = $"{masteryLv}";
        boosterLvText.text = $"{boosterLv}";
        criticalLvText.text = $"{criticalLv}";
        hasteLvText.text = $"{hasteLv}";
        mesoUpLvText.text = $"{mesoUpLv}";
        avengerLvText.text = $"{avengerLv}";
        shadowPartnerLvText.text = $"{shadowPartnerLv}";
        tripleThrowLvText.text = $"{tripleThrowLv}";
    }
    void SillLevelInit()
    {
        kinEyesLv = 0;
        luckySevenLv = 1;
        masteryLv = 0;
        boosterLv = 0;
        hasteLv = 0;
        criticalLv = 0;
        mesoUpLv = 0;
        avengerLv = 0;
        shadowPartnerLv = 0;
        tripleThrowLv = 0;

        GameManager.Instance.ThrowDist = 500;
        GameManager.Instance.LuckySevenCoefficient = 58;
        GameManager.Instance.Proficiency = 10;
        GameManager.Instance.BoosterTime = 0;
        GameManager.Instance.CriticalRate = 0;
        GameManager.Instance.CriticalDamage = 100;
        GameManager.Instance.HasteTime = 0;
        GameManager.Instance.AddMoveSpeed = 0;
        GameManager.Instance.AddJumpSpeed = 0;
        GameManager.Instance.AddMeso = 0;
        GameManager.Instance.AvengerCoefficient = 0;
        GameManager.Instance.ShadowAttack = 0;
        GameManager.Instance.ShadowTime = 0;
        GameManager.Instance.TripleThrowCoefficient = 0;
    }
    public void KinEyesUp()
    {
        if (GameManager.Instance.SkillPoint > 0 && kinEyesLv<9)
        {
            GameManager.Instance.SkillPoint -= 1;
            kinEyesLv += 1;
            GameManager.Instance.ThrowDist = 550 + kinEyesLv * 50;
        }
    }
    public void LuckkySevenUp()
    {
        if (GameManager.Instance.SkillPoint > 0 && luckySevenLv < 22)
        {
            GameManager.Instance.SkillPoint -= 1;
            luckySevenLv += 1;
            GameManager.Instance.LuckySevenCoefficient = 50 + 5*luckySevenLv;
        }
    }
    public void MasteryUp()
    {
        if (GameManager.Instance.SkillPoint > 0 && masteryLv < 20 && GameManager.Instance.PlayerLV>=20)
        {
            GameManager.Instance.SkillPoint -= 1;
            masteryLv += 1;
            GameManager.Instance.Proficiency = 10 + 5 * (masteryLv+1)/2;
            GameManager.Instance.PlayerAddACC = masteryLv;
        }
    }
}
