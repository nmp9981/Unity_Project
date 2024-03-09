using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    [SerializeField] GameObject _bgmSetting;
    [SerializeField] GameObject _sfxSetting;

    Slider _bgmSlider;
    Slider _sfxSlider;
    // Start is called before the first frame update
    void Start()
    {
        _bgmSlider = _bgmSetting.GetComponent<Slider>();
        _sfxSlider = _sfxSetting.GetComponent<Slider>();

        _bgmSlider.value = GameManager.Instance.BGMVolume;
        _sfxSlider.value = GameManager.Instance.SFXVolume;
    }

    // Update is called once per frame
    void Update()
    {
        GameManager.Instance.BGMVolume = _bgmSlider.value;
        GameManager.Instance.SFXVolume = _sfxSlider.value;
    }
}
