using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI playerLVText;
    [SerializeField] TextMeshProUGUI playerHPText;
    [SerializeField] TextMeshProUGUI playerMPText;
    [SerializeField] TextMeshProUGUI playerExpText;

    [SerializeField] Image playerHPFill;
    [SerializeField] Image playerMPFill;
    [SerializeField] Image playerExpFill;

    [SerializeField] TextMeshProUGUI curHPPosionCount;
    [SerializeField] TextMeshProUGUI curMPPosionCount;

    [Header ("Stat")] 
    [SerializeField] GameObject statUI;
    [SerializeField] TextMeshProUGUI statJobText;
    [SerializeField] TextMeshProUGUI statLvText;
    [SerializeField] TextMeshProUGUI statHPText;
    [SerializeField] TextMeshProUGUI statMPText;
    [SerializeField] TextMeshProUGUI statEXPText;
    [SerializeField] TextMeshProUGUI statAttackText;
    [SerializeField] TextMeshProUGUI statDEXText;
    [SerializeField] TextMeshProUGUI statLUKText;
    [SerializeField] TextMeshProUGUI statACCText;

    void Update()
    {
        ShowPlayerUI();
        ShowStatUI();
        StatUIInfo();
    }
    void ShowPlayerUI()
    {
        playerLVText.text = $"Lv. {GameManager.Instance.PlayerLV}";

        playerHPText.text = $"HP. {GameManager.Instance.PlayerHP} / {GameManager.Instance.PlayerMaxHP}";
        playerHPFill.fillAmount = (float)GameManager.Instance.PlayerHP / (float)GameManager.Instance.PlayerMaxHP;

        playerMPText.text = $"MP. {GameManager.Instance.PlayerMP} / {GameManager.Instance.PlayerMaxMP}";
        playerMPFill.fillAmount = (float)GameManager.Instance.PlayerMP / (float)GameManager.Instance.PlayerMaxMP;

        float expRate = (float)GameManager.Instance.PlayerEXP*100 / (float)GameManager.Instance.PlayerReqExp;
        playerExpText.text = string.Format("Exp. {0} / {1} [{2:N2}%]", GameManager.Instance.PlayerEXP, GameManager.Instance.PlayerReqExp, expRate);
        playerExpFill.fillAmount = (float)GameManager.Instance.PlayerEXP / (float) GameManager.Instance.PlayerReqExp;

        curHPPosionCount.text = $"{GameManager.Instance.HPPosionCount}";
        curMPPosionCount.text = $"{GameManager.Instance.MPPosionCount}";
    }
    void ShowStatUI()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (statUI.activeSelf) statUI.SetActive(false);
            else statUI.SetActive(true);
        }
    }
    void StatUIInfo()
    {
        statJobText.text = $"{GameManager.Instance.PlayerJob}";
        statLvText.text = $"{GameManager.Instance.PlayerLV}";
        statHPText.text = $"{GameManager.Instance.PlayerHP} / {GameManager.Instance.PlayerMaxHP}";
        statMPText.text = $"{GameManager.Instance.PlayerMP} / {GameManager.Instance.PlayerMaxMP}";
        statEXPText.text = $"{GameManager.Instance.PlayerEXP} [{GameManager.Instance.PlayerEXP*100/ GameManager.Instance.PlayerReqExp}%]";
        statAttackText.text = $"{GameManager.Instance.PlayerAttack}";
        statDEXText.text = $"{GameManager.Instance.PlayerDEX}";
        statLUKText.text = $"{GameManager.Instance.PlayerLUK}";
        statACCText.text = $"{GameManager.Instance.PlayerACC}";
    }
}
