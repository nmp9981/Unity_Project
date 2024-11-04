using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public class MenuUIManager : MonoBehaviour
{
    //선택한 맵 이름
    public static TextMeshProUGUI selectMapText;
    //맵 페이지 오브젝트 리스트
    [SerializeField]
    private List<GameObject> mapPageList = new List<GameObject>();

    //로비UI
    [SerializeField]
    private GameObject lobbyUI;
    //타임어택 UI
    [SerializeField]
    private GameObject timeAttackUI;
    //상점 UI
    [SerializeField]
    private GameObject storeUI;
    //게임 종료 UI
    [SerializeField]
    private GameObject exitUIObject;
    //사운드 조절 UI
    [SerializeField]
    private GameObject settingSoundUIIbject;
    //사운드 조절 값
    [SerializeField]
    private Slider bgmSlider;
    [SerializeField]
    private Slider sfxSlider;

    private void Awake()
    {
        MenuButtonBinding();
    }
    private void Start()
    {
        SoundManger._sound.PlayBGM((int)BGMSound.Main);
    }
    /// <summary>
    /// 기능 : 메인 UI 세팅
    /// 1) 맵 선택 칸
    /// 2) 맵 페이지 이동 버튼 오브젝트 등록
    /// </summary>
    void SettingMainUI()
    {
        selectMapText = GameObject.Find("MapTextBackGround").GetComponentInChildren<TextMeshProUGUI>();
        selectMapText.text = string.Empty;
    }
    /// <summary>
    /// 기능 : 메뉴 버튼 바인딩
    /// 1) 시작 버튼 바인딩
    /// 2) 맵 페이지 이동 버튼 바인딩
    /// </summary>
    void MenuButtonBinding()
    {
        foreach(var gm in gameObject.GetComponentsInChildren<Button>(true))
        {
            string gmName = gm.gameObject.name;
            switch (gmName)
            {
                case "StartButton":
                    gm.onClick.AddListener(() => GoRacingStart(GameManager.Instance.CurrentMap));
                    break;
                case "PrevPage":
                    gm.onClick.AddListener(() => MovePrevMapPage());
                    break;
                case "NextPage":
                    gm.onClick.AddListener(() => MoveNextMapPage());
                    break;
                case "SettingButton":
                    gm.onClick.AddListener(() => OepnSettingSoundWindow());
                    break;
                case "ExitButton":
                    gm.onClick.AddListener(() => OpenExitWindow());
                    break;
                case "YesExit":
                    gm.onClick.AddListener(() => ExitYesButton());
                    break;
                case "NoExit":
                    gm.onClick.AddListener(() => ExitNoButton());
                    break;
                case "SoundCheckButton":
                    gm.onClick.AddListener(() => CheckSoungSettingButton());
                    break;
                case "TimeAttack":
                    gm.onClick.AddListener(() => GoTimeAttackMode());
                    break;
                case "Store":
                    gm.onClick.AddListener(() => GoStore());
                    break;
                case "BackLobbyButton":
                    gm.onClick.AddListener(() => BackLobbyUI());
                    break;
                default:
                    break;
            }
        }
    }
    /// <summary>
    /// 기능 : 고른 맵으로 이동
    /// 1) 선택된 맵이 존재해야함
    /// 2) 인게임으로 씬 전환
    /// 3) 각 맵의 시작 지점으로 이동
    /// </summary>
    public void GoRacingStart(string mapName)
    {
        if (mapName != string.Empty)
        {
            //선택된 맵의 시작지점으로 씬 이동
            SceneManager.LoadScene("KartGameMain");
            //로딩창 켜기
            GameLoading.LoadingOn();
            //주행 상태 아님
            GameManager.Instance.IsDriving = false;
            //시작지점으로 이동
            int mapNumber = GameManager.mapDictoinaty[mapName];
            GameManager.MoveStartPositionAndDataInit(mapNumber);
            int musicNum = 2*mapNumber+(Random.Range(0, 10) % 2)+1;
            if (musicNum > 6) musicNum = 6;
            SoundManger._sound.PlayBGM(musicNum);
        }
    }
    /// <summary>
    /// 기능 : 이전 맵 페이지로 이동
    /// </summary>
    public void MovePrevMapPage()
    {
        mapPageList[GameManager.Instance.CurrentMapPageIndex].SetActive(false);
        GameManager.Instance.CurrentMapPageIndex-=1;
        if (GameManager.Instance.CurrentMapPageIndex <= 0) GameManager.Instance.CurrentMapPageIndex = 0;
        mapPageList[GameManager.Instance.CurrentMapPageIndex].SetActive(true);
    }
    /// <summary>
    /// 기능 ; 다음 맵 페이지로 이동
    /// </summary>
    public void MoveNextMapPage()
    {
        mapPageList[GameManager.Instance.CurrentMapPageIndex].SetActive(false);
        GameManager.Instance.CurrentMapPageIndex += 1;
        if (GameManager.Instance.CurrentMapPageIndex > 1) GameManager.Instance.CurrentMapPageIndex = 1;
        mapPageList[GameManager.Instance.CurrentMapPageIndex].SetActive(true);
    }
    /// <summary>
    /// 기능 : 게임 종료창 열기
    /// </summary>
    public void OpenExitWindow()
    {
        if (!exitUIObject.activeSelf)
        {
            exitUIObject.SetActive(true);
        }
    }
    /// <summary>
    /// 기능 : 게임 종료
    /// </summary>
    public void ExitYesButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    /// <summary>
    /// 기능 : 게임 종료창 닫기
    /// </summary>
    public void ExitNoButton()
    {
        if (exitUIObject.activeSelf)
        {
            exitUIObject.SetActive(false);
        }
    }
    /// <summary>
    /// 기능 : 사운드 세팅창 열기
    /// </summary>
    public void OepnSettingSoundWindow()
    {
        if (!settingSoundUIIbject.activeSelf)
        {
            settingSoundUIIbject.SetActive(true);
        }
    }
    /// <summary>
    /// 사운드 세팅 홛인 버튼
    /// bgm, sfx값 확정하여 최종 적용
    /// </summary>
    public void CheckSoungSettingButton()
    {
        //사운드 값 확정
        GameManager.Instance.BGMVolume = bgmSlider.value;
        GameManager.Instance.SFXVolume = sfxSlider.value;
        // UI창 닫기
        if (settingSoundUIIbject.activeSelf)
        {
            settingSoundUIIbject.SetActive(false);
        }
    }
    public void GoTimeAttackMode()
    {
        lobbyUI.SetActive(false);
        timeAttackUI.SetActive(true);
        GameManager.Instance.SettingMapList();
        SettingMainUI();
    }
    public void GoStore()
    {
        lobbyUI.SetActive(false);
        storeUI.SetActive(true);
    }
    public void BackLobbyUI()
    {
        timeAttackUI.SetActive(false);
        storeUI.SetActive(false);
        lobbyUI.SetActive(true);
    }
}
