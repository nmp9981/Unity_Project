using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    PlayerManager playerManager;
    [SerializeField] GameObject _gameOver;//게임 오버
    public TextMeshProUGUI expPoint;

    private void Awake()
    {
        playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
    }
    // Update is called once per frame
    void Update()
    {
        expPoint.text = "<size=100%>Score : </size=90%>" + GamaManager.Instance.Score.ToString();
    }
    public void Init()
    {
        GamaManager.Instance.Score = 0;
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
        }
        GamaManager.Instance.Stage = 1+GamaManager.Instance.Score / 25;//스테이지 지정
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
        GamaManager.Instance.HP = GamaManager.Instance.FullHP;//다시 풀피
    }
}
