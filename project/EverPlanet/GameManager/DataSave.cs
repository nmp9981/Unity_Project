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
        
        if (PlayerPrefs.HasKey("AP")) GameManager.Instance.ApPoint = PlayerPrefs.GetInt("AP");
        else GameManager.Instance.ApPoint = 0;
        if (PlayerPrefs.HasKey("SP")) GameManager.Instance.SkillPoint = PlayerPrefs.GetInt("SP");
        else GameManager.Instance.SkillPoint = 0;
    }
    void DataSet()
    {
        PlayerPrefs.SetInt("Lv", GameManager.Instance.PlayerLV);
        PlayerPrefs.SetInt("Meso", GameManager.Instance.PlayerMeso);
        PlayerPrefs.SetInt("AP", GameManager.Instance.ApPoint);
        PlayerPrefs.SetInt("SP", GameManager.Instance.SkillPoint);
    }
}
