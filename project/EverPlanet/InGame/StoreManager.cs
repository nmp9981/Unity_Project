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
    [SerializeField] public Sprite[] itemImageSpriteList = new Sprite[11];

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
        for (int i = 0; i < 11; i++)
        {
            GameManager.Instance.storeItemList[i].itemIdx = i;
            GameManager.Instance.storeItemList[i].itemSpriteImage = itemImageSpriteList[i];
        }
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
        if (GameManager.Instance.PlayerMeso >= totalMeso)
        {
            GameManager.Instance.PlayerMeso -= totalMeso;

            switch (itemText)
            {
                case "White Possion":
                    GameManager.Instance.storeItemList[0].theNumber += many;
                    break;
                case "Mana Elixer":
                    GameManager.Instance.storeItemList[1].theNumber += many;
                    break;
                case "Drake's Blood":
                    GameManager.Instance.storeItemList[9].theNumber += many;
                    GameManager.Instance.AttackUPCount += many;
                    break;
                case "Broiled eels":
                    GameManager.Instance.storeItemList[2].theNumber += many;
                    break;
                case "Clear Water":
                    GameManager.Instance.storeItemList[3].theNumber += many;
                    break;
                case "Ice Bar":
                    GameManager.Instance.storeItemList[4].theNumber += many;
                    break;
                case "Patbingsu":
                    GameManager.Instance.storeItemList[5].theNumber += many;
                    break;
                case "Cheeze":
                    GameManager.Instance.storeItemList[6].theNumber += many;
                    break;
                case "Nimble's Pill":
                    GameManager.Instance.storeItemList[7].theNumber += many;
                    GameManager.Instance.AvoidUPCount += many;
                    break;
                case "Bowman's Pill":
                    GameManager.Instance.storeItemList[8].theNumber += many;
                    GameManager.Instance.AccUPCount += many;
                    break;
                case "Return to village":
                    GameManager.Instance.storeItemList[10].theNumber += many;
                    GameManager.Instance.ReturnVillegeCount += many;
                    break;
            }
        }
        else mesoInsufficientImage.SetActive(true);

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
