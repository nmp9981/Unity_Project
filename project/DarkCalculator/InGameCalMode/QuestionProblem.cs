using TMPro;
using UnityEngine;

public enum CalMode
{
    Pluse,
    Minus,
    Multi,
    Div
}
public class QuestionProblem : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI problemCountText;
    [SerializeField]
    TextMeshProUGUI timeText;

    TextMeshProUGUI problemText;
    public float currentTime;

    private void Awake()
    {
        problemText = GetComponent<TextMeshProUGUI>();
        currentTime = 0;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetProblem();
        ShowProblemCount();
    }

    // Update is called once per frame
    void Update()
    {
        TimeFlow();
    }
    //문제 출제
    public void SetProblem()
    {
        int a = Random.Range(10, 100);
        int b = Random.Range(10, 100);
        GameManager.Instance.CurrentCalMode = CalMode.Pluse;

        switch (GameManager.Instance.CurrentCalMode)
        {
            case CalMode.Pluse:
                GameManager.Instance.RealAnswer = a + b;
                problemText.text = a.ToString() + " + " + b.ToString();
                break;
            case CalMode.Minus:
                GameManager.Instance.RealAnswer = a - b;
                problemText.text = a.ToString() + " - " + b.ToString();
                break;
            case CalMode.Multi:
                GameManager.Instance.RealAnswer = a * b;
                problemText.text = a.ToString() + " * " + b.ToString();
                break;
            case CalMode.Div:
                GameManager.Instance.RealAnswer = a / b;
                problemText.text = a.ToString() + " / " + b.ToString();
                break;
            default:
                break;
        }
       
    }
    /// <summary>
    /// 기능 ; 시간 흐름
    /// </summary>
    void TimeFlow()
    {
        currentTime += Time.deltaTime;
        ShowCurrentTime();
    }
    /// <summary>
    /// 기능 : 현재 시간이 보여야함
    /// </summary>
    void ShowCurrentTime()
    {
        timeText.text = Mathf.Floor(currentTime).ToString();
    }
    public void ShowProblemCount()
    {
        GameManager.Instance.CurrentSolveCount += 1;
        problemCountText.text = $"{GameManager.Instance.CurrentSolveCount} / {GameManager.Instance.TargetSolveCount}";
    }
}
