using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ItemKind
{
    HPHeal,
    MPHeal,
    AttackUp,
    AccUp,
    Return
}
public class ItemManager : MonoBehaviour
{
    [SerializeField] GameObject itemOptionUI;
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject player;

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
    public void ItemRegister()
    {

    }
    //사용하기
    public void UseItem()
    {
        switch (GameManager.Instance.SelectedItemName)
        {
            case "White Possion":
                if (GameManager.Instance.storeItemList[0].theNumber > 0)
                {
                    GameManager.Instance.storeItemList[0].theNumber -= 1;
                    player.GetComponent<PlayerHeal>().HealHP(300);
                }
                break;
            case "Mana Elixer":
                if (GameManager.Instance.storeItemList[1].theNumber > 0)
                {
                    GameManager.Instance.storeItemList[1].theNumber -= 1;
                    player.GetComponent<PlayerHeal>().HealMP(300);
                }
                break;
            case "Broiled eels":
                if (GameManager.Instance.storeItemList[2].theNumber > 0)
                {
                    GameManager.Instance.storeItemList[2].theNumber -= 1;
                    player.GetComponent<PlayerHeal>().HealHP(1000);
                }
                break;
            case "Clear Water":
                if (GameManager.Instance.storeItemList[3].theNumber > 0)
                {
                    GameManager.Instance.storeItemList[3].theNumber -= 1;
                    player.GetComponent<PlayerHeal>().HealMP(800);
                }
                break;
            case "Ice Bar":
                if (GameManager.Instance.storeItemList[4].theNumber > 0)
                {
                    GameManager.Instance.storeItemList[4].theNumber -= 1;
                    player.GetComponent<PlayerHeal>().HealHP(2000);
                }
                break;
            case "Patbingsu":
                if (GameManager.Instance.storeItemList[5].theNumber > 0)
                {
                    GameManager.Instance.storeItemList[5].theNumber -= 1;
                    player.GetComponent<PlayerHeal>().HealMP(2000);
                }
                break;
            case "Cheeze":
                if (GameManager.Instance.storeItemList[6].theNumber > 0)
                {
                    GameManager.Instance.storeItemList[6].theNumber -= 1;
                    player.GetComponent<PlayerHeal>().HealHP(4000);
                }
                break;
            case "Warrior's Pill":
                if (GameManager.Instance.storeItemList[7].theNumber > 0) GameManager.Instance.storeItemList[7].theNumber -= 1;
                break;
            case "Bowman's Pill":
                if (GameManager.Instance.storeItemList[8].theNumber > 0) GameManager.Instance.storeItemList[8].theNumber -= 1;
                break;
            case "Drake's blood":
                if (GameManager.Instance.storeItemList[9].theNumber > 0) GameManager.Instance.storeItemList[9].theNumber -= 1;
                break;
            case "Return to village":
                if (GameManager.Instance.storeItemList[10].theNumber > 0)
                {
                    GameManager.Instance.storeItemList[10].theNumber -= 1;
                    player.GetComponent<PlayerHeal>().ReturnToVillege();
                }
                break;
        }
    }
}
