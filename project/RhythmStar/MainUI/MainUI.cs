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

    public int[] musicLevelList = new int[3] {1,5,10 };
    void Awake()
    {
        musicTitleText.text = "";
        musicBPMText.text = " = ?";
        for (int i = 0; i < 10; i++) musicLevelImage[i].enabled = false;
        if(UserDataManager.userData != null) UserDataManager.userData.LoadData();
    }
    public void MusicSetting(int idx)
    {
        GameManager.Instance.MusicNumber = idx;
        musicTitleText.text = musicList[idx].GetComponentInChildren<TextMeshProUGUI>().text;
        musicBPMText.text = " = "+GameManager.Instance.MusicBPMList[idx].ToString();
        musicBestRank.text = UserDataManager.userRankData[idx].rank.ToString();
        musicBestRank.color = RankColor(musicBestRank.text);
        if (UserDataManager.userRankData[idx].score == 0) musicBestScore.text = " : ?";
        else musicBestScore.text = " : " + UserDataManager.userRankData[idx].score.ToString();
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
