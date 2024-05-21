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
    [SerializeField] TextMeshProUGUI musicBPMText;
    [SerializeField] TextMeshProUGUI musicBestScore;
    [SerializeField] TextMeshProUGUI musicBestRank;
    public List<Button> musicList;
    public List<Image> musicLevelImage;

    public int[] musicLevelList;
    void Awake()
    {
        musicTitleText.text = "";
        musicBPMText.text = " = ?";
        musicLevelList = new int[9] { 1, 4, 7, 5, 8,10,8,9,3 };
        for (int i = 0; i < 10; i++) musicLevelImage[i].enabled = false;
        if(UserDataManager.userData != null) UserDataManager.userData.LoadData();
    }
    public void MusicSetting(int idx)
    {
        GameManager.Instance.MusicNumber = idx;
        musicTitleText.text = musicList[idx].GetComponentInChildren<TextMeshProUGUI>().text;
        musicBPMText.text = " = "+GameManager.Instance.MusicBPMList[idx].ToString();
        musicBestRank.text = UserDataManager.musicListDatas.musicList[idx].rank.ToString();
        musicBestRank.color = RankColor(musicBestRank.text);
        if (UserDataManager.musicListDatas.musicList[idx].score == 0) musicBestScore.text = " : ?";
        else musicBestScore.text = " : " + UserDataManager.musicListDatas.musicList[idx].score.ToString();
        GameManager.Instance.MusicName = musicTitleText.text;
    }
    Color RankColor(string rank)
    {
        switch (rank)
        {
            case "S":
                return new Color(0.2f, 0.9f, 0.9f);
            case "A":
                return Color.red;
            case "B":
                return new Color(1f, 0.5f, 0);
            case "C":
                return Color.green;
            case "D":
                return Color.blue;
            case "F":
                return Color.gray;
        }
        return Color.white;
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
