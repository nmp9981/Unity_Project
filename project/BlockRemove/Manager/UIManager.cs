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

    void Update()
    {
        _restBlockCountText.text = $"Count : {GameManager.Instance.RestBlockCount}";
        _scoreText.text = $"Score : {GameManager.Instance.Score}";
    }
}
