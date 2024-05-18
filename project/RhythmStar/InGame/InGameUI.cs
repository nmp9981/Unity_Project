using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InGameUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _scoreText;
    [SerializeField] TextMeshProUGUI _comboText;
    [SerializeField] TextMeshProUGUI _comboTitle;
    [SerializeField] TextMeshProUGUI _comboBonusText;
    [SerializeField] TextMeshProUGUI _currentHPText;
    [SerializeField] TextMeshProUGUI _startTimeText;

    [SerializeField] TextMeshProUGUI _maxComboText;
    [SerializeField] TextMeshProUGUI _perfectCountText;
    [SerializeField] TextMeshProUGUI _greatCountText;
    [SerializeField] TextMeshProUGUI _goodCountText;
    [SerializeField] TextMeshProUGUI _missCountText;
    [SerializeField] TextMeshProUGUI _scoreResultText;
    [SerializeField] TextMeshProUGUI _rankResultText;

    [SerializeField] Image hpBar;
    [SerializeField] GameObject gameOverUI;
    [SerializeField] GameObject gameClearUI;

    int[] bounusCut = new int[4]{ 20,40,70,100};
   
    void Awake()
    {
        GameManager.Instance.HealthPoint = GameManager.Instance.MaxHealthPoint;
        gameOverUI.SetActive(false);
        StartCoroutine(ShowStartText());
    }

    // Update is called once per frame
    void Update()
    {
        ComboBonusJudge();
        ShowText();
        ShowHP();
        GameOver();
        GameClear();
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
    void GameOver()
    {
        if (GameManager.Instance.IsGameOver)
        {
            gameOverUI.SetActive(true);
            GameManager.Instance.Rank = "F";
            if (UserDataManager.musicListDatas.musicList[GameManager.Instance.MusicNumber].rank == "F" ||
                 UserDataManager.musicListDatas.musicList[GameManager.Instance.MusicNumber].rank == "")
            {
                UserDataManager.musicListDatas.musicList[GameManager.Instance.MusicNumber].rank = GameManager.Instance.Rank;
            }
            SoundManager._sound.StopBGM(GameManager.Instance.MusicNumber);
        }
    }
    public void CloseGameOverUI()
    {
        GameManager.Instance.Score = 0;
        gameOverUI.SetActive(false);
        SceneManager.LoadScene("MainUI");
    }
    void GameClear()
    {
        if (GameManager.Instance.IsGameClear && !GameManager.Instance.IsGameOver)
        {
            RankJudge();//랭크 판정
            //최대치 갱신
            if (GameManager.Instance.Score > UserDataManager.musicListDatas.musicList[GameManager.Instance.MusicNumber].score)
            {
                UserDataManager.musicListDatas.musicList[GameManager.Instance.MusicNumber].score = GameManager.Instance.Score;
            }
            //랭크 갱신
            if (UserDataManager.musicListDatas.musicList[GameManager.Instance.MusicNumber].rank == "")
            {
                UserDataManager.musicListDatas.musicList[GameManager.Instance.MusicNumber].rank = GameManager.Instance.Rank;
            }
            else
            {
                if(GameManager.Instance.Rank == "S" ||
                UserDataManager.musicListDatas.musicList[GameManager.Instance.MusicNumber].rank[0] - 'A' > GameManager.Instance.Rank[0] - 'A')
                {
                    UserDataManager.musicListDatas.musicList[GameManager.Instance.MusicNumber].rank = GameManager.Instance.Rank;
                }
            } 
            UserDataManager.userData.SaveData(UserDataManager.musicListDatas.musicList);
            Invoke("GameClearUISetting", 5f);
        }
    }
    void GameClearUISetting()
    {
        gameClearUI.SetActive(true);
        _maxComboText.text = $"Max Combo : {GameManager.Instance.MaxCombo}";
        _perfectCountText.text = $"Perfect : {GameManager.Instance.PerfectCount}";
        _greatCountText.text = $"Great : {GameManager.Instance.GreatCount}";
        _goodCountText.text = $"Good : {GameManager.Instance.GoodCount}";
        _missCountText.text = $"Miss : {GameManager.Instance.MissCount}";
        _scoreResultText.text = $"Score : {GameManager.Instance.Score}";
        _rankResultText.text = $"Rank : <size=150>{GameManager.Instance.Rank}</size>";
        _rankResultText.color = RankColor(GameManager.Instance.Rank);
        SoundManager._sound.StopBGM(GameManager.Instance.MusicNumber);
    }
    public void CloseGameClearUI()
    {
        GameManager.Instance.Score = 0;
        gameClearUI.SetActive(false);
        SceneManager.LoadScene("MainUI");
    }
    Color RankColor(string rank)
    {
        switch (rank)
        {
            case "S":
                return new Color(0.2f,0.9f,0.9f);
            case "A":
                return Color.red;
            case "B":
                return new Color(1f,0.5f,0);
            case "C":
                return Color.green;
            case "D":
                return Color.blue;
            case "F":
                return Color.gray;
        }
        return Color.white;
    }
    void RankJudge() {
        GameManager.Instance.Rank = "";
        int totalNote = GameManager.Instance.PerfectCount + GameManager.Instance.GreatCount + GameManager.Instance.GoodCount + GameManager.Instance.MissCount;

        if (GameManager.Instance.PerfectCount * 100 / totalNote >= 99 && GameManager.Instance.MissCount == 0) GameManager.Instance.Rank = "S";
        else if(GameManager.Instance.PerfectCount * 100 / totalNote >= 90 && GameManager.Instance.MissCount <= 3) GameManager.Instance.Rank = "A";
        else if((GameManager.Instance.PerfectCount * 100 / totalNote >= 40 
            && GameManager.Instance.GreatCount * 100 / totalNote >= 30 && GameManager.Instance.MissCount <= 15)) GameManager.Instance.Rank = "B";
        else if((GameManager.Instance.PerfectCount + GameManager.Instance.GreatCount) * 100 / totalNote >= 40) GameManager.Instance.Rank = "C";
        else GameManager.Instance.Rank = "D";

        if(GameManager.Instance.MissCount == 0 &&(GameManager.Instance.Rank == "C" || GameManager.Instance.Rank == "D") ) GameManager.Instance.Rank = "B";//올콤
    }
}
