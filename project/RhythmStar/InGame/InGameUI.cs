using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InGameUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _scoreText;
    [SerializeField] TextMeshProUGUI _comboText;
    [SerializeField] TextMeshProUGUI _comboTitle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ShowText();
    }
    void ShowText()
    {
        _scoreText.text = GameManager.Instance.Score.ToString();
        if (GameManager.Instance.ComboCount >= 2)
        {
            _comboText.text = GameManager.Instance.ComboCount.ToString();
            _comboTitle.text = "COMBO";
        }
        else
        {
            _comboText.text = "";
            _comboTitle.text = "";
        }
    }
}
