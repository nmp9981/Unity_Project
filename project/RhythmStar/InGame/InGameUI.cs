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
        ComboBonusJudge();
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
    void ComboBonusJudge()
    {
        if (GameManager.Instance.ComboCount < 20)
        {
            GameManager.Instance.ComboBonus = 1;
        }else if(GameManager.Instance.ComboCount < 40 && GameManager.Instance.ComboCount >= 20)
        {
            GameManager.Instance.ComboBonus = 2;
        }
        else if (GameManager.Instance.ComboCount < 70 && GameManager.Instance.ComboCount >= 40)
        {
            GameManager.Instance.ComboBonus = 3;
        }
        else if (GameManager.Instance.ComboCount < 100 && GameManager.Instance.ComboCount >= 70)
        {
            GameManager.Instance.ComboBonus = 4;
        }
        else if (GameManager.Instance.ComboCount >= 100)
        {
            GameManager.Instance.ComboBonus = 5;
        }
    }
}
