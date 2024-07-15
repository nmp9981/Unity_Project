using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    public List<Item> UserItemList = new List<Item>();
    public GameObject[] UserHaveItem = new GameObject[20];

    private void Awake()
    {
        
    }
    void Update()
    {
        ShowItemIcon();
    }
    //아이템 창 키면 보임
    void ShowItemIcon()
    {
        foreach(var item in UserItemList)
        {
            if (item.theNumber > 0)
            {
                for(int idx=0;idx<20;idx++)
                {
                    if (!UserHaveItem[idx].activeSelf)
                    {
                        UserHaveItem[idx].SetActive(true);
                        //UserHaveItem[idx].GetComponent<Image>() = itemImageList[item];
                        UserHaveItem[idx].GetComponentInChildren<TextMeshProUGUI>().text = item.theNumber.ToString();
                        break;
                    }
                   
                }
            }
        }
    }
}
