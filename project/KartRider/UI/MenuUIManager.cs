using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public class MenuUIManager : MonoBehaviour
{
    //선택한 맵 이름
    public static TextMeshProUGUI selectMapText;
    //맵 페이지 오브젝트 리스트
    [SerializeField]
    List<GameObject> mapPageList = new List<GameObject>();

    private void Awake()
    {
        SettingMainUI();
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
        var startButton = GameObject.Find("RacingStart").GetComponentInChildren<Button>();
        startButton.onClick.AddListener(() => GoRacingStart(GameManager.Instance.CurrentMap));

        var prevMapPageButton = GameObject.Find("PrevPage").GetComponent<Button>();
        prevMapPageButton.onClick.AddListener(() => MovePrevMapPage());

        var nextMapPageButton = GameObject.Find("NextPage").GetComponent<Button>();
        nextMapPageButton.onClick.AddListener(() => MoveNextMapPage());
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
}
