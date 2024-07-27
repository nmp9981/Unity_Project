using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillUIManager : MonoBehaviour
{
    [SerializeField] GameObject skillUI;
    [SerializeField] TextMeshProUGUI skillPointText;
    Color activeColor = new Color(204f / 255f, 221f / 255f, 238f / 255f);
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

    [SerializeField] GameObject masteryImage;
    [SerializeField] GameObject boosterImage;
    [SerializeField] GameObject criticalThrowImage;
    [SerializeField] GameObject hasteImage;
    [SerializeField] GameObject mesoUpImage;
    [SerializeField] GameObject avengerImage;
    [SerializeField] GameObject shadowPartnerImage;
    [SerializeField] GameObject triplethrowImage;
    #endregion

    private void Awake()
    {
        SillLevelInit();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) SkillUIOnOff();
        SkillPointShow();
        SkillIconActive();
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
        GameManager.Instance.AddMeso = 100;
        GameManager.Instance.AvengerCoefficient = 0;
        GameManager.Instance.ShadowAttack = 0;
        GameManager.Instance.ShadowTime = 0;
        GameManager.Instance.TripleThrowCoefficient = 0;

        masteryImage.GetComponent<Image>().color = Color.gray;
        boosterImage.GetComponent<Image>().color = Color.gray;
        criticalThrowImage.GetComponent<Image>().color = Color.gray;
        hasteImage.GetComponent<Image>().color = Color.gray;
        mesoUpImage.GetComponent<Image>().color = Color.gray;
        avengerImage.GetComponent<Image>().color = Color.gray;
        shadowPartnerImage.GetComponent<Image>().color = Color.gray;
        triplethrowImage.GetComponent<Image>().color = Color.gray;
    }
    public void KinEyesUp()
    {
        if (GameManager.Instance.SkillPoint > 0 && kinEyesLv<8)
        {
            GameManager.Instance.SkillPoint -= 1;
            kinEyesLv += 1;
            GameManager.Instance.ThrowDist = 550 + kinEyesLv * 50;
        }
    }
    public void LuckkySevenUp()
    {
        if (GameManager.Instance.SkillPoint > 0 && luckySevenLv < 20)
        {
            GameManager.Instance.SkillPoint -= 1;
            luckySevenLv += 1;
            GameManager.Instance.LuckySevenCoefficient = 60 + 5*luckySevenLv;
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
    public void BoosterUp()
    {
        if (GameManager.Instance.SkillPoint > 0 && boosterLv < 20 && masteryLv >= 5)
        {
            GameManager.Instance.SkillPoint -= 1;
            boosterLv += 1;
            GameManager.Instance.BoosterTime = boosterLv * 10;
        }
    }
    public void CriticalUp()
    {
        if (GameManager.Instance.SkillPoint > 0 && criticalLv < 30 && masteryLv >= 3)
        {
            GameManager.Instance.SkillPoint -= 1;
            criticalLv += 1;
            GameManager.Instance.CriticalRate = 20 + criticalLv;
            GameManager.Instance.CriticalDamage = 110+3*criticalLv;
        }
    }
    public void HasteUp()
    {
        if (GameManager.Instance.SkillPoint > 0 && hasteLv < 30 && GameManager.Instance.PlayerLV >= 20)
        {
            GameManager.Instance.SkillPoint -= 1;
            hasteLv += 1;
            GameManager.Instance.AddMoveSpeed = 2 * hasteLv;
            GameManager.Instance.AddJumpSpeed = hasteLv;
            GameManager.Instance.HasteTime = 90+hasteLv * 5;
        }
    }
    public void MesoUpUp()
    {
        if (GameManager.Instance.SkillPoint > 0 && mesoUpLv < 30 && GameManager.Instance.PlayerLV >= 50)
        {
            GameManager.Instance.SkillPoint -= 1;
            mesoUpLv += 1;
            GameManager.Instance.MesoUpTime = 30+mesoUpLv * 7;
            GameManager.Instance.AddMeso = 110 + 3 * mesoUpLv;
        }
    }
    public void ShadowPartnerUp()
    {
        if (GameManager.Instance.SkillPoint > 0 && shadowPartnerLv < 40 && GameManager.Instance.PlayerLV >= 50)
        {
            GameManager.Instance.SkillPoint -= 1;
            shadowPartnerLv += 1;
            GameManager.Instance.ShadowAttack = Mathf.Max(50, 20+shadowPartnerLv);
            GameManager.Instance.ShadowTime = 60 * ((shadowPartnerLv+9) / 10);
        }
    }
    public void AvengerUp()
    {
        if (GameManager.Instance.SkillPoint > 0 && avengerLv < 40 && GameManager.Instance.PlayerLV >= 50)
        {
            GameManager.Instance.SkillPoint -= 1;
            avengerLv += 1;
            GameManager.Instance.AvengerCoefficient = avengerLv<11? 60 + 5 * avengerLv:70+4*avengerLv;
        }
    }
    public void TriplethrowUp()
    {
        if (GameManager.Instance.SkillPoint > 0 && tripleThrowLv < 40 && avengerLv >= 5)
        {
            GameManager.Instance.SkillPoint -= 1;
            tripleThrowLv += 1;
            GameManager.Instance.TripleThrowCoefficient = tripleThrowLv < 31 ? 100 + 2 * tripleThrowLv : 130 + tripleThrowLv;
        }
    }
    void SkillIconActive()
    {
        if(GameManager.Instance.PlayerLV>=20) masteryImage.GetComponent<Image>().color = activeColor;
        if (masteryLv >= 5) boosterImage.GetComponent<Image>().color = activeColor;
        if (masteryLv >= 3) criticalThrowImage.GetComponent<Image>().color = activeColor;
        if (GameManager.Instance.PlayerLV >= 20) hasteImage.GetComponent<Image>().color = activeColor;
        if (GameManager.Instance.PlayerLV >= 50) mesoUpImage.GetComponent<Image>().color = activeColor;
        if (GameManager.Instance.PlayerLV >= 50) avengerImage.GetComponent<Image>().color = activeColor;
        if (GameManager.Instance.PlayerLV >= 50) shadowPartnerImage.GetComponent<Image>().color = activeColor;
        if (avengerLv >= 5) triplethrowImage.GetComponent<Image>().color = activeColor;
    }
}
