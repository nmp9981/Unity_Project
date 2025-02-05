using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    CastleManager castleManager;

    [SerializeField]
    LoadingManager loadingManager;

    void Awake()
    {
        castleManager = GetComponent<CastleManager>();
        BindingButton();
    }
    /// <summary>
    /// 기능 : 버튼 바인딩
    /// </summary>
    void BindingButton()
    {
        foreach (Button btn in this.gameObject.GetComponentsInChildren<Button>(true))
        {
            string btnName = btn.gameObject.name;
            switch (btnName)
            {
                case "Start":
                    btn.onClick.AddListener(GoGameStart);
                    break;
                case "Exit":
                    btn.onClick.AddListener(QuitGame);
                    break;
                default:
                    break;
            }
        }
    }
    /// <summary>
    /// 게임 종료
    /// </summary>
    void QuitGame()
    {
#if UNITY_EDITOR //에디터에서
        UnityEditor.EditorApplication.isPlaying = false;
#else //나머지
        Application.Quit(); // 어플리케이션 종료
#endif
    }
    /// <summary>
    /// 게임 시작
    /// </summary>
    void GoGameStart()
    {
        if(castleManager != null)
        {
            loadingManager.LoadingPrograssBar();//로딩창
            castleManager.InitGameInfomation();
        }

        //활성화
        castleManager.monsterEntranceObj.SetActive(true);
        castleManager.gameObject.SetActive(true);

        GameManager.Instance.IsGamePlaying = true;
        GameManager.Instance.IsDie = false;
        GameManager.Instance.CurrentTurretList[0].gameObject.SetActive(true);
        SoundManager._sound.PlayBGM(0);
        this.gameObject.SetActive(false);
    }
}
