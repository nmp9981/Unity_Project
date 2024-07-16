using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEditor.Progress;
using static UnityEngine.GraphicsBuffer;

public enum ConsumeItem
{
    WhitePossion,
    ManaElixir,
    BroiledEel,
    ClearWater,
    LceBar,
    Patbingsu,
    Cheeze,
    WarriorFill,
    BowmanFill,
    DrakeBlood,
    ReturnToVillage
}
public class StoreManager : MonoBehaviour
{
    [SerializeField] GameObject StoreUI;
    [SerializeField] GameObject BuyInfoUI;

    [SerializeField] TextMeshProUGUI myMesoText;
    [SerializeField] List<Button> itemList;
    [SerializeField] TMP_InputField howMany;

    [SerializeField] GameObject itemManager;
    [SerializeField] GameObject mesoInsufficientImage;
    [SerializeField] Sprite[] itemImageList = new Sprite[11];

    string itemText;
    string priceText;
    string manyText;

    private void Awake()
    {
        BuyInfoUI.SetActive(false);
        mesoInsufficientImage.SetActive(false);
        SettingItemImage();
    }
    void Update()
    {
        OpenStore();
        ShowStore();
        SelectItem();
    }
    void OpenStore()
    {
        if (Input.GetMouseButtonDown(1))
        { 
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.transform.name);
                if (hit.transform.tag == "Store")
                {
                    if (!StoreUI.activeSelf) StoreUI.SetActive(true);
                    else StoreUI.SetActive(false);
                }
            }
           
        }
    }
    void SettingItemImage()
    {
        GameManager.Instance.storeItemList[0].itemImage = itemImageList[0];
        GameManager.Instance.storeItemList[1].itemImage = itemImageList[1];
        GameManager.Instance.storeItemList[2].itemImage = itemImageList[2];
        GameManager.Instance.storeItemList[3].itemImage = itemImageList[3];
        GameManager.Instance.storeItemList[4].itemImage = itemImageList[4];
        GameManager.Instance.storeItemList[5].itemImage = itemImageList[5];
        GameManager.Instance.storeItemList[6].itemImage = itemImageList[6];
        GameManager.Instance.storeItemList[7].itemImage = itemImageList[7];
        GameManager.Instance.storeItemList[8].itemImage = itemImageList[8];
        GameManager.Instance.storeItemList[9].itemImage = itemImageList[9];
        GameManager.Instance.storeItemList[10].itemImage = itemImageList[10];
        for(int i=0;i<11;i++) GameManager.Instance.storeItemList[0].itemIdx = i;
    }
    void ShowStore()
    {
        myMesoText.text = string.Format("{0:#,0}", GameManager.Instance.PlayerMeso);
    }
    public void CloseStoreUI()
    {
        StoreUI.SetActive(false);
    }
    //실제 구매
    public void BuyItem()
    {
        int price = int.Parse(priceText.Substring(0, priceText.Length - 5));
        int many = int.Parse(manyText);

        int totalMeso = price * many;
        if (GameManager.Instance.PlayerMeso >= totalMeso) GameManager.Instance.PlayerMeso -= totalMeso;
        else mesoInsufficientImage.SetActive(true);

        switch (itemText)
        {
            case "White Possion":
                GameManager.Instance.storeItemList[0].theNumber += many;
                if(GameManager.Instance.storeItemList[0].theNumber == many)//0개였으면
                {
                    itemManager.GetComponent<ItemManager>().UserItemList.Add(GameManager.Instance.storeItemList[0]);
                }
                break;
            case "Mana elixir":
                GameManager.Instance.storeItemList[1].theNumber += many;
                if (GameManager.Instance.storeItemList[1].theNumber == many)//0개였으면
                {
                    itemManager.GetComponent<ItemManager>().UserItemList.Add(GameManager.Instance.storeItemList[1]);
                }
                GameManager.Instance.MPPosionCount += many;
                break;
            case "Drake's blood":
                GameManager.Instance.storeItemList[2].theNumber += many;
                if (GameManager.Instance.storeItemList[2].theNumber == many)//0개였으면
                {
                    itemManager.GetComponent<ItemManager>().UserItemList.Add(GameManager.Instance.storeItemList[2]);
                }
                GameManager.Instance.AttackUPCount += many;
                break;
            case "Broiled eels":
                GameManager.Instance.storeItemList[3].theNumber += many;
                if (GameManager.Instance.storeItemList[3].theNumber == many)//0개였으면
                {
                    itemManager.GetComponent<ItemManager>().UserItemList.Add(GameManager.Instance.storeItemList[3]);
                }
                GameManager.Instance.HPPosionCount += many;
                break;
            case "Clear Water":
                GameManager.Instance.storeItemList[4].theNumber += many;
                if (GameManager.Instance.storeItemList[4].theNumber == many)//0개였으면
                {
                    itemManager.GetComponent<ItemManager>().UserItemList.Add(GameManager.Instance.storeItemList[4]);
                }
                break;
            case "Ice Bar":
                GameManager.Instance.storeItemList[5].theNumber += many;
                if (GameManager.Instance.storeItemList[5].theNumber == many)//0개였으면
                {
                    itemManager.GetComponent<ItemManager>().UserItemList.Add(GameManager.Instance.storeItemList[5]);
                }
                break;
            case "Patbingsu":
                GameManager.Instance.storeItemList[6].theNumber += many;
                if (GameManager.Instance.storeItemList[6].theNumber == many)//0개였으면
                {
                    itemManager.GetComponent<ItemManager>().UserItemList.Add(GameManager.Instance.storeItemList[6]);
                }
                break;
            case "Cheeze":
                GameManager.Instance.storeItemList[7].theNumber += many;
                if (GameManager.Instance.storeItemList[7].theNumber == many)//0개였으면
                {
                    itemManager.GetComponent<ItemManager>().UserItemList.Add(GameManager.Instance.storeItemList[7]);
                }
                break;
            case "Warrior's Pill":
                GameManager.Instance.storeItemList[8].theNumber += many;
                if (GameManager.Instance.storeItemList[8].theNumber == many)//0개였으면
                {
                    itemManager.GetComponent<ItemManager>().UserItemList.Add(GameManager.Instance.storeItemList[8]);
                }
                break;
            case "Bowman's Pill":
                GameManager.Instance.storeItemList[9].theNumber += many;
                if (GameManager.Instance.storeItemList[9].theNumber == many)//0개였으면
                {
                    itemManager.GetComponent<ItemManager>().UserItemList.Add(GameManager.Instance.storeItemList[9]);
                }
                break;
            case "Return to village":
                GameManager.Instance.storeItemList[10].theNumber += many;
                if (GameManager.Instance.storeItemList[10].theNumber == many)//0개였으면
                {
                    itemManager.GetComponent<ItemManager>().UserItemList.Add(GameManager.Instance.storeItemList[10]);
                }
                break;
        }

        BuyInfoUI.SetActive(false);
    }
    public void BuyInfoOpen()
    {
        if(BuyInfoUI.activeSelf) BuyInfoUI.SetActive(false);
        else BuyInfoUI.SetActive(true);
    }
    public void BuyInfoClose()
    {
        BuyInfoUI.SetActive(false);
    }
    public void MesoInsufficientClose()
    {
        mesoInsufficientImage.SetActive(false);
    }
    void SelectItem()
    {
        GameObject clickObj = EventSystem.current.currentSelectedGameObject;
        if (clickObj != null && clickObj.tag == "Item")
        {
            int idx = 0;
            foreach(Transform tr in clickObj.GetComponentInChildren<Transform>())
            {
                if(idx==0) itemText = tr.gameObject.GetComponent<TextMeshProUGUI>().text;
                if(idx==1) priceText = tr.gameObject.GetComponent<TextMeshProUGUI>().text;
                idx++;
            }
        }
        howMany.onValueChanged.AddListener(delegate { ManyText();});
    }
    public void ManyText()
    {
        manyText = howMany.GetComponent<TMP_InputField>().text;
    }
}
