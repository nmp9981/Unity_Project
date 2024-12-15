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
            outputField.text = "Possible to 1844Kyeong";
            return;
        }else if(GameManager.Instance.InputPrimeString.Length == 20)
        {
            if (IsUpperUlong(GameManager.Instance.InputPrimeString))
            {
                outputField.text = "Possible to 1844Kyeong";
                return;
            }
        }

        GameManager.Instance.InputPrime = ulong.Parse(GameManager.Instance.InputPrimeString);
        Debug.Log(GameManager.Instance.InputPrime+" 입력 숫자");
        if(GameManager.Instance.InputPrime <= 1)
        {
            outputField.text = "Not Prime";
        }
        else
        {
            factorStringList = factorsPrimeClass.FactorizationPrimes(GameManager.Instance.InputPrime);
            outputField.text += factorStringList[0].ToString();
            if(factorStringList.Count >= 2)
            {
                for(int idx = 1;idx < factorStringList.Count;idx++)
                {
                    outputField.text += $" x {factorStringList[idx].ToString()}";
                }
            }
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
        inputField.text = string.Empty;
        outputField.text = string.Empty;
        GameManager.Instance.InputPrimeString = string.Empty;
    }
    /// <summary>
    /// 기능 : 메인씬으로 돌아가기
    /// </summary>
    public void ReturnMain()
    {
        SceneManager.LoadScene("MainCalMode", LoadSceneMode.Single);
        SoundManager._sound.PlayBGM(0);
    }
}
