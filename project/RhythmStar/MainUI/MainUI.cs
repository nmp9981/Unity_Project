using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI musicTitleText;
    public List<Button> musicList;
    public List<Image> musicLevelImage;

    public int[] musicLevelList = new int[3] {1,5,10 };
    void Awake()
    {
        musicTitleText.text = "";
        for (int i = 0; i < 10; i++) musicLevelImage[i].enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void MusicSetting(int idx)
    {
        GameManager.Instance.MusicNumber = idx;
        musicTitleText.text = musicList[idx].GetComponentInChildren<TextMeshProUGUI>().text;
    }
    public void GameStartButton()
    {
        SceneManager.LoadScene("RhythmStarIngame");
    }
    public void BackButton()
    {
        SceneManager.LoadScene("StartUI");
    }
    public void ShowLevel(int idx)
    {
        GameManager.Instance.MusicLevel = musicLevelList[idx];
        for (int i = 0; i < 10; i++) musicLevelImage[i].enabled = false;
        for (int i = 0; i < GameManager.Instance.MusicLevel; i++) musicLevelImage[i].enabled = true;
    }
}
