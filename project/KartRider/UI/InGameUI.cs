using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class InGameUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI velocityText;
    [SerializeField]
    TextMeshProUGUI distText;
    TextMeshProUGUI timerText;

    TextMeshProUGUI lapUI;
    Image gageAmountUI;

    private void Awake()
    {
        gageAmountUI = GameObject.Find("GageAmountImage").GetComponent<Image>();
        timerText = GameObject.Find("TimerUI").GetComponent<TextMeshProUGUI>();
        lapUI = GameObject.Find("Lap").transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        GameManager.Instance.CurrentTime = 148f;
    }
    void Update()
    {
        ShowVelocityText();
        ShowDistText();
        ShowTimeText();
        ShowLapText();
        BoosterGageUI();
    }
    /// <summary>
    /// 기능 : 출발 이벤트
    /// 1) 3,2,1,Start
    /// 2) Start를 하면 주행 진행 상태로 변경
    /// 3) Start까지 끝나면 텍스트는 사라지게
    /// </summary>
    async UniTask ShowReadyText()
    {
        TextMeshProUGUI readyText = null;
        for(int i = 3; i >= 0; i--)
        {
            if (i == 0)
            {
                readyText.text = "Start!!";
                GameManager.Instance.IsDriving = true;
            }
            else
            {
                readyText.text = $"{i}";
            }
            await UniTask.Delay(1000);
        }
        readyText.text = string.Empty;
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
    void ShowLapText()
    {
        lapUI.text = $"{GameManager.Instance.CurrentLap}/{GameManager.Instance.MapLap}";
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
    /// 2) 주행상태일때만 시간이 흐름
    /// </summary>
    void ShowTimeText()
    {
        //주행중일때만 시간 흐름
        if (GameManager.Instance.IsDriving)
        {
            GameManager.Instance.CurrentTime -= Time.deltaTime;
        }
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
