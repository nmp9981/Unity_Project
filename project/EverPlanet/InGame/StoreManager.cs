using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;

public class StoreManager : MonoBehaviour
{
    [SerializeField] GameObject StoreUI;

    [SerializeField] TextMeshProUGUI myMesoText;
    // Update is called once per frame
    void Update()
    {
        OpenStore();
        ShowStore();
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
}
