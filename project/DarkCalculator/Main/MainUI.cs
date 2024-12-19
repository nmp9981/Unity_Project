using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Xml.Linq;

public class MainUI : MonoBehaviour
{
    //세팅 UI
    [SerializeField]
    private GameObject settingUI;
    //사운드 조절 UI
    [SerializeField]
    private GameObject settingSoundUIIbject;
    //사운드 조절 값
    [SerializeField]
    private Slider bgmSlider;

    private void Awake()
    {
        MenuButtonBinding();
    }
    private void Start()
    {
        //초기 사운드 값 결정
        bgmSlider.value = GameManager.Instance.BGMVolume;
        SoundManager._sound.PlayBGM(0);
    }

    /// <summary>
    /// 기능 : 메뉴 버튼 바인딩
    /// 1) 시작 버튼 바인딩
    /// 2) 맵 페이지 이동 버튼 바인딩
    /// </summary>
    void MenuButtonBinding()
    {
        foreach (var gm in gameObject.GetComponentsInChildren<Button>(true))
        {
            string gmName = gm.gameObject.name;
            
            switch (gmName)
            {
                case "Play":
                    gm.onClick.AddListener(() => GoGameStart(gmName));
                    break;
                case "Prime":
                    gm.onClick.AddListener(() => GoGameStart(gmName));
                    break;
                case "Setting":
                    gm.onClick.AddListener(() => SettingCalMode());
                    break;
                case "SettingOK":
                    gm.onClick.AddListener(() => CloseSettingCalMode());
                    break;
                case "Exit":
                    gm.onClick.AddListener(() => ExitButton());
                    break;       
                case "Sound":
                    gm.onClick.AddListener(() => OepnSettingSoundWindow());
                    break;
                case "SoundClose":
                    gm.onClick.AddListener(() => CheckSoungSettingButton());
                    break;
                default:
                    break;
            }
        }
    }
    /// <summary>
    /// 기능 : 암산 게임으로 이동
    /// 1) 선택된 맵이 존재해야함
    /// 2) 인게임으로 씬 전환
    /// 3) 각 맵의 시작 지점으로 이동
    /// 4) BGM 재생
    /// </summary>
    public void GoGameStart(string mode)
    {
        switch (mode)
        {
            case "Play":
                SceneManager.LoadScene("InGameCalMode");
                SoundManager._sound.StopBGM(0);
                break;
            case "Prime":
                SceneManager.LoadScene("PrimeMode");
                SoundManager._sound.StopBGM(0);
                SoundManager._sound.PlayBGM(1);
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 기능 : 암산 모드 세팅창 열기
    /// </summary>
    public void SettingCalMode()
    {
        if (!settingUI.activeSelf)
        {
            settingUI.SetActive(true);
        }
    }
    /// <summary>
    /// 기능 : 암산 모드 세팅창 닫기 (세팅 가능할 경우만)
    /// </summary>
    public void CloseSettingCalMode()
    {
        if (GameManager.Instance.IsSettingPossible)
        {
            settingUI.SetActive(false);
        }
    }
    
    /// <summary>
    /// 기능 : 게임 종료
    /// </summary>
    public void ExitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
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
        // UI창 닫기
        if (settingSoundUIIbject.activeSelf)
        {
            settingSoundUIIbject.SetActive(false);
        }
    }
}
