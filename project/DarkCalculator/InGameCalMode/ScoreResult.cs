using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    }
    /// <summary>
    /// 기능 : 결과 기록 조회
    /// </summary>
    void ShowResultRecord()
    {
        resultText.text = $"Time\n{GameManager.Instance.RecordTime} \n\nCorrect Count\n{GameManager.Instance.CurrentSolveCount} \n\n" +
            $"Score\n{GameManager.Instance.Score}";
    }
    /// <summary>
    /// 기능 :점수 계산
    /// </summary>
    void CalScore()
    {
        float answerCountScore = (float)(GameManager.Instance.CurrentSolveCount*40)/GameManager.Instance.TargetSolveCount;
        float timeScore = (GameManager.Instance.RecordTime <= GameManager.Instance.TargetSolveCount) ? 60f :
            Mathf.Max(0, 60 + GameManager.Instance.TargetSolveCount - GameManager.Instance.RecordTime);
  
        GameManager.Instance.Score = (int) (answerCountScore+timeScore);
    }
}
