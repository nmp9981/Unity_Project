using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class DataSave : MonoBehaviour
{
    int currentPlayerLv;
    void Awake()
    {
        //DataGet();
    }
    void Update()
    {
        //DataSet();
    }
    void DataGet()
    {
        if (PlayerPrefs.HasKey("Lv")) GameManager.Instance.PlayerLV = PlayerPrefs.GetInt("Lv");
        else GameManager.Instance.PlayerLV = 10;
        if (PlayerPrefs.HasKey("Meso")) GameManager.Instance.PlayerMeso = PlayerPrefs.GetInt("Meso");
        else GameManager.Instance.PlayerMeso = 10000;
    }
    void DataSet()
    {
        PlayerPrefs.SetInt("Lv", GameManager.Instance.PlayerLV);
        PlayerPrefs.SetInt("Meso", GameManager.Instance.PlayerMeso);
    }
}
