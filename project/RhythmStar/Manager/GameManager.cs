using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    }
  
    #region 데이터
    int _keyCount = 3;//키 개수
    int _comboCount;//콤보 카운트
    int _comboBonus;//콤보 보너스
    int _totalNoteCount;//총 노트 개수
    int _score = 0;//점수
    float _healthPoint;//HP
    float _maxHealthPoint = 100.0f;//최대 HP

    int _maxCombo;//최대 콤보 카운트
    int _missCount;//Miss 개수
    int _goodCount;//good 개수
    int _greatCount;//Great 개수
    int _perfectCount;//퍼펙트 개수

    float _originNoteScale = 0.2f;//원래 노트 크기
    float _longNoteStandardScale = 0.25f;//롱노트 기준 크기
    float _noteSpeed = 4.0f;//노트 속도
    float _bgmVolume = 0.1f;
    float _sfxVolume = 1.0f;

    bool _isLongNoteMake1;
    bool _isLongNoteMake2;
    bool _isLongNoteMake3;

    bool _isPlayGame;//게임 진행 여부
    bool _isGameOver;//게임 오버 여부
    bool _isGameClear;//게임 클리어 여부

    int _musicNum;//재생할 음악 번호
    int _musicBPM;//재생할 음악 박자
    int _musicLevel;//재생할 음악 난이도
    string _musicName;//재생할 음악 제목

    bool _isLongNote1;//롱노트인가?
    bool _isLongNote2;
    bool _isLongNote3;

    public int[] MusicBPMList = new int[3] { 126, 148, 180 };
    public int KeyCount { get { return _keyCount; } set { _keyCount = value; } }
    public int ComboCount { get { return _comboCount; } set { _comboCount = value; } }
    public int ComboBonus { get { return _comboBonus; } set { _comboBonus = value; } }
    public int TotalNoteCount { get { return _totalNoteCount; } set { _totalNoteCount = value; } }
    public int Score { get { return _score; } set { _score = value; } }
    public float HealthPoint { get { return _healthPoint; } set { _healthPoint = value; } }
    public float MaxHealthPoint { get { return _maxHealthPoint; } set { _maxHealthPoint = value; } }

    public int MaxCombo { get { return _maxCombo; } set { _maxCombo = value; } }
    public int MissCount { get { return _missCount; } set { _missCount = value; } }
    public int GoodCount { get { return _goodCount; } set { _goodCount = value; } }
    public int GreatCount { get { return _greatCount; } set { _greatCount = value; } }
    public int PerfectCount { get { return _perfectCount; } set { _perfectCount = value; } }

    public float OriginNoteScale { get { return _originNoteScale; }}
    public float LongNoteStandardScale { get { return _longNoteStandardScale; } }
    public float NoteSpeed { get { return _noteSpeed; } set { _noteSpeed = value; } }
    public float BGMVolume { get { return _bgmVolume; } set { _bgmVolume = value; } }
    public float SFXVolume { get { return _sfxVolume; } set { _sfxVolume = value; } }

    public bool IsLongNoteMake1 { get { return _isLongNoteMake1; } set { _isLongNoteMake1 = value; } }
    public bool IsLongNoteMake2 { get { return _isLongNoteMake2; } set { _isLongNoteMake2 = value; } }
    public bool IsLongNoteMake3 { get { return _isLongNoteMake3; } set { _isLongNoteMake3 = value; } }

    public bool IsPlayGame { get { return _isPlayGame; } set { _isPlayGame = value; } }
    public bool IsGameOver { get { return _isGameOver; } set { _isGameOver = value; } }
    public bool IsGameClear { get { return _isGameClear; } set { _isGameClear = value; } }
    public int MusicNumber { get { return _musicNum; }set { _musicNum = value; } }
    public int MusicBPM { get { return _musicBPM; } set { _musicBPM = value; } }
    public int MusicLevel { get { return _musicLevel; } set { _musicLevel = value; } }
    public string MusicName { get { return _musicName; } set { _musicName = value; } }

    public bool IsLongNote1 { get { return _isLongNote1; } set { _isLongNote1 = value; } }
    public bool IsLongNote2 { get { return _isLongNote2; } set { _isLongNote2 = value; } }
    public bool IsLongNote3 { get { return _isLongNote3; } set { _isLongNote3 = value; } }
    #endregion
}
