using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static GameManager _instance;
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
        }
    }
    
    void Awake()
    {
        Init();
    }
    
    #region 데이터
    float _playerMoveSpeed = 4.0f;//기본 이속
    float _playerJumpSpeed = 4.0f;//기본 점프력
    int _maxJumpCount = 2;//최대 점프 횟수
    
    bool _playerHit = true;//플레이어 피격 여부
    long _playerAttack = 2;//플레이어 공격력
    float _playerDragSpeed = 12.0f;//표창 날아가는 속도
    float _playerAttackSpeed = 0.5f;//플레이어 공격 속도

    string _playerJob;
    int _playerLV;
    int _playerHP;
    int _playerMP;
    int _playerExp;

    int _maxMonsterCount;//최대 스폰 몬스터 수

    float _bgmVolume = 1.0f;
    float _sfxVolume = 1.0f;

    public float PlayerMoveSpeed { get { return _playerMoveSpeed; } set { _playerMoveSpeed = value; } }
    public float PlayerJumpSpeed { get { return _playerJumpSpeed; } set { _playerJumpSpeed = value; } }
    public int MaxJumpCount{ get { return _maxJumpCount; } set { _maxJumpCount = value; } }
    
    public bool PlayerHit { get { return _playerHit; } set { _playerHit = value; } }
    public long PlayerAttack { get { return _playerAttack; } set { _playerAttack = value; } }
    public float PlayerDragSpeed { get { return _playerDragSpeed; } set { _playerDragSpeed = value; } }
    public float PlayerAttackSpeed { get { return _playerAttackSpeed; } set { _playerAttackSpeed = value; } }

    public string PlayerJob { get { return _playerJob; } set { _playerJob = value; } }
    public int PlayerLV { get { return _playerLV; } set { _playerLV = value; } }
    public int PlayerHP { get { return _playerHP; } set { _playerHP = value; } }
    public int PlayerMP { get { return _playerMP; } set { _playerMP = value; } }
    public int PlayerEXP { get { return _playerExp; } set { _playerExp = value; } }

    public int MaxMonsterCount { get { return _maxMonsterCount; } set { _maxMonsterCount = value; } }

    public float BGMVolume { get { return _bgmVolume; } set { _bgmVolume = value; } }
    public float SFXVolume { get { return _sfxVolume; } set { _sfxVolume = value; } }
    #endregion
}
