using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PrimeModeKeySetting : MonoBehaviour
{
    [SerializeField]
    GameObject KeyButtonSet;

    [SerializeField]
    FactorizationIntoPrimes factorsPrimeClass;

    [SerializeField]
    Button resultButton;

    [SerializeField]
    Button backButton;
    
    [SerializeField]
    TMP_InputField inputField;

    [SerializeField]
    TextMeshProUGUI outputField;

    const int maxInputNum = 18446744;//입력 최대 숫자
    List<ulong> factorStringList = new List<ulong>();
    void Awake()
    {
        BindingKeyButton();
    }
    /// <summary>
    /// 기능 : 버튼 바인딩
    /// </summary>
    void BindingKeyButton()
    {
        foreach (Button key in KeyButtonSet.GetComponentsInChildren<Button>(true))
        {
            key.onClick.AddListener(() => InputNumber(key.gameObject.name));
        }
        resultButton.onClick.AddListener(ResultPrime);
        backButton.onClick.AddListener(ReturnMain);
        outputField.text = string.Empty;
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
            case "X":
                GameManager.Instance.InputPrimeString = string.Empty;
                InputInit();
                break;
            default:
                GameManager.Instance.InputPrimeString += keyNumber;
                break;
        }
        ShowInputAnswer();
    }
    /// <summary>
    /// 기능 : 입력값이 보이게
    /// </summary>
    void ShowInputAnswer()
    {
        inputField.text = GameManager.Instance.InputPrimeString;

        if(inputField.text.Length >= 15)
        {
            inputField.pointSize = 70;
        }else if(inputField.text.Length > 10)
        {
            inputField.pointSize = 100;
        }
        else
        {
            inputField.pointSize = 120;
        }
        // 10자리 이상이면 사이즈 줄이기
    }
    /// <summary>
    /// 소인수 분해 결과 보이기
    /// </summary>
    void ResultPrime()
    {
        //범위 초과 수 예외 처리
        if(GameManager.Instance.InputPrimeString.Length >= 21)
        {
            outputField.text = "1844경 초과";
            return;
        }else if(GameManager.Instance.InputPrimeString.Length == 20)
        {
            if (IsUpperUlong(GameManager.Instance.InputPrimeString))
            {
                outputField.text = "1844경 초과";
                return;
            }
        }

        GameManager.Instance.InputPrime = ulong.Parse(GameManager.Instance.InputPrimeString);

        if(GameManager.Instance.InputPrime <= 1)
        {
            outputField.text = "소수, 합성수\n둘다 아닙니다.";
        }
        else
        {
            factorStringList = factorsPrimeClass.FactorizationPrimes(GameManager.Instance.InputPrime);//소인수 분해 진행
            ExponentialNotation(factorStringList);//지수 표기 방식으로 변환
            ShowResultFactorPrimeText(factorStringList);//결과 표시
        }
    }
    
    /// <summary>
    /// 기능 : ulong 범위 초과 여부
    /// </summary>
    /// <returns></returns>
    bool IsUpperUlong(string inputString)
    {
        string sub = inputString.Substring(0, 8);
        if(int.Parse(sub) >= maxInputNum)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 기능 : 입력 초기화
    /// </summary>
    public void InputInit()
    {
        factorStringList.Clear();
        GameManager.Instance.primeFactorCountDic.Clear();
        inputField.text = string.Empty;
        outputField.text = string.Empty;
        GameManager.Instance.InputPrimeString = string.Empty;
    }
    /// <summary>
    /// 기능 : 지수 표기 방식으로 변환
    /// </summary>
    void ExponentialNotation(List<ulong> factorStringList)
    {
        foreach(ulong fac in factorStringList)
        {
            //새로운 소인수 등록
            if (!GameManager.Instance.primeFactorCountDic.ContainsKey(fac))
            {
                GameManager.Instance.primeFactorCountDic.Add(fac,1);
            }//기존 소인수
            else
            {
                GameManager.Instance.primeFactorCountDic[fac] += 1;
            }
        }
    }
    /// <summary>
    /// 시능 : 소인수 분해 결과 보이기
    /// </summary>
    void ShowResultFactorPrimeText(List<ulong> factorStringList)
    {
        if (factorStringList.Count == 1)
        {
            outputField.text = factorStringList[0].ToString()+" (소수)";
        }
        else
        {
            foreach (var dic in GameManager.Instance.primeFactorCountDic)
            {
                if (dic.Value == 1)
                {
                    outputField.text += $"{dic.Key}";
                }
                else
                {
                //지수형식으로 표기
                    outputField.text += $"{dic.Key}<cspace=-0.1em></cspace><sup><size=180>{dic.Value}</size></sup>";
                }
                outputField.text += $" x ";
            }
            outputField.text = outputField.text.Substring(0, outputField.text.Length - 3);
        }
    }
    /// <summary>
    /// 기능 : 메인씬으로 돌아가기
    /// </summary>
    public void ReturnMain()
    {
        InputInit();//입력 초기화
        SceneManager.LoadScene("MainCalMode", LoadSceneMode.Single);
        SoundManager._sound.PlayBGM(0);
    }
}
