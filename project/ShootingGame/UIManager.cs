using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    PlayerManager playerManager;
    BackGroundScrolling backGroundScrolling;
    [SerializeField] GameObject _gameOver;//게임 오버
    public TextMeshProUGUI expPoint;
    public TextMeshProUGUI stagePoint;
    public TextMeshProUGUI restMobPoint;

    private void Awake()
    {
        playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
        backGroundScrolling = GameObject.Find("BackgroundManager").GetComponent<BackGroundScrolling>();
    }
    // Update is called once per frame
    void Update()
    {
        expPoint.text = "<size=100%>Score : </size=90%>" + GamaManager.Instance.Score.ToString();
        stagePoint.text = "<size=100%>Stage : </size=80%>" + GamaManager.Instance.Stage.ToString();
        restMobPoint.text = GamaManager.Instance.HitMonster.ToString()+" / "+ GamaManager.Instance.GoalMonster.ToString();
    }
    public void Init()
    {
        GamaManager.Instance.Score = 0;
        GamaManager.Instance.Stage = 1;
        GamaManager.Instance.HitMonster = 0;
        GamaManager.Instance.GoalMonster = 5;
    }
    //득점
    public void getScore(string type)
    {
        switch (type)
        {
            case "EnemyA":
                GamaManager.Instance.Score += 3;
                break;
            case "EnemyB":
                GamaManager.Instance.Score += 4;
                break;
            case "EnemyC":
                GamaManager.Instance.Score += 6;
                break;
            case "EnemyD":
                GamaManager.Instance.Score += 9;
                break;
            case "EnemyE":
                GamaManager.Instance.Score += 13;
                break;
            case "EnemyF":
                GamaManager.Instance.Score += 18;
                break;
        }
        GamaManager.Instance.HitMonster += 1;//몬스터를 잡음
        Debug.Log(GamaManager.Instance.HitMonster + "스테이지"+ GamaManager.Instance.GoalMonster);
        if (GamaManager.Instance.HitMonster >= GamaManager.Instance.GoalMonster) NextStage();//스테이지 증가
    }
    //스테이지 증가
    public void NextStage()
    {
        GamaManager.Instance.Stage += 1;
        GamaManager.Instance.HitMonster = 0;
        GamaManager.Instance.GoalMonster = 5+GamaManager.Instance.Stage/2;
        
        backGroundScrolling.BackGroundChange(GamaManager.Instance.Stage);//배경 변화
    }
    //게임 오버
    public void GameOver()
    {
        playerManager.HPBarDestroy();//HP바 파괴
        Time.timeScale = 0.0f;
        _gameOver.SetActive(true);
    }
    //게임 시작
    public void ReGame()
    {
        Time.timeScale = 1.0f;
        _gameOver.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        GamaManager.Instance.Score = 0;//점수 초기화
        GamaManager.Instance.Stage = 1;//스테이지 초기화
        GamaManager.Instance.HitMonster = 0;
        GamaManager.Instance.GoalMonster = 5;
        GamaManager.Instance.FullHP = 14;
        GamaManager.Instance.startAttack = 2;//공격력 초기화
        GamaManager.Instance.PowerStack = 1;//총알 단계도 초기화
        GamaManager.Instance.HP = GamaManager.Instance.FullHP;//다시 풀피
        backGroundScrolling.BackGroundChange(1);//배경 초기화
    }
}
