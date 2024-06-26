using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static GameManager _instance;
    FishesSpawn _fishSpawn;
    private float coolTime = 7.5f;

    public static GameManager Instance { get { Init(); return _instance; } }
    static void Init()
    {
        if (_instance == null)
        {
            GameObject gm = GameObject.Find("GameManager");
            if (gm == null)
            {
                gm = new GameObject { name = "GameManager" };

                gm.AddComponent<GameManager>();
            }
            DontDestroyOnLoad(gm);
            _instance = gm.GetComponent<GameManager>();

            PlayerPrefs.SetString("BestAttack", 2.ToString());
            PlayerPrefs.SetInt("BestStage", 1);
            PlayerPrefs.SetFloat("BestTime", 0);
            PlayerPrefs.SetFloat("BestClear", 1);
        }

    }
    void InitComponent()
    {
        if(SceneManager.GetActiveScene().name == "FishDomInGame")
        {
            _fishSpawn = GameObject.Find("FishSpawner").GetComponent<FishesSpawn>();
            _fishSpawn.Init();
        }
    }
    void Awake()
    {
        Init();
        InitPlayerData(PlayMode);
        InitComponent();
    }
    public void InitPlayerData(int playmode)
    {
        Time.timeScale = 1.0f;
        if (playmode == 0)
        {
            PlayerAttack = 2;
            PlayerScale = 1.0f;
            StageNum = 1;
            PlayInitTime = -7.0f;
        }
        else
        {
            StageNum = playmode;
            PlayerAttack = 10 * (playmode - 1) + 2;
            PlayerScale = 1.0f + PlayerAttack * 0.0015f;
        }
    }
    public void StageUp()
    {
        if (PlayMode == 0)//챌린지 모드
        {
            PlayInitTime += Time.deltaTime;
            PlayerMaxTime += Time.deltaTime;
            if (PlayInitTime > coolTime)
            {
                StageNum += 1;
                StageNum = (StageNum >= _fishKinds) ? _fishKinds : StageNum;

                PlayInitTime = 0.0f;
            }

            if (StageNum > PlayerMaxStage)//최고 스테이지 저장
            {
                PlayerMaxStage = StageNum;
                PlayerPrefs.SetInt("BestStage", PlayerMaxStage);
            }
        }
    }
    #region 데이터
    float _currentTime = -7.0f;
    static int _playMode;
    float _playerMoveSpeed = 4.0f;
    float _playerDir = -1.0f;
    bool _playerHit = true;
    float _playerScale = 1.0f;
    long _playerAttack = 2;

    int _playerMaxStage = 1;
    long _playerMaxAttack = 2;
    float _playerMaxTime = 0;
    int _playerMaxClearStage = 1;

    int _stageNumber = 1;
    int _restCount = 30;
    float _enemyMoveSpeed = 2.0f;
    float _enemyToCameraStdDistance = 100.0f;

    const int _fishKinds = 30;

    float _bgmVolume = 1.0f;
    float _sfxVolume = 1.0f;

    public float PlayInitTime { get { return _currentTime; }set { _currentTime = value; } }
    public int PlayMode { get { return _playMode; } set { _playMode = value; } }
    public float PlayerMoveSpeed { get { return _playerMoveSpeed; } set { _playerMoveSpeed = value; } }
    public float PlayerDir { get { return _playerDir; } set { _playerDir = _playerDir == 0 ? 1.0f : value; } }
    public bool PlayerHit { get { return _playerHit; } set { _playerHit = value; } }
    public float PlayerScale { get { return _playerScale; } set { _playerScale = value; } }
    public long PlayerAttack { get { return _playerAttack; } set { _playerAttack = value; } }

    public int PlayerMaxStage { get { return _playerMaxStage; } set { _playerMaxStage = value; } }
    public long PlayerMaxAttack { get { return _playerMaxAttack; } set { _playerMaxAttack = value; } }
    public float PlayerMaxTime { get { return _playerMaxTime; } set { _playerMaxTime = value; } }
    public int PlayerMaxClearStage { get { return _playerMaxClearStage; } set { _playerMaxClearStage = value; } }

    public int StageNum { get { return _stageNumber; } set { _stageNumber = value; } }
    public int RestCount { get { return _restCount; } set { _restCount = value; } }
    public float EnemyMoveSpeed { get { return _enemyMoveSpeed; } set { _enemyMoveSpeed = value; } }
    public float EnemyToCameraStdDistance { get { return _enemyToCameraStdDistance; } set { _enemyToCameraStdDistance = value; } }

    public int FishKinds {get { return _fishKinds;}}

    public float BGMVolume { get { return _bgmVolume; } set { _bgmVolume = value; } }
    public float SFXVolume { get { return _sfxVolume; } set { _sfxVolume = value; } }
    #endregion
}
