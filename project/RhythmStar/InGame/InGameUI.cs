using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _scoreText;
    [SerializeField] TextMeshProUGUI _comboText;
    [SerializeField] TextMeshProUGUI _comboTitle;
    [SerializeField] TextMeshProUGUI _comboBonusText;
    [SerializeField] TextMeshProUGUI _currentHPText;
    [SerializeField] TextMeshProUGUI _startTimeText;
    [SerializeField] Image hpBar;

    int[] bounusCut = new int[4]{ 20,40,70,100};
   
    void Awake()
    {
        GameManager.Instance.HealthPoint = GameManager.Instance.MaxHealthPoint;
        StartCoroutine(ShowStartText());
    }

    // Update is called once per frame
    void Update()
    {
        ComboBonusJudge();
        ShowText();
        ShowHP();
    }
    //3초뒤 시작
    IEnumerator ShowStartText()
    {
        _startTimeText.text = "";
        yield return new WaitForSecondsRealtime(0.5f);
        _startTimeText.text = "3";
        yield return new WaitForSecondsRealtime(1f);
        _startTimeText.text = "2";
        yield return new WaitForSecondsRealtime(1f);
        _startTimeText.text = "1";
        yield return new WaitForSecondsRealtime(1f);
        _startTimeText.text = "Start!!";
        yield return new WaitForSecondsRealtime(1f);
        GameManager.Instance.IsPlayGame = true;//이때 게임이 시작해야 함
        SoundManager._sound.PlayBGM(GameManager.Instance.MusicNumber);
        _startTimeText.text = "";
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
        if (GameManager.Instance.ComboCount < bounusCut[0])
        {
            GameManager.Instance.ComboBonus = 1;
            _comboBonusText.text = "";
        }else if(GameManager.Instance.ComboCount < bounusCut[1] && GameManager.Instance.ComboCount >= bounusCut[0])
        {
            GameManager.Instance.ComboBonus = 2;
            _comboBonusText.text = "X2";
            _comboBonusText.color = Color.blue;
        }
        else if (GameManager.Instance.ComboCount < bounusCut[2] && GameManager.Instance.ComboCount >= bounusCut[1])
        {
            GameManager.Instance.ComboBonus = 3;
            _comboBonusText.text = "X3";
            _comboBonusText.color = Color.green;
        }
        else if (GameManager.Instance.ComboCount < bounusCut[3] && GameManager.Instance.ComboCount >= bounusCut[2])
        {
            GameManager.Instance.ComboBonus = 4;
            _comboBonusText.text = "X4";
            _comboBonusText.color = Color.red;
        }
        else if (GameManager.Instance.ComboCount >= bounusCut[3])
        {
            GameManager.Instance.ComboBonus = 5;
            _comboBonusText.text = "X5";
            _comboBonusText.color = Color.magenta;
        }
    }
    void ShowHP()
    {
        GameManager.Instance.HealthPoint = Mathf.Min(GameManager.Instance.MaxHealthPoint, GameManager.Instance.HealthPoint);
        GameManager.Instance.HealthPoint = Mathf.Max(0f, GameManager.Instance.HealthPoint);
        hpBar.fillAmount = GameManager.Instance.HealthPoint / GameManager.Instance.MaxHealthPoint;
        _currentHPText.text = Mathf.Round(GameManager.Instance.HealthPoint).ToString()+"%";
    }
}
