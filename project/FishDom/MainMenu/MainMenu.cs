using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.AssemblyQualifiedNameParser;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject _stageUIObject;
    [SerializeField] GameObject _modeUIObject;
    [SerializeField] GameObject _recordBoard;
    [SerializeField] GameObject _creditBoard;
    [SerializeField] GameObject _settingBoard;
    [SerializeField] TextMeshProUGUI _recordText;
    [SerializeField] TextMeshProUGUI _pageNumText;
    [SerializeField] GameObject _howToPlayBoard;

    public GameObject[] _uiPage = new GameObject[5];
    private int _uiPageNum;
    // Start is called before the first frame update
    void Awake()
    {
        _uiPageNum = 0;
        StageButtonSetting();
    }

    // Update is called once per frame
    void Update()
    {
        PageTextShow();
    }
    void StageButtonSetting()
    {
        for (int i = 0; i < _uiPage.Length; i++)
        {
            foreach (Transform go in _uiPage[i].transform)
            {
                if (int.Parse(go.gameObject.name) <= GameManager.Instance.PlayerMaxClearStage)
                {
                    go.GetComponent<Button>().interactable = true;
                }
                else go.GetComponent<Button>().interactable = false;
            }
        }
    }
    private void PageTextShow()
    {
        if (_stageUIObject.activeSelf) _pageNumText.text = "Page : "+(_uiPageNum + 1).ToString() + " / 5";
    }
    public void StartButton()
    {
        _modeUIObject.SetActive(true);
    }
    public void StageButton()
    {
        _modeUIObject.SetActive(false);
        _stageUIObject.SetActive(true);

        for (int i = 0; i < _uiPage.Length; i++)
        {
            _uiPage[i].SetActive(false);
        }
        _uiPage[0].SetActive(true);//맨 처음 페이지
       
    }
    public void StageButtonOff()
    {
        _stageUIObject.SetActive(false);
        _modeUIObject.SetActive(true);
    }
    public void StagePageMoveLeft()
    {
        if (_uiPageNum == 0) return;
        _uiPage[_uiPageNum].SetActive(false);
        _uiPageNum -= 1;
        _uiPage[_uiPageNum].SetActive(true);

    }
    public void StagePageMoveRight()
    {
        if (_uiPageNum == _uiPage.Length-1) return;
        _uiPage[_uiPageNum].SetActive(false);
        _uiPageNum += 1;
        _uiPage[_uiPageNum].SetActive(true);
    }
    public void StageStartButton()
    {
        GameObject clickStage = EventSystem.current.currentSelectedGameObject;//방금 클릭한 오브젝트 정보 저장
        GameManager.Instance.PlayMode = Int32.Parse(clickStage.name);
        GameManager.Instance.InitPlayerData(GameManager.Instance.PlayMode);
        SceneManager.LoadScene("FishDomInGame");
    }
    public void ChallengeButton()
    {
        GameManager.Instance.PlayMode = 0;
        SceneManager.LoadScene("FishDomInGame");
    }
    public void ModeUIOff()
    {
        _modeUIObject.SetActive(false);
    }
    public void RecordButton()
    {
        _recordBoard.SetActive(true);
        _recordText.text = "My Record" + "\n\nMax Stege : " + PlayerPrefs.GetInt("BestStage")
            + "\nMax Attack : " + PlayerPrefs.GetString("BestAttack")
            + "\nMax Time : " + string.Format("{0:N0}", PlayerPrefs.GetFloat("BestTime"));
    }
    public void RecordButtonOff()
    {
        _recordBoard.SetActive(false);
    }
    public void ExitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        UnityEngine.Application.Quit(); // 어플리케이션 종료
#endif
    }
    public void CreditButtonOn()
    {
        _creditBoard.SetActive(true);
    }
    public void CreditButtonOff()
    {
        _creditBoard.SetActive(false);
    }
    public void SettingButtonOn()
    {
        _settingBoard.SetActive(true);
        
    }
    public void SettingButtonOff()
    {
        _settingBoard.SetActive(false);
    }
    public void HowToPlayOn()
    {
        _howToPlayBoard.SetActive(true);
    }
    public void HowToPlayOff()
    {
        _howToPlayBoard.SetActive(false);
    }
}
