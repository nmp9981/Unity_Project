using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum KartColor
{
    White,
    Red,
    Yellow,
    Green,
    Blue,
    Brown,
    Purple,
    Black,
    Count
}

public class StoreUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI playerLucciText;

    void Awake()
    {
        BindingStoreUIButton();
    }
    private void OnEnable()
    {
        ShowPlayerLucci();
    }
    /// <summary>
    /// 기능 : 상점UI에 있는 버튼 바인딩
    /// </summary>
    void BindingStoreUIButton()
    {
        foreach (var gm in gameObject.GetComponentsInChildren<Button>(true))
        {
            string gmName = gm.gameObject.name;
            string gmTag = gm.gameObject.tag;

            if (gmTag == "ColorButton")
            {
                string payLucciText = gm.GetComponentInChildren<TextMeshProUGUI>().text;
                gm.onClick.AddListener(() => BuyKartColor(gmName, payLucciText));
            }
        }
    }
    /// <summary>
    /// 기능 : 물품 구매
    /// </summary>
    /// <param name="gmName"></param>
    public void BuyKartColor(string gmName, string payLucciText)
    {
        //구매 가격
        int payLucciTextLen = payLucciText.Length;
        int payLucci = int.Parse(payLucciText.Substring(0, payLucciTextLen - 6));
        //구매 가능한가?
        if(payLucci <= GameManager.Instance.PlayerLucci)
        {
            GameManager.Instance.PlayerLucci -= payLucci;
            //구매 확정 UI
        }
        else
        {
            //구매 불가 UI
            return;
        }
        // 구매 여부 기록
        switch (gmName)
        {
            case "White":
                GameManager.Instance.isHaveColor[(int)KartColor.White] = true;
                break;
            case "Red":
                GameManager.Instance.isHaveColor[(int)KartColor.Red] = true;
                break;
            case "Yellow":
                GameManager.Instance.isHaveColor[(int)KartColor.Yellow] = true;
                break;
            case "Green":
                GameManager.Instance.isHaveColor[(int)KartColor.Green] = true;
                break;
            case "Blue":
                GameManager.Instance.isHaveColor[(int)KartColor.Blue] = true;
                break;
            case "Brown":
                GameManager.Instance.isHaveColor[(int)KartColor.Brown] = true;
                break;
            case "Purple":
                GameManager.Instance.isHaveColor[(int)KartColor.Purple] = true;
                break;
            case "Black":
                GameManager.Instance.isHaveColor[(int)KartColor.Black] = true;
                break;
            default:
                break;

        }
        ShowPlayerLucci();
    }
    /// <summary>
    /// 기능 : 플레이어가 가지고 있는 돈
    /// </summary>
    void ShowPlayerLucci()
    {
        playerLucciText.text = GameManager.Instance.PlayerLucci.ToString();
    }
}
