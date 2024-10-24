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

    //시작 버튼
    Button startButton;

    private void Awake()
    {
        SettingMainUI();
    }
    private void Start()
    {
        SoundManger._sound.PlayBGM((int)BGMSound.Main);
    }
    /// <summary>
    /// 기능 : 메인 UI 세팅
    /// 1) 맵 선택 칸
    /// 2) 시작 버튼 바인딩
    /// </summary>
    void SettingMainUI()
    {
        selectMapText = GameObject.Find("MapTextBackGround").GetComponentInChildren<TextMeshProUGUI>();
        selectMapText.text = string.Empty;

        startButton = GameObject.Find("RacingStart").GetComponentInChildren<Button>();
        startButton.onClick.AddListener(() => GoRacingStart(GameManager.Instance.CurrentMap));
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
            SoundManger._sound.PlayBGM(musicNum);
        }
    }
}
