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

    [SerializeField] GameObject _stagePage1UI;
    [SerializeField] GameObject _stagePage2UI;
    [SerializeField] GameObject _stagePage3UI;
    [SerializeField] GameObject _stagePage4UI;
    [SerializeField] GameObject _stagePage5UI;

    public GameObject[] _uiPage;
    private int _uiPageNum;
    // Start is called before the first frame update
    void Awake()
    {
        _uiPageNum = 0;

        _uiPage = new GameObject[5];
        _uiPage[0] = _stagePage1UI;
        _uiPage[1] = _stagePage2UI;
        _uiPage[2] = _stagePage3UI;
        _uiPage[3] = _stagePage4UI;
        _uiPage[4] = _stagePage5UI;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StartButton()
    {
        _modeUIObject.SetActive(true);
    }
    public void StageButton()
    {
        _modeUIObject.SetActive(false);
        _stageUIObject.SetActive(true);

        for (int i = 0; i < _uiPage.Length; i++) _uiPage[i].SetActive(false);
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
        //PlayerPref로 해결
        //_recordText.text = "My Record"+"\n\nMax Stege : "+GameManager.Instance.StageNum.ToString()+"\nMax Attack : "+GameManager.Instance.PlayerAttack.ToString();
    }
    public void RecordButtonOff()
    {
        _recordBoard.SetActive(false);
    }
    public void ExitButton()
    {

    }
}
