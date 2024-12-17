using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    Button passButton;
    InputKeyInGame inputKeyInGame;
    TextMeshProUGUI problemText;
    public float currentTime;

    const long maxInt = 2147483648;
    private void Awake()
    {
        problemText = GetComponent<TextMeshProUGUI>();
        inputKeyInGame = GameObject.Find("InputKey").GetComponent<InputKeyInGame>();
        passButton = GameObject.Find("PassButton").GetComponent<Button>();
        currentTime = 0;

        BindingButton();
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
    /// <summary>
    /// 기능 : 버튼 바인딩
    /// </summary>
    void BindingButton()
    {
        passButton.onClick.AddListener(PassButton);
    }
    
    //문제 출제
    public void SetProblem()
    {
        //3개
        if (GameManager.Instance.calCountList[0] == false && GameManager.Instance.calCountList[1] == true)
        {
            SetProblemDegree3();
        }//2개
        else if (GameManager.Instance.calCountList[0] == true && GameManager.Instance.calCountList[1] == false)
        {
            SetProblemDegree2();
        }
        else if (GameManager.Instance.calCountList[0] == true && GameManager.Instance.calCountList[1] == true)
        {
            int digitRan = Random.Range(0, 10) % 2;
            switch (digitRan)
            {
                case 0: //2개
                    SetProblemDegree2();
                    break;
                case 1: //3개
                    SetProblemDegree3();
                    break;
                default:
                    break;
            }
        }
    }
    public void SetProblemDegree2()
    {
        int plusMinNum = 10;
        int plusMaxNum = (int)Mathf.Pow(10, GameManager.Instance.DigitPlusMaxCount);
        int multiMaxNum = (int)Mathf.Pow(10, GameManager.Instance.DigitMultiMaxCount);
        int multiMinNum = 3;
        
        int a = (int)Random.Range(plusMinNum, plusMaxNum);
        int b = (int)Random.Range(plusMinNum, plusMaxNum);
        int c = (int)Random.Range(multiMinNum, multiMaxNum);
        int d = (int)Random.Range(multiMinNum, multiMaxNum);

        int idx = Random.Range(0, GameManager.Instance.calSymbolJudgeList.Count);
        int calModeidx = GameManager.Instance.calSymbolJudgeList[idx];

        switch (calModeidx)
        {
            case 0:
                GameManager.Instance.RealAnswer = a + b;
                problemText.text = a.ToString() + " + " + b.ToString();
                break;
            case 1:
                GameManager.Instance.RealAnswer = a - b;
                problemText.text = a.ToString() + " - " + b.ToString();
                break;
            case 2:
                GameManager.Instance.RealAnswer = c * d;
                problemText.text = c.ToString() + " x " + d.ToString();
                break;
            case 3:
                d = (d>=30)?d/10:d;
                GameManager.Instance.RealAnswer = c / d;
                problemText.text = c.ToString() + " / " + d.ToString();
                break;
            default:
                break;
        }
    }
    public void SetProblemDegree3()
    {
        int plusMinNum = 10;
        int plusMaxNum = (int)Mathf.Pow(10, GameManager.Instance.DigitPlusMaxCount);
        int multiMaxNum = (int)Mathf.Pow(10, GameManager.Instance.DigitMultiMaxCount);
        int multiMinNum = 3;

        int a = (int)Random.Range(plusMinNum, plusMaxNum);
        int b = (int)Random.Range(plusMinNum, plusMaxNum);
        int c = (int)Random.Range(plusMinNum, plusMaxNum);

        int d = (int)Random.Range(multiMinNum, multiMaxNum);
        int e = (int)Random.Range(multiMinNum, multiMaxNum);
        int f = (int)Random.Range(multiMinNum, multiMaxNum);

        int idx = Random.Range(0, GameManager.Instance.calSymbolJudgeList.Count);
        int calModeidx = GameManager.Instance.calSymbolJudgeList[idx];

        switch (calModeidx)
        {
            case 0:
                GameManager.Instance.RealAnswer = a + b + c;
                problemText.text = a.ToString() + " + " + b.ToString()+ " + " + c.ToString();
                break;
            case 1:
                GameManager.Instance.RealAnswer = a - b - c;
                problemText.text = a.ToString() + " - " + b.ToString() + " - " + c.ToString(); ;
                break;
            case 2:
                long tempAnswer = (long) d * e * f;
                //int overflow 예외처리
                if(tempAnswer >= maxInt)
                {
                    d /= 6;
                    e /= 8;
                    f /= 10;
                }
                GameManager.Instance.RealAnswer = d * e * f;
                problemText.text = d.ToString() + " x " + e.ToString() + " x " + f.ToString(); ;
                break;
            case 3://나눗셈은 2자리만 지원
                e = (e >= 30) ? e / 10 : e;
                GameManager.Instance.RealAnswer = d / e;
                problemText.text = d.ToString() + " / " + e.ToString();
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
        GameManager.Instance.CurrentProblemNum += 1;
        problemCountText.text = $"{GameManager.Instance.CurrentProblemNum} / {GameManager.Instance.TargetSolveCount}";
    }
    /// <summary>
    /// 기능 : 패스
    /// 1) 다음 문제로 넘어가기
    /// 2) 점수 반영 X
    /// </summary>
    public void PassButton()
    {
        //모두 맞춤
        if (GameManager.Instance.CurrentProblemNum == GameManager.Instance.TargetSolveCount)
        {
            inputKeyInGame.AllSolveProblem();
        }

        ShowProblemCount();
        SetProblem();
        inputKeyInGame.InputInit();
    }
}
