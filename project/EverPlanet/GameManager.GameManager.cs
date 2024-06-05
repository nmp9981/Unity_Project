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
    bool _playerHit = true;//플레이어 피격 여부
    long _playerAttack = 2;//플레이어 공격력
    
    string _playerJob;
    int _playerLV;
    int _playerHP;
    int _playerMP;
    int _playerExp;

    float _bgmVolume = 1.0f;
    float _sfxVolume = 1.0f;

    public float PlayerMoveSpeed { get { return _playerMoveSpeed; } set { _playerMoveSpeed = value; } }
    public bool PlayerHit { get { return _playerHit; } set { _playerHit = value; } }
    public long PlayerAttack { get { return _playerAttack; } set { _playerAttack = value; } }

    public string PlayerJob { get { return _playerJob; } set { _playerJob = value; } }
    public int PlayerLV { get { return _playerLV; } set { _playerLV = value; } }
    public int PlayerHP { get { return _playerHP; } set { _playerHP = value; } }
    public int PlayerMP { get { return _playerMP; } set { _playerMP = value; } }
    public int PlayerEXP { get { return _playerExp; } set { _playerExp = value; } }

    public float BGMVolume { get { return _bgmVolume; } set { _bgmVolume = value; } }
    public float SFXVolume { get { return _sfxVolume; } set { _sfxVolume = value; } }
    #endregion
}
