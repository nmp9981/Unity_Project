using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CalSetting : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI digitText;
    
    [SerializeField]
    TextMeshProUGUI problemCountText;

    private void Awake()
    {
        InitShowText();
    }
    void OnEnable()
    {
        BindingToggle();
        BindingSlider();
    }
    /// <summary>
    /// 기능 : 초기 보이는 설정값
    /// </summary>
    void InitShowText()
    {
        problemCountText.text = $"Problem Count : {GameManager.Instance.TargetSolveCount}";
        digitText.text = $"Digit ({GameManager.Instance.DigitMinCount}~{GameManager.Instance.DigitMaxCount})";
    }
    /// <summary>
    /// 기능 : 각 토글들 기능 바인딩
    /// </summary>
    void BindingToggle()
    {
        GameManager.Instance.calSymbolList[0] = true;
        GameManager.Instance.calSymbolList[1] = false;
        GameManager.Instance.calSymbolList[2] = false;
        GameManager.Instance.calSymbolList[3] = false;

        GameManager.Instance.calCountList[0] = true;
        GameManager.Instance.calCountList[1] = false;

        foreach (var gm in gameObject.GetComponentsInChildren<Toggle>(true))
        {
            string gmName = gm.gameObject.name;

            switch (gmName)
            {
                case "Plus":
                    gm.isOn = true;
                    gm.onValueChanged.AddListener(delegate{ SettingSymbol(gm, 0); });
                    break;
                case "Minus":
                    gm.isOn = false;
                    gm.onValueChanged.AddListener(delegate { SettingSymbol(gm, 1); });
                    break;
                case "Multi":
                    gm.isOn = false;
                    gm.onValueChanged.AddListener(delegate { SettingSymbol(gm, 2); });
                    break;
                case "Div":
                    gm.isOn = false;
                    gm.onValueChanged.AddListener(delegate { SettingSymbol(gm, 3); });
                    break;
                case "Count2":
                    gm.isOn = true;
                    gm.onValueChanged.AddListener(delegate { SettingCalCount(gm, 0); });
                    break;
                case "Count3":
                    gm.isOn = false;
                    gm.onValueChanged.AddListener(delegate { SettingCalCount(gm, 1); });
                    break;
                default:
                    break;
            }
        }
    }
    /// <summary>
    /// 기능 : 각 슬라이더들 기능 바인딩
    /// </summary>
    void BindingSlider()
    {
        foreach (var gm in gameObject.GetComponentsInChildren <Slider>(true))
        {
            string gmName = gm.gameObject.name;

            switch (gmName)
            {
                case "MinSlider":
                    gm.onValueChanged.AddListener(delegate { SettingMinDigit(gm); });
                    break;
                case "MaxSlider":
                    gm.onValueChanged.AddListener(delegate { SettingMaxDigit(gm); });
                    break;
                case "ProblemCount":
                    gm.onValueChanged.AddListener(delegate { SettingProblemCount(gm); });
                    break;
                default:
                    break;
            }
        }
    }
    /// <summary>
    /// 기능 :연산 기호 세팅
    /// 1) On : 해당 연산이 문제 출제가 되도록
    /// </summary>
    /// <param name="gm">토글 버튼</param>
    /// <param name="idx">연산 인덱스</param>
    public void SettingSymbol(Toggle gm, int idx)
    {
        if (!gm.isOn)
        {
            GameManager.Instance.calSymbolList[idx] = false;
        }
        else
        {
            GameManager.Instance.calSymbolList[idx] = true;
        }
    }
    /// <summary>
    /// 기능 : 계산할 숫자 개수 설정
    /// 1) On : 체크에 따라 2항 or 3항으로 출제
    /// </summary>
    /// <param name="gm">토글 버튼</param>
    /// <param name="idx">개수 인덱스</param>
    public void SettingCalCount(Toggle gm, int idx)
    {
        if (!gm.isOn)
        {
            GameManager.Instance.calCountList[idx] = false;
            if (GameManager.Instance.calSymbolJudgeList.Count > 1)
            {
                GameManager.Instance.calSymbolJudgeList.Remove(idx);
            }
        }
        else
        {
            GameManager.Instance.calCountList[idx] = true;
            //중복 방지
            foreach(var item in GameManager.Instance.calSymbolJudgeList)
            {
                if(item == idx)
                {
                    return;
                }
            }
            GameManager.Instance.calSymbolJudgeList.Add(idx);
        }
    }
    /// <summary>
    /// 기능 : 출제할 문제 수 세팅
    /// </summary>
    /// <param name="sl">슬라이더</param>
    public void SettingProblemCount(Slider sl)
    {
        GameManager.Instance.TargetSolveCount = (int)sl.value;
        problemCountText.text = $"Problem Count : {GameManager.Instance.TargetSolveCount}";
    }
    /// <summary>
    /// 기능 : 최소 자릿수 세팅
    /// </summary>
    /// <param name="sl">슬라이더</param>
    public void SettingMinDigit(Slider sl)
    {
        GameManager.Instance.DigitMinCount = (int)sl.value;
        digitText.text = $"Digit ({GameManager.Instance.DigitMinCount}~{GameManager.Instance.DigitMaxCount})";
    }
    /// <summary>
    /// 기능 : 최대 자릿수 세팅
    /// </summary>
    /// <param name="sl">슬라이더</param>
    public void SettingMaxDigit(Slider sl)
    {
        GameManager.Instance.DigitMaxCount = (int)sl.value;
        digitText.text = $"Digit ({GameManager.Instance.DigitMinCount}~{GameManager.Instance.DigitMaxCount})";
    }
}
