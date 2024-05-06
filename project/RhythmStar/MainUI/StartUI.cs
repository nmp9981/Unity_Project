using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartUI : MonoBehaviour
{
    [SerializeField] GameObject settingUI;
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider sfxSlider;

    private void Start()
    {
        bgmSlider.value = GameManager.Instance.BGMVolume;
        sfxSlider.value = GameManager.Instance.SFXVolume;
    }
    private void Update()
    {
        SetVolume();
    }
    void SetVolume()
    {
        GameManager.Instance.BGMVolume = bgmSlider.value;
        GameManager.Instance.SFXVolume = sfxSlider.value;
    }
    public void GoGame()
    {
        SceneManager.LoadScene("MainUI");
    }
    public void GameSetting()
    {
        settingUI.SetActive(true);
    }
    public void CloseSetting()
    {
        settingUI.SetActive(false);
    }
}
