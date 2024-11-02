using System.Collections;
using System.Collections.Generic;
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
    //각 색상을 가지는가?
    public Dictionary<int, bool> isHaveColor;
    void Awake()
    {
        SettingColorDic();
        BindingStoreUIButton();
    }
    void SettingColorDic()
    {
        for(int i = 0; i < 8; i++)
        {
            isHaveColor.Add(i, false);
        }
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
                gm.onClick.AddListener(() => BuyKartColor(gmName));
            }
        }
    }
    public void BuyKartColor(string gmName)
    {
        //구매
        //돈이 충분하면 구매
        Debug.Log(KartColor.White.ToString());
        switch (gmName)
        {
            case "White":
                isHaveColor[(int)KartColor.White] = true;
                break;
            case "Red":
                isHaveColor[(int)KartColor.Red] = true;
                break;
            case "Yellow":
                isHaveColor[(int)KartColor.Yellow] = true;
                break;
            case "Green":
                isHaveColor[(int)KartColor.Green] = true;
                break;
            case "Blue":
                isHaveColor[(int)KartColor.Blue] = true;
                break;
            case "Brown":
                isHaveColor[(int)KartColor.Brown] = true;
                break;
            case "Purple":
                isHaveColor[(int)KartColor.Purple] = true;
                break;
            case "Black":
                isHaveColor[(int)KartColor.Black] = true;
                break;
            default:
                break;

        }
    }
  
}
