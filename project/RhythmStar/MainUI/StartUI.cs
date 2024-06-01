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
        bgmSlider.value = PlayerPrefs.GetFloat("BGMVolume");
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");
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
        PlayerPrefs.SetFloat("BGMVolume", GameManager.Instance.BGMVolume);
        PlayerPrefs.SetFloat("SFXVolume", GameManager.Instance.SFXVolume);
        settingUI.SetActive(false);
    }
    public void GameExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
    }
}
