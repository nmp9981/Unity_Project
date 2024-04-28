using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager _instance;

    public static GameManager Instance
    {
        get { Init(); return _instance; }
    }
    static void Init()
    {
        if (_instance == null)//싱글톤 구현
        {
            GameObject gm = GameObject.Find("GameManager");
            if (gm == null)
            {
                gm = new GameObject { name = "GameManager" };

                gm.AddComponent<GameManager>();
            }
            DontDestroyOnLoad(gm);
            _instance = gm.GetComponent<GameManager>();
        }
    }
    void Awake()
    {
        Init();
    }
    private void Update()
    {
        GameOver();
    }
    public void GameOver()
    {
        //게임 오버
        if (HealthPoint <= 0)
        {
            IsGameOver = true;
        }
        //클리어
        if(TotalNoteCount>=25000 && !IsGameOver)
        {
            Debug.Log("게임 클리어");
            IsGameOver = true;
        }
    }
    #region 데이터
    int _keyCount = 3;//키 개수
    int _comboCount;//콤보 카운트
    int _comboBonus;//콤보 보너스
    int _totalNoteCount;//총 노트 개수
    int _score = 0;//점수
    float _healthPoint;//HP
    float _maxHealthPoint = 100.0f;//최대 HP

    float _noteSpeed = 4.0f;//노트 속도
    float _bgmVolume = 0.1f;
    float _sfxVolume = 1.0f;

    bool _isPlayGame;//게임 진행 여부
    bool _isGameOver;//게임 오버 여부

    public int KeyCount { get { return _keyCount; } set { _keyCount = value; } }
    public int ComboCount { get { return _comboCount; } set { _comboCount = value; } }
    public int ComboBonus { get { return _comboBonus; } set { _comboBonus = value; } }
    public int TotalNoteCount { get { return _totalNoteCount; } set { _totalNoteCount = value; } }
    public int Score { get { return _score; } set { _score = value; } }
    public float HealthPoint { get { return _healthPoint; } set { _healthPoint = value; } }
    public float MaxHealthPoint { get { return _maxHealthPoint; } set { _maxHealthPoint = value; } }
    public float NoteSpeed { get { return _noteSpeed; } set { _noteSpeed = value; } }
    public float BGMVolume { get { return _bgmVolume; } set { _bgmVolume = value; } }
    public float SFXVolume { get { return _sfxVolume; } set { _sfxVolume = value; } }
    public bool IsPlayGame { get { return _isPlayGame; } set { _isPlayGame = value; } }
    public bool IsGameOver { get { return _isGameOver; } set { _isGameOver = value; } }
    #endregion
}
