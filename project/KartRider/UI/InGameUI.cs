using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI velocityText;
    [SerializeField]
    TextMeshProUGUI distText;
   
    Image gageAmountUI;
    private void Awake()
    {
        gageAmountUI = GameObject.Find("GageAmountImage").GetComponent<Image>();
    }
    void Update()
    {
        ShowVelocityText();
        ShowDistText();
        BoosterGageUI();
    }
    void ShowVelocityText()
    {
        float velocityValue = Mathf.Round(GameManager.Instance.CarSpeed);
        velocityText.text = $"{velocityValue} km/h";
    }
    void ShowDistText()
    {
        float distValue = Mathf.Round(GameManager.Instance.RacingDist);
        distText.text = $"{distValue} m";
    }
    /// <summary>
    /// 부스터 게이지 관리
    /// 1) 부스터 현재 게이지 보이게
    /// 2) 현재 부스터 개수 보이게
    /// </summary>
    void BoosterGageUI()
    {
        gageAmountUI.fillAmount = GameManager.Instance.CurrentBoosterGage;
    }
}
