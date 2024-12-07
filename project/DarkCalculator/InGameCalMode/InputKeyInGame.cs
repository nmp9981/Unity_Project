using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputKeyInGame : MonoBehaviour
{
    [SerializeField]
    GameObject clearObject;

    [SerializeField]
    GameObject KeyButtonSet;

    [SerializeField]
    TMP_InputField inputAnswerField;

    QuestionProblem questionProblem;
    
    void Awake()
    {
        BindingKeyButton();
        questionProblem = GameObject.Find("Question").GetComponent<QuestionProblem>();

        GameManager.Instance.TargetSolveCount = 5;
    }

    void BindingKeyButton()
    {
        foreach(Button key in KeyButtonSet.GetComponentsInChildren<Button>(true))
        {
            key.onClick.AddListener(()=>InputNumber(key.gameObject.name));
        }
        
    }
    /// <summary>
    /// 기능 : 숫자키 입력버튼 등록
    /// </summary>
    /// <param name="name">버튼 오브젝트 명으로 구분</param>
    public void InputNumber(string name)
    {
        string keyNumber = name.Substring(3);
        
        switch (keyNumber)
        {
            case "Minus":
                GameManager.Instance.InputAnswerString += "-";
                break;
            case "X":
                GameManager.Instance.InputAnswerString = string.Empty;
                break;
            default:
                GameManager.Instance.InputAnswerString += keyNumber;
                break;
        }
        ShowInputAnswer();
        CompareAnswerAndInput();
    }
    /// <summary>
    /// 기능 : 입력값이 보이게
    /// </summary>
    void ShowInputAnswer()
    {
        inputAnswerField.text = GameManager.Instance.InputAnswerString;
    }
    /// <summary>
    /// 입력값과 정답 비교
    /// </summary>
    void CompareAnswerAndInput()
    {
        //정답
        if (GameManager.Instance.InputAnswerString == GameManager.Instance.RealAnswer.ToString())
        {
            //모두 맞춤
            if (GameManager.Instance.CurrentSolveCount+1 == GameManager.Instance.TargetSolveCount)
            {
                AllSolveProblem();
            }

            questionProblem.SetProblem();
            questionProblem.ShowProblemCount();
            //초기화
            inputAnswerField.text = string.Empty;
            GameManager.Instance.InputAnswerString = string.Empty;

            
        }
        else
        {
            Debug.Log("배드");
        }
    }
    /// <summary>
    /// 기능 : 문제를 모두 맟췃을 때 로직
    /// 1) 시간 기록
    /// 2) 클리어 UI 활성화
    /// </summary>
    void AllSolveProblem()
    {
        GameManager.Instance.RecordTime = questionProblem.currentTime;
        clearObject.SetActive(true);
    }
}
