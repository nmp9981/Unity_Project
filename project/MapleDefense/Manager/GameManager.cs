using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public enum CastleSkill
{
    Booster,
    Mastery,
    HPUP
}

public class GameManager : MonoBehaviour
{
    static GameManager _instance;

    public static GameManager Instance { get { Init(); return _instance; } }

    static CastleManager castleManager;
    static BackGroundManager backGroundManager;
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
        backGroundManager = GameObject.Find("GameBackGround").GetComponent<BackGroundManager>();
    }

    void Awake()
    {
        Init();
        SettingRequireEXP();
        PlayInitBGM();
        SkillLevelSetting();
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
    /// 초기 BGM재생
    /// </summary>
    void PlayInitBGM()
    {
        SoundManager._sound.PlayBGM(0);
    }
    /// <summary>
    /// 기능 : 스킬 레벨 초기화
    /// </summary>
    void SkillLevelSetting()
    {
        for (int i = 0; i < 3; i++)
        {
            CurrentSkillLvArray[i] = 1;
        }
        MaxSkillLvArray[(int)CastleSkill.Booster] = 7;
        MaxSkillLvArray[(int)CastleSkill.Mastery] = 4;
        MaxSkillLvArray[(int)CastleSkill.HPUP] = 9;

        IncreaseCastleHP[0] = 0;
        IncreaseCastleHP[1] = 500;
        IncreaseCastleHP[2] = 1500;
        IncreaseCastleHP[3] = 5000;
        IncreaseCastleHP[4] = 14000;
        IncreaseCastleHP[5] = 40000;
        IncreaseCastleHP[6] = 100000;
        IncreaseCastleHP[7] = 320000;
        IncreaseCastleHP[8] = 1100000;
        IncreaseCastleHP[9] = 3100000;

        MasteryPriceArray[1] = 3000;
        MasteryPriceArray[2] = 80000;
        MasteryPriceArray[3] = 1000000;
        MasteryPriceArray[4] = 30000000;

        IncreaseCastleHPPrice[1] = 1000;
        IncreaseCastleHPPrice[2] = 2000;
        IncreaseCastleHPPrice[3] = 4000;
        IncreaseCastleHPPrice[4] = 12000;
        IncreaseCastleHPPrice[5] = 36000;
        IncreaseCastleHPPrice[6] = 144000;
        IncreaseCastleHPPrice[7] = 576000;
        IncreaseCastleHPPrice[8] = 2880000;
        IncreaseCastleHPPrice[9] = 14400000;

        BoosterPriceArray[1] = 500;
        BoosterPriceArray[2] = 1500;
        BoosterPriceArray[3] = 5000;
        BoosterPriceArray[4] = 25000;
        BoosterPriceArray[5] = 125000;
        BoosterPriceArray[6] = 750000;
        BoosterPriceArray[7] = 4500000;
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
            AttackBetweenTime -= 0.01f;
            castleManager.ShowCastleHP();
            backGroundManager.ChangeBackground();
            //새 BGM 재생
            SoundManager._sound.PlayBGM(CurrentStage/3);
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

    int _curThrowIndex = 0;//현재 투사체 인덱스
    int _curSupportIndex = 0;//현재 소환수 인덱스

    int _spawnTime = 1500;//스폰 시간
    public uint[] RequireStageUPExp;//스테이지 업을 위한 누적 경험치

    float _attackBetweenTime = 0.5f;//공격 간격 시간

    bool _isDie = false;//사망 여부
    bool _isOpenUpgradeUI = false;//업그레이드 UI가 열려있는가?

    //현재 페이지
    int _currentWeaponPageNum = 0;
    int _currentThrowPageNum = 0;
    int _currentSupportPageNum = 0;
    int _currentSkillPageNum = 0;

    public List<GameObject> ActiveUnitList = new List<GameObject>();//필드에 활성화된 몬스터
    public List<CastleAttack> CurrentTurretList = new List<CastleAttack>();//필드에 활성화된 설치기

    //스킬레벨 관리
    public int[] CurrentSkillLvArray = new int[3];
    public int[] MaxSkillLvArray = new int[3];
    public int[] IncreaseCastleHP = new int[10];
    public ulong[] IncreaseCastleHPPrice = new ulong[10];
    public ulong[] MasteryPriceArray = new ulong[5];
    public ulong[] BoosterPriceArray = new ulong[8];

    public int CurrentStage { get { return _currentStage; } set { _currentStage = value; } }
    public uint CurrentExp { get { return _currentExp; } set { _currentExp = value; } }
    public ulong CurrentMeso { get { return _currentMeso; } set { _currentMeso = value; } }
    public int CurrentCastleHP { get { return _currentCastleHP; } set { _currentCastleHP = value; } }
    public int FullCastleHP { get { return _fullCastleHP; } set { _fullCastleHP = value; } }

    public int CurrentTurretIndex { get { return _curTurretCount; } set { _curTurretCount = value; } }
    public int MaxTurretCount { get { return _maxTurretCount; } set { _maxTurretCount = value; } }
    
    public int CurrentThrowIndex { get { return _curThrowIndex; } set { _curThrowIndex = value; } }
    public int CurrentSupportIndex { get { return _curSupportIndex; } set { _curSupportIndex = value; } }

    public int SpawnTime { get { return _spawnTime; } set { _spawnTime = value; } }

    public float AttackBetweenTime { get { return _attackBetweenTime; } set { _attackBetweenTime = value; } }

    public int CurrentWeaponPageNum { get { return _currentWeaponPageNum; } set { _currentWeaponPageNum = value; } }
    public int CurrentThrowPageNum { get { return _currentThrowPageNum; } set { _currentThrowPageNum = value; } }
    public int CurrentSupportPageNum { get { return _currentSupportPageNum; } set { _currentSupportPageNum = value; } }
    public int CurrentSkillPageNum { get { return _currentSkillPageNum; } set { _currentSkillPageNum = value; } }

    public bool IsDie { get { return _isDie; } set { _isDie = value; } }
    public bool IsFighting { get; set; } = false;//전투중인가?
    public bool IsOpenUpgradeUI { get; set; } = false;//업그레이드 UI활성화 여부

    //소리 관련
    public float BGMVolume { get; set; } = 0.7f;
    public float SFXVolume { get; set; } = 0.7f;
    #endregion 데이터
}
