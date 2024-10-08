using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public struct Item
{
    public Sprite itemSpriteImage;
    public int itemIdx;
    public int theNumber;
}
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
    float _playerMoveSpeed = 5.0f;//기본 이속
    float _playerJumpSpeed = 7.0f;//기본 점프력
    int _maxJumpCount = 2;//최대 점프 횟수
    
    bool _playerHit = true;//플레이어 피격 여부
    long _playerAttack;//플레이어 공격력
    float _playerDragSpeed = 12.0f;//표창 날아가는 속도
    float _playerAttackSpeed = 0.5f;//플레이어 공격 속도
    long _shadowAttack = 0;//그림자 공격력
    int _criticalRate = 0;//크리티컬 확률
    int _criticalDamage = 100;//크리티컬 데미지
    int _workmanship = 50;//숙현도

    int _playerCurrentMap;//캐릭터가 현재 있는 맵
    int _playBGMNumber;//BGM번호

    #region 캐릭터 스탯 관련
    string _playerJob;
    int _playerLV;
    int _playerHP;
    int _playerMaxHP;
    int _playerMP;
    int _playerMaxMP;
    long _playerExp;
    long _playerReqExp;
    int _playerDex;
    int _playerLuk;
    int _playerAcc;
    int _playerAvoid;
    int _playerMeso = 10000;//초기 재화량
    #endregion
    #region 아이템 관련
    int _hpPosionCount;
    int _mpPosionCount;
    int _hpPosionHealAmount;
    int _mpPosionHealAmount;
    int _keyHPIdx;
    int _keyMPIdx;
    int _attackUPCount;
    int _avoidUPCount;
    int _accUPCount;
    int _returnVillegeCount;
    public Item[] storeItemList = new Item[11];
    ItemKind _itemKind;

    string _selectItemName;
    #endregion
    int _maxMonsterCount;//최대 스폰 몬스터 수

    int _apPoint;
    int _skillPoint;//스킬 포인트

    bool _isAttackBuffOn;
    bool _isAccBuffOn;
    bool _isAvoidBuffOn;

    #region 스킬 관련
    float _throwDist = 8;
    long _luckySevenCoefficient = 58;
    int _proficiency = 10;
    int _playerAddAcc = 0;
    float _addMoveSpeed = 0;
    float _addJumpSpeed = 0;
    float _hasteTime = 0;//지속시간
    int _addMeso = 100;
    float _mesoUpTime = 0;//지속시간
    long _avengerCoefficient = 0;
    long _tripleThrowCoefficient = 0;
    float _boosterTime = 0;//지속시간
    float _shadowTime = 0;//지속시간
    #endregion
    float _bgmVolume = 1.0f;
    float _sfxVolume = 1.0f;

    bool _isCharacterDie = false;
    bool _isInvincibility = false;

    public float PlayerMoveSpeed { get { return _playerMoveSpeed; } set { _playerMoveSpeed = value; } }
    public float PlayerJumpSpeed { get { return _playerJumpSpeed; } set { _playerJumpSpeed = value; } }
    public int MaxJumpCount{ get { return _maxJumpCount; } set { _maxJumpCount = value; } }
    
    public bool PlayerHit { get { return _playerHit; } set { _playerHit = value; } }
    public long PlayerAttack { get { return _playerAttack; } set { _playerAttack = value; } }
    public float PlayerDragSpeed { get { return _playerDragSpeed; } set { _playerDragSpeed = value; } }
    public float PlayerAttackSpeed { get { return _playerAttackSpeed; } set { _playerAttackSpeed = value; } }
    public long ShadowAttack { get { return _shadowAttack; } set { _shadowAttack = value; } }
    public int CriticalRate { get { return _criticalRate; } set { _criticalRate = value; } }
    public int CriticalDamage { get { return _criticalDamage; } set { _criticalDamage = value; } }
    public int Workmanship { get { return _workmanship; } set { _workmanship = value; } }

    public int PlayerCurrentMap { get { return _playerCurrentMap; } set { _playerCurrentMap = value; } }
    public int PlayerBGMNumber { get { return _playBGMNumber; } set { _playBGMNumber = value; } }

    #region 캐릭터 스탯 관련
    public string PlayerJob { get { return _playerJob; } set { _playerJob = value; } }
    public int PlayerLV { get { return _playerLV; } set { _playerLV = value; } }
    public int PlayerHP { get { return _playerHP; } set { _playerHP = value; } }
    public int PlayerMaxHP { get { return _playerMaxHP; }set { _playerMaxHP = value; } }
    public int PlayerMP { get { return _playerMP; } set { _playerMP = value; } }
    public int PlayerMaxMP { get { return _playerMaxMP; } set { _playerMaxMP = value; } }
    public long PlayerEXP { get { return _playerExp; } set { _playerExp = value; } }
    public long PlayerReqExp { get { return _playerReqExp; } set { _playerReqExp = value; } }
    public int PlayerDEX { get { return _playerDex; } set { _playerDex = value; } }
    public int PlayerLUK { get { return _playerLuk; } set { _playerLuk = value; } }
    public int PlayerACC { get { return _playerAcc; } set { _playerAcc = value; } }
    public int PlayerAvoid { get { return _playerAvoid; } set { _playerAvoid = value; } }
    public int PlayerMeso { get { return _playerMeso; } set { _playerMeso = value; } }
    #endregion
    #region 아이템 관련
    public int HPPosionCount { get { return _hpPosionCount; } set { _hpPosionCount = value; } }
    public int MPPosionCount { get { return _mpPosionCount; } set { _mpPosionCount = value; } }
    public int HPPosionHealAmount { get { return _hpPosionHealAmount; } set { _hpPosionHealAmount = value; } }
    public int MPPosionHealAmount { get { return _mpPosionHealAmount; } set { _mpPosionHealAmount = value; } }
    public int KeyHPIdx { get { return _keyHPIdx; } set { _keyHPIdx = value; } }
    public int KeyMPIdx { get { return _keyMPIdx; } set { _keyMPIdx = value; } }
    public int AttackUPCount { get { return _attackUPCount; } set { _attackUPCount = value; } }
    public int AccUPCount { get { return _accUPCount; } set { _accUPCount = value; } }
    public int AvoidUPCount { get { return _avoidUPCount; } set { _avoidUPCount = value; } }
    public int ReturnVillegeCount { get { return _returnVillegeCount; } set { _returnVillegeCount = value; } }
    public string SelectedItemName { get { return _selectItemName; } set { _selectItemName = value; } }
    public ItemKind ItemKinds { get { return _itemKind; } set { _itemKind = value; } }
#endregion
    public int MaxMonsterCount { get { return _maxMonsterCount; } set { _maxMonsterCount = value; } }

    public int ApPoint { get { return _apPoint; } set { _apPoint = value; } }
    public int SkillPoint { get { return _skillPoint; } set { _skillPoint = value; } }

    public bool IsAtaackBuffOn { get { return _isAttackBuffOn; } set { _isAttackBuffOn = value; } }
    public bool IsAccBuffOn { get { return _isAccBuffOn; } set { _isAccBuffOn = value; } }
    public bool IsAvoidBuffOn { get { return _isAvoidBuffOn; } set { _isAvoidBuffOn = value; } }

    #region 스킬 관련
    public float ThrowDist { get { return _throwDist; } set { _throwDist = value; } }
    public long LuckySevenCoefficient { get { return _luckySevenCoefficient; } set { _luckySevenCoefficient = value; } }
    public int Proficiency { get { return _proficiency; } set { _proficiency = value; } }
    public int PlayerAddACC { get { return _playerAddAcc; } set { _playerAddAcc = value; } }
    public float AddMoveSpeed { get { return _addMoveSpeed; } set { _addMoveSpeed = value; } }
    public float AddJumpSpeed { get { return _addJumpSpeed; } set { _addJumpSpeed = value; } }
    public float HasteTime { get { return _hasteTime; } set { _hasteTime = value; } }
    public int AddMeso { get { return _addMeso; } set { _addMeso = value; } }
    public float MesoUpTime { get { return _mesoUpTime; } set { _mesoUpTime = value; } }
    public long AvengerCoefficient { get { return _avengerCoefficient; } set { _avengerCoefficient = value; } }
    public long TripleThrowCoefficient { get { return _tripleThrowCoefficient; } set { _tripleThrowCoefficient = value; } }
    public float BoosterTime { get { return _boosterTime; } set { _boosterTime = value; } }
    public float ShadowTime { get { return _shadowTime; } set { _shadowTime = value; } }
    #endregion
    public float BGMVolume { get { return _bgmVolume; } set { _bgmVolume = value; } }
    public float SFXVolume { get { return _sfxVolume; } set { _sfxVolume = value; } }

    public bool IsCharacterDie { get { return _isCharacterDie; }set { _isCharacterDie = value; } }
    public bool IsInvincibility { get { return _isInvincibility; } set { _isInvincibility = value; } }
    #endregion
}
