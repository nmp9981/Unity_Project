using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    static GameManager _instance;

    public static GameManager Instance { get { Init(); return _instance; } }

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
    }

    void Awake()
    {
        Init();
    }
    #region 데이터
    int _currentStage = 1;//현재 스테이지
    uint _currentExp = 0;//현재 경험치
    ulong _currentMeso = 0;//현재 메소
    int _currentCastleHP = 900;//현재 성의 체력
    int _fullCastleHP = 900;//성의 최대 HP

    public uint[] RequireStageUPExp = new uint[10];//스테이지 업을 위한 누적 경험치

    public int CurrentStage { get { return _currentStage; } set { _currentStage = value; } }
    public uint CurrentExp { get { return _currentExp; } set { _currentExp = value; } }
    public ulong CurrentMeso { get { return _currentMeso; } set { _currentMeso = value; } }
    public int CurrentCastleHP { get { return _currentCastleHP; } set { _currentCastleHP = value; } }
    public int FullCastleHP { get { return _fullCastleHP; } set { _fullCastleHP = value; } }
    #endregion 데이터
}
