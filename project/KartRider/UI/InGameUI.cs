using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI velocityText;
    [SerializeField]
    TextMeshProUGUI distText;
    TextMeshProUGUI timerText;

    Image gageAmountUI;

    private void Awake()
    {
        gageAmountUI = GameObject.Find("GageAmountImage").GetComponent<Image>();
        timerText = GameObject.Find("TimerUI").GetComponent<TextMeshProUGUI>();
        GameManager.Instance.CurrentTime = 148f;
    }
    void Update()
    {
        ShowVelocityText();
        ShowDistText();
        ShowTimeText();
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
    /// <summary>
    /// 기능 : 현재 경과시간을 보이기
    /// 1) 10초 미만인 경우 앞에 0을 붙임
    /// </summary>
    void ShowTimeText()
    {
        GameManager.Instance.CurrentTime -= Time.deltaTime;
        GameManager.Instance.CurrentTime = Mathf.Max(GameManager.Instance.CurrentTime, 0);

        int minutes = (int)GameManager.Instance.CurrentTime / 60;
        float rest = GameManager.Instance.CurrentTime % 60;
        float seconds = Mathf.Round(rest * 100f);//소수점 둘째 자리까지
        int secondsDiv = (int)seconds / 100;
        int secondsMod = (int)seconds % 100;

        string secondDivText = (secondsDiv<10)? $"0{secondsDiv}" : $"{secondsDiv}";
        string secondModText = (secondsMod<10) ? $"0{secondsMod}" : $"{secondsMod}";
        
        timerText.text = $"{minutes}:{secondDivText}.{secondModText}";
        if (GameManager.Instance.CurrentTime < 10) timerText.color = Color.red;
        else timerText.color = Color.white;
    }
}
