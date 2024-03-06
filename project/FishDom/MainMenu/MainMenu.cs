using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.AssemblyQualifiedNameParser;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject _stageUIObject;
    [SerializeField] GameObject _modeUIObject;
    [SerializeField] GameObject _recordBoard;
    [SerializeField] TextMeshProUGUI _recordText;
    [SerializeField] TextMeshProUGUI _pageNumText;

    public GameObject[] _uiPage = new GameObject[5];
    private int _uiPageNum;
    // Start is called before the first frame update
    void Awake()
    {
        _uiPageNum = 0;

    }

    // Update is called once per frame
    void Update()
    {
        PageTextShow();
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
        _recordText.text = "My Record"+"\n\nMax Stege : "+PlayerPrefs.GetInt("BestStage")+"\nMax Attack : "+PlayerPrefs.GetString("BestAttack");
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
        Application.Quit(); // 어플리케이션 종료
#endif
    }
}
