using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PrimeModeKeySetting : MonoBehaviour
{
    [SerializeField]
    GameObject KeyButtonSet;

    [SerializeField]
    Button resultButton;

    [SerializeField]
    Button backButton;
    
    [SerializeField]
    TMP_InputField inputField;

    [SerializeField]
    TextMeshProUGUI outputField;

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
    }
    /// <summary>
    /// 소인수 분해 결과 보이기
    /// </summary>
    void ResultPrime()
    {
        GameManager.Instance.InputPrime = ulong.Parse(GameManager.Instance.InputPrimeString);
        outputField.text = GameManager.Instance.InputPrimeString;
    }
    /// <summary>
    /// 기능 : 입력 초기화
    /// </summary>
    public void InputInit()
    {
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
