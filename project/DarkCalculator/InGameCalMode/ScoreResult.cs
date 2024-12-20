using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScoreResult : MonoBehaviour
{
    [SerializeField]
    Button okButton;

    [SerializeField]
    TextMeshProUGUI resultText;

    private void Awake()
    {
        BindingOKButton();
    }
    private void OnEnable()
    {
        CalScore();
        ShowResultRecord();
    }
    /// <summary>
    /// OK 버튼 바인딩
    /// </summary>
    void BindingOKButton()
    {
        okButton.onClick.AddListener(MoveToMainScene);
    }
    /// <summary>
    /// 기능 : 메인씬 이동
    /// </summary>
    void MoveToMainScene()
    {
        GameManager.Instance.InitInGameData();
        SceneManager.LoadScene("MainCalMode", LoadSceneMode.Single);
        SoundManager._sound.PlayBGM(0);
    }
    /// <summary>
    /// 기능 : 결과 기록 조회
    /// </summary>
    void ShowResultRecord()
    {
        resultText.text = $"경과 시간\n{GameManager.Instance.RecordTime} \n\n정답 개수\n{GameManager.Instance.CurrentSolveCount} \n\n" +
            $"점수\n{GameManager.Instance.Score}";
    }
    /// <summary>
    /// 기능 :점수 계산
    /// </summary>
    void CalScore()
    {
        float answerCountScore = (float)GameManager.Instance.CurrentSolveCount/GameManager.Instance.TargetSolveCount;
        float perfectTimeLimit = (GameManager.Instance.TargetSolveCount * 5/2) + 3 + GameManager.Instance.Cal3DigitCount*3;

        float finalScore = (GameManager.Instance.RecordTime <= perfectTimeLimit) ? 100f* answerCountScore :
            Mathf.Max(0, 100 - (GameManager.Instance.RecordTime - perfectTimeLimit))* answerCountScore;
  
        GameManager.Instance.Score = (int) finalScore;
    }
}
