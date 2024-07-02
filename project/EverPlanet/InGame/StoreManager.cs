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

public class StoreManager : MonoBehaviour
{
    [SerializeField] GameObject StoreUI;
    [SerializeField] GameObject BuyInfoUI;

    [SerializeField] TextMeshProUGUI myMesoText;
    [SerializeField] List<Button> itemList;
    [SerializeField] TMP_InputField howMany;

    [SerializeField] GameObject mesoInsufficientImage;

    string itemText;
    string priceText;
    string manyText;

    private void Awake()
    {
        BuyInfoUI.SetActive(false);
        mesoInsufficientImage.SetActive(false);
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
            case "Broiled eels":
                GameManager.Instance.HPPosionCount += many;
                break;
            case "Mana elixir":
                GameManager.Instance.MPPosionCount += many;
                break;
            case "Drake's blood":
                GameManager.Instance.AttackUPCount += many;
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
