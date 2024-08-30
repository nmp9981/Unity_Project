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
    [SerializeField] TextMeshProUGUI curAttackUPCount;
    [SerializeField] TextMeshProUGUI curAccUPCount;
    [SerializeField] TextMeshProUGUI curAvoidUPCount;
    [SerializeField] TextMeshProUGUI curReturnToVillegeCount;

    [Header("Message")]
    [SerializeField] List<TextMeshProUGUI> messageList;

    [Header ("Stat")] 
    [SerializeField] GameObject statUI;
    [SerializeField] TextMeshProUGUI statJobText;
    [SerializeField] TextMeshProUGUI statLvText;
    [SerializeField] TextMeshProUGUI statHPText;
    [SerializeField] TextMeshProUGUI statMPText;
    [SerializeField] TextMeshProUGUI statEXPText;
    [SerializeField] TextMeshProUGUI statApPoint;
    [SerializeField] TextMeshProUGUI statAttackText;
    [SerializeField] TextMeshProUGUI statDEXText;
    [SerializeField] TextMeshProUGUI statLUKText;
    [SerializeField] TextMeshProUGUI statACCText;

    [Header("Item")]
    [SerializeField] GameObject itemUI;
    [SerializeField] TextMeshProUGUI playerMesoText;

    [Header("MiniMap")]
    [SerializeField] GameObject minimapObj;
    [SerializeField] Camera minimapCamera;

    [Header("Sound")]
    [SerializeField] GameObject SoundImageObj;
    [SerializeField] Sprite SoundOnImage;
    [SerializeField] Sprite SoundOffImage;
    void Update()
    {
        ShowPlayerUI();
        ShowUI();
        StatUIInfo();
        ItemUIUInfo();
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
        curAttackUPCount.text = $"{GameManager.Instance.AttackUPCount}";
        curAccUPCount.text = $"{GameManager.Instance.AccUPCount}";
        curAvoidUPCount.text = $"{GameManager.Instance.AvoidUPCount}";
        curReturnToVillegeCount.text = $"{GameManager.Instance.ReturnVillegeCount}";
    }
    void ShowUI()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (statUI.activeSelf) statUI.SetActive(false);
            else statUI.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (itemUI.activeSelf) itemUI.SetActive(false);
            else itemUI.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.M)){
            if (minimapObj.activeSelf)
            {
                minimapObj.SetActive(false);
                minimapCamera.gameObject.SetActive(false);
            }
            else
            {
                minimapObj.SetActive(true);
                minimapCamera.gameObject.SetActive(true);
            }
        }
    }
    void StatUIInfo()
    {
        statJobText.text = $"{GameManager.Instance.PlayerJob}";
        statLvText.text = $"{GameManager.Instance.PlayerLV}";
        statHPText.text = $"{GameManager.Instance.PlayerHP} / {GameManager.Instance.PlayerMaxHP}";
        statMPText.text = $"{GameManager.Instance.PlayerMP} / {GameManager.Instance.PlayerMaxMP}";
        statEXPText.text = $"{GameManager.Instance.PlayerEXP} [{GameManager.Instance.PlayerEXP*100/ GameManager.Instance.PlayerReqExp}%]";

        statApPoint.text = $"{GameManager.Instance.ApPoint}";

        statAttackText.text = $"{GameManager.Instance.PlayerAttack}";
        statDEXText.text = $"{GameManager.Instance.PlayerDEX}";
        statLUKText.text = $"{GameManager.Instance.PlayerLUK}";
        statACCText.text = $"{GameManager.Instance.PlayerACC}";
    }
    void ItemUIUInfo()
    {
        playerMesoText.text = string.Format("{0:#,0}", GameManager.Instance.PlayerMeso);
    }
    public void ShowGetText(string type, int amount)
    {
        for(int i = 0; i < messageList.Count; i++)
        {
            if (messageList[i].text == "")
            {
                StartCoroutine(ShowGetTextCor(type, amount, i));
                return;
            }
        }
        StartCoroutine(ShowGetTextCor(type, amount, 0));
    }
    IEnumerator ShowGetTextCor(string type, int amount, int idx)
    {
        messageList[idx].text = $"Get {type} +{amount}";
        yield return new WaitForSeconds(0.75f);
        messageList[idx].text = "";
    }
    public void SoundOnOff()
    {
        GameManager.Instance.BGMVolume = 1f- GameManager.Instance.BGMVolume;
        GameManager.Instance.SFXVolume = 1f - GameManager.Instance.SFXVolume;

        if (GameManager.Instance.BGMVolume == 1) SoundImageObj.GetComponent<Image>().sprite = SoundOnImage;
        else SoundImageObj.GetComponent<Image>().sprite = SoundOffImage;
    }
}
