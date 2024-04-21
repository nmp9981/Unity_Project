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

    #region 데이터
    int _keyCount = 3;//키 개수
    int _comboCount;//콤보 카운트
    int _score = 0;//점수
    float _bgmVolume;
    float _sfxVolume;

    public int KeyCount { get { return _keyCount; } set { _keyCount = value; } }
    public int ComboCount { get { return _comboCount; } set { _comboCount = value; } }
    public int Score { get { return _score; } set { _score = value; } }
    public float BGMVolume { get { return _bgmVolume; } set { _bgmVolume = value; } }
    public float SFXVolume { get { return _sfxVolume; } set { _sfxVolume = value; } }
    #endregion
}
