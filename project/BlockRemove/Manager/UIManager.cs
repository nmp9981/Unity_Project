using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _restBlockCountText;
    [SerializeField] TextMeshProUGUI _scoreText;
    [SerializeField] public Button _startButton;
    [SerializeField] public GameObject _gameOverUI;
    [SerializeField] public GameObject _getReadyUI;
    [SerializeField] public GameObject BGMOffImage;
    [SerializeField] public GameObject SFXOffImage;

    bool _isBGMOn;
    bool _isSFXOn;

    private void Awake()
    {
        _isBGMOn = true;
        _isSFXOn = true;
        GameManager.Instance.BGMVolume = 1.0f;
        GameManager.Instance.SFXVolume = 1.0f;
    }
    void Update()
    {
        _restBlockCountText.text = $"Count : {GameManager.Instance.RestBlockCount}";
        _scoreText.text = $"Score : {GameManager.Instance.Score}";
    }
    public void BGMButton()
    {
        _isBGMOn = !_isBGMOn;
        if (_isBGMOn)
        {
            GameManager.Instance.BGMVolume = 1.0f;
            BGMOffImage.SetActive(false);
        }
        else
        {
            GameManager.Instance.BGMVolume = 0f;
            BGMOffImage.SetActive(true);
        }
    }
    public void SFXButton()
    {
        _isSFXOn = !_isSFXOn;
        if (_isSFXOn)
        {
            GameManager.Instance.SFXVolume = 1.0f;
            SFXOffImage.SetActive(false);
        }
        else
        {
            GameManager.Instance.SFXVolume = 0f;
            SFXOffImage.SetActive(true);
        }
    }
}
