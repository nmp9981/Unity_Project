using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    [SerializeField] GameObject itemOptionUI;
    [SerializeField] GameObject canvas;

    public Item[] UserItemList = new Item[11];
    public GameObject[] UserHaveItem = new GameObject[20];
    public List<GameObject> UserHaveItemList = new List<GameObject>();
    private void Awake()
    {
        EmptyItemShow();
    }
    void Update()
    {
        ShowItemIcon();
        //ShowOption();
    }
    
    void EmptyItemShow()
    {
        for (int idx = 0; idx < 20; idx++)
        {
            UserHaveItem[idx].SetActive(false);
        }
        itemOptionUI.SetActive(false);
    }
 
    //아이템 창 키면 보임
    public void ShowItemIcon()
    {
        foreach (var item in GameManager.Instance.storeItemList)
        {
            
            if (!UserHaveItem[item.itemIdx].activeSelf) UserHaveItem[item.itemIdx].SetActive(true);
            UserHaveItem[item.itemIdx].GetComponent<Image>().sprite = item.itemSpriteImage;
            UserHaveItem[item.itemIdx].GetComponentInChildren<TextMeshProUGUI>().text = item.theNumber.ToString();
        }
        /*
        foreach (var item in GameManager.Instance.storeItemList)
        {
            if (item.theNumber > 0)
            {
                Debug.Log(item.itemIdx + " " + item.theNumber);

                for(int idx=0;idx<20;idx++)
                {
                    if (UserHaveItem[idx].activeSelf) continue;
                    if (!UserHaveItem[idx].activeSelf)
                    {
                        UserHaveItem[idx].SetActive(true);
                        UserHaveItem[idx].GetComponent<Image>().sprite = item.itemSpriteImage;
                        UserHaveItem[idx].GetComponentInChildren<TextMeshProUGUI>().text = item.theNumber.ToString();
                        return;
                    }
                    
                }
            }
        }
        */
    }
    //단축키 등록
}
