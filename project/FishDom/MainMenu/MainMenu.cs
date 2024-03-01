using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
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
        SceneManager.LoadScene("FishDomInGame");
    }
    public void ChallengeButton()
    {
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
