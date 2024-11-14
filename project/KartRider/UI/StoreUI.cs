using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
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
public enum KartSkid
{
    Basic,
    Orange,
    Red,
    Yellow,
    Green,
    Blue,
    Purple,
    Rainbow,
    Count
}
public enum KartName
{
    Basic,
    Cotton,
    Police,
    Taxi,
    Count
}

public class StoreUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI playerLucciText;

    GameObject kartListObj;
    GameObject skidListObj;
    GameObject paintListObj;

    void Awake()
    {
        RegisterObjectList();
        BindingStoreUIButton();
    }
    private void OnEnable()
    {
        ShowPlayerLucci();
    }
    /// <summary>
    /// 기능 : 오브젝트 리스트 등록
    /// </summary>
    void RegisterObjectList()
    {
        foreach (var gm in gameObject.GetComponentsInChildren<Transform>(true))
        {
            string gmName = gm.gameObject.name;
            switch (gmName)
            {
                case "KartList":
                    kartListObj = gm.gameObject;
                    break;
                case "SkidList":
                    skidListObj = gm.gameObject;
                    break;
                case "PaintList":
                    paintListObj = gm.gameObject;
                    break;
                default:
                    break;
            }
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
                string payLucciText = gm.GetComponentInChildren<TextMeshProUGUI>().text;
                gm.onClick.AddListener(() => BuyKartColor(gmName, payLucciText));
            }else if(gmTag == "SkidSelectedButton")
            {
                string payLucciText = gm.GetComponentInChildren<TextMeshProUGUI>().text;
                gm.onClick.AddListener(() => BuySkidMark(gmName, payLucciText));
            }else if (gmTag == "KartSelectedButton")
            {
                string payLucciText = gm.GetComponentInChildren<TextMeshProUGUI>().text;
                gm.onClick.AddListener(() => BuyKart(gmName, payLucciText));
            }
            else
            {
                switch (gmName)
                {
                    case "KartTab":
                        gm.onClick.AddListener(() => ShowObjectList(kartListObj));
                        break;
                    case "SkidMarkTab":
                        gm.onClick.AddListener(() => ShowObjectList(skidListObj));
                        break;
                    case "PaintTab":
                        gm.onClick.AddListener(() => ShowObjectList(paintListObj));
                        break;
                    default:
                        break;
                }
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
    /// 기능 : 스키드 마크 물품 구매
    /// </summary>
    /// <param name="gmName">제품명</param>
    public void BuySkidMark(string gmName, string payLucciText)
    {
        //구매 가격
        int payLucciTextLen = payLucciText.Length;
        int payLucci = int.Parse(payLucciText.Substring(0, payLucciTextLen - 6));
        //구매 가능한가?
        if (payLucci <= GameManager.Instance.PlayerLucci)
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
            case "Orange":
                GameManager.Instance.isHaveSkidMark[(int)KartSkid.Orange] = true;
                break;
            case "Red":
                GameManager.Instance.isHaveSkidMark[(int)KartSkid.Red] = true;
                break;
            case "Yellow":
                GameManager.Instance.isHaveSkidMark[(int)KartSkid.Yellow] = true;
                break;
            case "Green":
                GameManager.Instance.isHaveSkidMark[(int)KartSkid.Green] = true;
                break;
            case "Blue":
                GameManager.Instance.isHaveSkidMark[(int)KartSkid.Blue] = true;
                break;
            case "Purple":
                GameManager.Instance.isHaveSkidMark[(int)KartSkid.Purple] = true;
                break;
            case "Rainbow":
                GameManager.Instance.isHaveSkidMark[(int)KartSkid.Rainbow] = true;
                break;
            default:
                break;

        }
        ShowPlayerLucci();
    }
    /// <summary>
    /// 기능 : 카트 구매
    /// </summary>
    /// <param name="gmName">제품명</param>
    public void BuyKart(string gmName, string payLucciText)
    {
        //구매 가격
        int payLucciTextLen = payLucciText.Length;
        int payLucci = int.Parse(payLucciText.Substring(0, payLucciTextLen - 6));
        //구매 가능한가?
        if (payLucci <= GameManager.Instance.PlayerLucci)
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
            case "Cotton":
                GameManager.Instance.isHaveKart[(int)KartName.Cotton] = true;
                break;
            case "Red":
                GameManager.Instance.isHaveKart[(int)KartName.Police] = true;
                break;
            case "Yellow":
                GameManager.Instance.isHaveKart[(int)KartName.Taxi] = true;
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
    /// <summary>
    /// 기능 : 리스트 오브젝트 끄고 키기
    /// </summary>
    /// <param name="listObj">탭 메뉴에 따라 다름</param>
    void ShowObjectList(GameObject listObj)
    {
        kartListObj.SetActive(false);
        skidListObj.SetActive(false);
        paintListObj.SetActive(false);
        listObj.SetActive(true);
    }
}
