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
 
    void Update()
    {
        ShowPlayerUI();
    }
    void ShowPlayerUI()
    {
        playerLVText.text = $"Lv. {GameManager.Instance.PlayerLV}";

        playerHPText.text = $"HP. {GameManager.Instance.PlayerHP} / {GameManager.Instance.PlayerMaxHP}";
        playerHPFill.fillAmount = (float)GameManager.Instance.PlayerHP / (float)GameManager.Instance.PlayerMaxHP;

        playerMPText.text = $"MP. {GameManager.Instance.PlayerMP} / {GameManager.Instance.PlayerMaxMP}";
        playerMPFill.fillAmount = (float)GameManager.Instance.PlayerMP / (float)GameManager.Instance.PlayerMaxMP;

        playerExpText.text = "EXP. "+GameManager.Instance.PlayerEXP.ToString()+" / "+GameManager.Instance.PlayerReqExp.ToString();
        playerExpFill.fillAmount = (float)GameManager.Instance.PlayerEXP / (float) GameManager.Instance.PlayerReqExp;
    }
}
