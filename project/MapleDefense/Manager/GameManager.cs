using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    static GameManager _instance;

    public static GameManager Instance { get { Init(); return _instance; } }

    static CastleManager castleManager;
    static public Dictionary<string, int> mapDictoinaty;
    static public List<Vector3> startPosList = new List<Vector3>();
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
        castleManager = GameObject.Find("Castle").GetComponent<CastleManager>();
    }

    void Awake()
    {
        Init();
        SettingRequireEXP();
    }
    /// <summary>
    /// 요구 경험치 세팅
    /// </summary>
    void SettingRequireEXP()
    {
        RequireStageUPExp = new uint[33];
        RequireStageUPExp[0] = 0;

        for(int i = 1; i < 32; i++)
        {
            RequireStageUPExp[i] = (uint)(i * i * 10);
        }
    }

    /// <summary>
    /// 레벨업
    /// </summary>
    public void CastleLevelUP()
    {
        if(CurrentExp >= RequireStageUPExp[CurrentStage])
        {
            CurrentExp -= RequireStageUPExp[CurrentStage];
            FullCastleHP += 50;
            CurrentCastleHP = FullCastleHP;
            CurrentStage += 1;
            castleManager.ShowCastleHP();
        }
    }
    #region 데이터
    int _currentStage = 1;//현재 스테이지
    uint _currentExp = 0;//현재 경험치
    ulong _currentMeso = 0;//현재 메소
    int _currentCastleHP = 900;//현재 성의 체력
    int _fullCastleHP = 900;//성의 최대 HP

    int _curTurretCount = 1;//현재 터렛 개수
    int _maxTurretCount = 1;//최대 터렛 개수

    int _spawnTime = 1500;//스폰 시간
    public uint[] RequireStageUPExp;//스테이지 업을 위한 누적 경험치

    public List<GameObject> ActiveUnitList = new List<GameObject>();//필드에 활성화된 유닛

    public int CurrentStage { get { return _currentStage; } set { _currentStage = value; } }
    public uint CurrentExp { get { return _currentExp; } set { _currentExp = value; } }
    public ulong CurrentMeso { get { return _currentMeso; } set { _currentMeso = value; } }
    public int CurrentCastleHP { get { return _currentCastleHP; } set { _currentCastleHP = value; } }
    public int FullCastleHP { get { return _fullCastleHP; } set { _fullCastleHP = value; } }

    public int CurrentTurretCount { get { return _curTurretCount; } set { _curTurretCount = value; } }
    public int MaxTurretCount { get { return _maxTurretCount; } set { _maxTurretCount = value; } }
    
    public int SpawnTime { get { return _spawnTime; } set { _spawnTime = value; } }
    #endregion 데이터
}
