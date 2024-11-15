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
    List<GameObject> activeKartList = new List<GameObject>();
    [SerializeField]
    List<MeshRenderer> kartColor = new List<MeshRenderer>();
    [SerializeField]
    GameObject returnHome;
    [SerializeField]
    TextMeshProUGUI velocityText;
    [SerializeField]
    TextMeshProUGUI distText;
    TextMeshProUGUI timerText;
    TextMeshProUGUI bestTimerText;
    TextMeshProUGUI readyText;

    [SerializeField] 
    TextMeshProUGUI getLucciText;

    TextMeshProUGUI lapUI;
    RectTransform velocityNeedleImage;
    Image gageAmountUI;
    BoosterManager boosterManager;

    private void Awake()
    {
        BindingInGameUI();
    }
    private void OnEnable()
    {
        PickActiveKart();
        PaintKartColor();
        ShowReadyText();
    }
    void Update()
    {
        ShowVelocityTextAndNeedle();
        ShowDistText();
        ShowRestTimeText();
        ShowBestTimeText();
        ShowLapText();
        ShowGetLucciText();
        BoosterGageUI();
        ShowReturnUI();
    }
    /// <summary>
    /// 기능 : ingame UI 바인딩
    /// </summary>
    void BindingInGameUI()
    {
        gageAmountUI = GameObject.Find("GageAmountImage").GetComponent<Image>();
        timerText = GameObject.Find("TimerUI").GetComponent<TextMeshProUGUI>();
        bestTimerText = GameObject.Find("BestTimeText").GetComponent<TextMeshProUGUI>();
        readyText = GameObject.Find("ReadyText").GetComponent<TextMeshProUGUI>();
        lapUI = GameObject.Find("Lap").transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        velocityNeedleImage = GameObject.Find("needle").GetComponent<RectTransform>();
        boosterManager = GameObject.Find("Player").GetComponent<BoosterManager>();
    }
    /// <summary>
    /// 기능 : 선택한 카트 디자인 적용
    /// </summary>
    void PickActiveKart()
    {
        for (int idx = 0; idx < activeKartList.Count; idx++)
        {
            if (GameManager.Instance.CurrentKartNum == idx)
            {
                activeKartList[idx].SetActive(true);
            }
            else
            {
                activeKartList[idx].SetActive(false);
            }
        }
    }
    /// <summary>
    /// 기능 : 카트 색상 적용
    /// </summary>
    void PaintKartColor()
    {
        switch (GameManager.Instance.CurrentKartNum)
        {
            case 0:
                kartColor[0].material.color = GameManager.Instance.CurrentKartColor;
                break;
            case 1:
                kartColor[1].materials[0].color = GameManager.Instance.CurrentKartColor;
                break;
            case 2:
                kartColor[2].material.color = GameManager.Instance.CurrentKartColor;
                break;
            case 3:
                kartColor[3].material.color = GameManager.Instance.CurrentKartColor;
                break;
        }
    }
    /// <summary>
    /// 기능 : 출발 이벤트
    /// 1) 3,2,1,Start
    /// 2) Start를 하면 주행 진행 상태로 변경
    /// 3) Start까지 끝나면 텍스트는 사라지게
    /// 4) 출발시 출발부스터 발동
    /// </summary>
    async UniTask ShowReadyText()
    {
        GameManager.Instance.CurrentRestTime = 48f+24*GameManager.Instance.CurrentMapIndex;
        GameManager.Instance.IsDriving = false;
        await UniTask.Delay(2000);
      
        GameManager.Instance.MapLap = GameManager.Instance.mapLapList[GameManager.Instance.CurrentMapIndex];//여기서 총 몇바퀴 맵인지 설정
        for (int i = 3; i >= -1; i--)
        {
            if (i == 0)
            {
                readyText.text = "Start!!";
                SoundManger._sound.PlaySfx((int)SFXSound.ReadyGo);
                GameManager.Instance.IsDriving = true;
                //시작 직후 1초내로 출발부스터 발동
                boosterManager.StartBooster();
            }
            else if (i == -1)
            {
                readyText.text = string.Empty;
                
            }
            else
            {
                readyText.text = $"{i}";
                SoundManger._sound.PlaySfx((int)SFXSound.Ready);
            }
            await UniTask.Delay(1000);
        }
    }
    void ShowVelocityTextAndNeedle()
    {
        float velocityValue = Mathf.Round(GameManager.Instance.CarSpeed);
        velocityText.text = $"{velocityValue} km/h";
        //속도계 바늘 회전
        velocityNeedleImage.rotation = Quaternion.Euler(0, 0, -velocityValue);
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
    void ShowGetLucciText()
    {
        if (GameManager.Instance.IsCollideCoin)
        {
            StartCoroutine(ShowGetLucci());
        }
    }
    //얻은 루찌 텍스트
    IEnumerator ShowGetLucci()
    {
        int getLucci = Random.Range(50, 100);
        GameManager.Instance.PlayerLucci += getLucci;
        GameManager.Instance.IsCollideCoin = false;

        getLucciText.text = $"+{getLucci} Lucci";
        yield return new WaitForSeconds(1.5f);
        getLucciText.text = string.Empty;
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
    void ShowRestTimeText()
    {
        //주행중일때만 시간 흐름
        if (GameManager.Instance.IsDriving)
        {
            GameManager.Instance.CurrentRestTime -= Time.deltaTime;
            GameManager.Instance.CurrentTime += Time.deltaTime;
        }
        //시간 다됨
        if(GameManager.Instance.CurrentRestTime<=0)
        {
            ShowFinishText();
            return;
        }
        GameManager.Instance.CurrentRestTime = Mathf.Max(GameManager.Instance.CurrentRestTime, 0);

        int minutes = (int)GameManager.Instance.CurrentRestTime / 60;
        float rest = GameManager.Instance.CurrentRestTime % 60;
        float seconds = Mathf.Round(rest * 100f);//소수점 둘째 자리까지
        int secondsDiv = (int)seconds / 100;
        int secondsMod = (int)seconds % 100;

        string secondDivText = (secondsDiv<10)? $"0{secondsDiv}" : $"{secondsDiv}";
        string secondModText = (secondsMod<10) ? $"0{secondsMod}" : $"{secondsMod}";
        
        timerText.text = $"{minutes}:{secondDivText}.{secondModText}";
        if (GameManager.Instance.CurrentRestTime < 10) timerText.color = Color.red;
        else timerText.color = Color.white;
    }
    /// <summary>
    /// 기능 : 베스트 타임 보이기
    /// 1) 10초 미만인 경우 앞에 0을 붙임
    /// 2) 아직 기록 없으면 0으로 진행
    /// </summary>
    public void ShowBestTimeText()
    {
        int minutes = (int)GameManager.Instance.BestLapTime / 60;
        float rest = GameManager.Instance.BestLapTime % 60;
        float seconds = Mathf.Round(rest * 100f);//소수점 둘째 자리까지
        int secondsDiv = (int)seconds / 100;
        int secondsMod = (int)seconds % 100;

        string secondDivText = (secondsDiv < 10) ? $"0{secondsDiv}" : $"{secondsDiv}";
        string secondModText = (secondsMod < 10) ? $"0{secondsMod}" : $"{secondsMod}";

        bestTimerText.text = $"{minutes}:{secondDivText}.{secondModText}";
    }
    /// <summary>
    /// 기능 종료 문구 띄우기
    /// </summary>
    void ShowFinishText()
    {
        GameManager.Instance.IsDriving = false;
        timerText.text = "Finish";
    }
    /// <summary>
    /// 기능 : Return Home UI열기
    /// </summary>
    void ShowReturnUI()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (returnHome.activeSelf)
            {
                returnHome.SetActive(false);
            }
            else if(!returnHome.activeSelf)
            {
                returnHome.SetActive(true);
            }
        }
    }
}
