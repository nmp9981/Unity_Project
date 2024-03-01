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
    // Start is called before the first frame update
    void Awake()
    {
        
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
    }
    public void StageButtonOff()
    {
        _stageUIObject.SetActive(false);
        _modeUIObject.SetActive(true);
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
