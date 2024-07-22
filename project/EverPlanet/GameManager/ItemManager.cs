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
    [SerializeField] GameObject storeManager;

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
    }
    //단축키 등록
    public void ItemRegister()
    {
        if(GameManager.Instance.ItemKinds == ItemKind.HPHeal)
        {
            GameObject gm = GameObject.Find("a");
            int imageIdx = 0;
            if (GameManager.Instance.SelectedItemName == "White Possion")
            {
                imageIdx = 0;
                GameManager.Instance.HPPosionHealAmount = 300;
            }
            else if (GameManager.Instance.SelectedItemName == "Broiled eels")
            {
                imageIdx = 2;
                GameManager.Instance.HPPosionHealAmount = 1000;
            }
            else if (GameManager.Instance.SelectedItemName == "Ice Bar")
            {
                imageIdx = 4;
                GameManager.Instance.HPPosionHealAmount = 2000;
            }
            else if (GameManager.Instance.SelectedItemName == "Cheeze")
            {
                imageIdx = 6;
                GameManager.Instance.HPPosionHealAmount = 4000;
            }
           
            gm.GetComponent<Image>().sprite = storeManager.GetComponent<StoreManager>().itemImageSpriteList[imageIdx];
            GameManager.Instance.HPPosionCount = GameManager.Instance.storeItemList[imageIdx].theNumber;
            GameManager.Instance.KeyHPIdx = imageIdx;
        }
        else if (GameManager.Instance.ItemKinds == ItemKind.MPHeal)
        {
            GameObject gm = GameObject.Find("d");
            int imageIdx = 0;
            if (GameManager.Instance.SelectedItemName == "Mana Elixer")
            {
                imageIdx = 1;
                GameManager.Instance.MPPosionHealAmount = 300;
            }
            else if (GameManager.Instance.SelectedItemName == "Clear Water")
            {
                imageIdx = 3;
                GameManager.Instance.MPPosionHealAmount = 800;
            }
            else if (GameManager.Instance.SelectedItemName == "Patbingsu")
            {
                imageIdx = 5;
                GameManager.Instance.MPPosionHealAmount = 2000;
            }
            
            gm.GetComponent<Image>().sprite = storeManager.GetComponent<StoreManager>().itemImageSpriteList[imageIdx];
            GameManager.Instance.MPPosionCount = GameManager.Instance.storeItemList[imageIdx].theNumber;
            GameManager.Instance.KeyMPIdx = imageIdx;
        }
        else if (GameManager.Instance.ItemKinds == ItemKind.AttackUp)
        {
            if (GameManager.Instance.SelectedItemName == "Nimble's Pill")
            {
                int imageIdx = 7;
                GameObject gmU = GameObject.Find("U");
                gmU.GetComponent<Image>().sprite = storeManager.GetComponent<StoreManager>().itemImageSpriteList[imageIdx];
                GameManager.Instance.AvoidUPCount = GameManager.Instance.storeItemList[imageIdx].theNumber;
            }
            else if (GameManager.Instance.SelectedItemName == "Bowman's Pill")
            {
                int imageIdx = 8;
                GameObject gmY = GameObject.Find("Y");
                gmY.GetComponent<Image>().sprite = storeManager.GetComponent<StoreManager>().itemImageSpriteList[imageIdx];
                GameManager.Instance.AccUPCount = GameManager.Instance.storeItemList[imageIdx].theNumber;
            }
            else if (GameManager.Instance.SelectedItemName == "Drake's Blood")
            {
                int imageIdx = 9;
                GameObject gmW = GameObject.Find("W");
                gmW.GetComponent<Image>().sprite = storeManager.GetComponent<StoreManager>().itemImageSpriteList[imageIdx];
                GameManager.Instance.AttackUPCount = GameManager.Instance.storeItemList[imageIdx].theNumber;
            }
        }
        else if (GameManager.Instance.ItemKinds == ItemKind.Return)
        {
            GameObject gm = GameObject.Find("p");
            int imageIdx = 0;
            if (GameManager.Instance.SelectedItemName == "Return to village") imageIdx = 10;
            
            gm.GetComponent<Image>().sprite = storeManager.GetComponent<StoreManager>().itemImageSpriteList[imageIdx];
            gm.GetComponentInChildren<TextMeshProUGUI>().text = $"{GameManager.Instance.storeItemList[imageIdx].theNumber}";
        }
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
                    player.GetComponent<PlayerHeal>().HealHP(300,0);
                }
                break;
            case "Mana Elixer":
                if (GameManager.Instance.storeItemList[1].theNumber > 0)
                {
                    GameManager.Instance.storeItemList[1].theNumber -= 1;
                    player.GetComponent<PlayerHeal>().HealMP(300,1);
                }
                break;
            case "Broiled eels":
                if (GameManager.Instance.storeItemList[2].theNumber > 0)
                {
                    GameManager.Instance.storeItemList[2].theNumber -= 1;
                    player.GetComponent<PlayerHeal>().HealHP(1000,2);
                }
                break;
            case "Clear Water":
                if (GameManager.Instance.storeItemList[3].theNumber > 0)
                {
                    GameManager.Instance.storeItemList[3].theNumber -= 1;
                    player.GetComponent<PlayerHeal>().HealMP(800,3);
                }
                break;
            case "Ice Bar":
                if (GameManager.Instance.storeItemList[4].theNumber > 0)
                {
                    GameManager.Instance.storeItemList[4].theNumber -= 1;
                    player.GetComponent<PlayerHeal>().HealHP(2000,4);
                }
                break;
            case "Patbingsu":
                if (GameManager.Instance.storeItemList[5].theNumber > 0)
                {
                    GameManager.Instance.storeItemList[5].theNumber -= 1;
                    player.GetComponent<PlayerHeal>().HealMP(2000,5);
                }
                break;
            case "Cheeze":
                if (GameManager.Instance.storeItemList[6].theNumber > 0)
                {
                    GameManager.Instance.storeItemList[6].theNumber -= 1;
                    player.GetComponent<PlayerHeal>().HealHP(4000,6);
                }
                break;
            case "Nimble's Pill":
                if (GameManager.Instance.storeItemList[7].theNumber > 0)
                {
                    GameManager.Instance.storeItemList[7].theNumber -= 1;
                    StartCoroutine(player.GetComponent<PlayerBuff>().AttackBuff());
                }
                break;
            case "Bowman's Pill":
                if (GameManager.Instance.storeItemList[8].theNumber > 0)
                {
                    GameManager.Instance.storeItemList[8].theNumber -= 1;
                    StartCoroutine(player.GetComponent<PlayerBuff>().ACCBuff());
                }
                break;
            case "Drake's Blood":
                if (GameManager.Instance.storeItemList[9].theNumber > 0)
                {
                    GameManager.Instance.storeItemList[9].theNumber -= 1;
                    StartCoroutine(player.GetComponent<PlayerBuff>().AvoidBuff());
                }
                break;
            case "Return to village":
                if (GameManager.Instance.storeItemList[10].theNumber > 0)
                {
                    GameManager.Instance.storeItemList[10].theNumber -= 1;
                    player.GetComponent<PlayerHeal>().ReturnToVillege();
                }
                break;
        }
        itemOptionUI.SetActive(false);
    }
}
