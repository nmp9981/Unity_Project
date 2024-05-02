using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI musicTitleText;
    public List<Button> musicList;
    
    void Awake()
    {
        musicTitleText.text = "";
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
}
