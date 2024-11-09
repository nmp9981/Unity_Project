using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MyRoomUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI showCurrentLucciText;
    [SerializeField]
    List<GameObject> allColorListButtonObject = new List<GameObject>();
   
    void Awake()
    {
        BidingMyRoomUIButton();
    }
    private void OnEnable()
    {
        ShowPlayerLucci();
        ShowCurrentHaveColor();
    }
    /// <summary>
    /// 현재 가지고 있는 색상
    /// </summary>
    void ShowCurrentHaveColor()
    {
        for(int idx=0;idx< GameManager.Instance.isHaveColor.Count;idx++)
        {
            //가지고 있음
            if (GameManager.Instance.isHaveColor[idx])
            {
                allColorListButtonObject[idx].SetActive(true);
            }//가지고 있지 않음
            else
            {
                allColorListButtonObject[idx].SetActive(false);
            }
        }
    }
    /// <summary>
    /// 마이룸 UI버튼 바인딩
    /// </summary>
    void BidingMyRoomUIButton()
    {
        foreach (var gm in gameObject.GetComponentsInChildren<Button>(true))
        {
            string gmName = gm.gameObject.name;
            string gmTag = gm.gameObject.tag;

            if (gmTag == "ColorSelectButton")
            {
                string payLucciText = gm.GetComponentInChildren<TextMeshProUGUI>().text;
                gm.onClick.AddListener(() => SetKartColor(gmName));
            }
        }
    }
    void SetKartColor(string gmName)
    {
        switch (gmName)
        {
            case "White":
                GameManager.Instance.CurrentKartColor = Color.white;
                break;
            case "Red":
                GameManager.Instance.CurrentKartColor = Color.red;
                break;
            case "Yellow":
                GameManager.Instance.CurrentKartColor = Color.yellow;
                break;
            case "Green":
                GameManager.Instance.CurrentKartColor = Color.green;
                break;
            case "Blue":
                GameManager.Instance.CurrentKartColor = Color.blue;
                break;
            case "Brown":
                GameManager.Instance.CurrentKartColor = new Color(113f/255f,72f/255f,20f/255f);
                break;
            case "Purple":
                GameManager.Instance.CurrentKartColor = new Color(128f / 255f, 65f / 255f, 217f / 255f);
                break;
            case "Black":
                GameManager.Instance.CurrentKartColor = Color.black;
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 기능 : 플레이어가 가지고 있는 돈
    /// </summary>
    void ShowPlayerLucci()
    {
        showCurrentLucciText.text = GameManager.Instance.PlayerLucci.ToString();
    }
}
