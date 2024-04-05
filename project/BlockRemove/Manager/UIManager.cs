using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _restBlockCountText;
    [SerializeField] public Button _startButton;
    [SerializeField] public GameObject _gameOverUI;
    [SerializeField] public GameObject _getReadyUI;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _restBlockCountText.text = $"Count : {GameManager.Instance.RestBlockCount}";
    }
}
