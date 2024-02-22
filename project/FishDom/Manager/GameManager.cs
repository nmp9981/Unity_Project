using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    static GameManager _instance;
    FishesSpawn _fishSpawn;

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
    void InitComponent()
    {
        _fishSpawn = GameObject.Find("FishSpawner").GetComponent<FishesSpawn>();
        _fishSpawn.Init();
        StartCoroutine(_fishSpawn.FishSpawnLogic());
    }
    void Awake()
    {
        Init();
        InitComponent();
    }

    void Update()
    {
        StageUp();
    }
    void StageUp()
    {
        if (PlayerAttack > 10*StageNum)
        {
            StageNum+=1;
            StageNum = (StageNum >= 3) ? 3:StageNum;
        }
    }
    #region 데이터
    float _playerMoveSpeed = 4.0f;
    float _playerDir = -1.0f;
    float _playerScale = 1.0f;
    long _playerAttack = 2;

    int _stageNumber = 1;
    float _enemyMoveSpeed = 2.0f;
    float _enemyToCameraStdDistance = 100.0f;

    public float PlayerMoveSpeed { get { return _playerMoveSpeed; } set { _playerMoveSpeed = value; } }
    public float PlayerDir { get { return _playerDir; } set { _playerDir = _playerDir == 0 ? 1.0f: value; } }
    public float PlayerScale { get { return _playerScale; } set { _playerScale = value; } }
    public long PlayerAttack { get { return _playerAttack; } set { _playerAttack = value; } }

    public int StageNum { get { return _stageNumber; } set { _stageNumber = value; } }
    public float EnemyMoveSpeed { get { return _enemyMoveSpeed; } set { _enemyMoveSpeed = value; } }
    public float EnemyToCameraStdDistance { get { return _enemyToCameraStdDistance; } set { _enemyToCameraStdDistance = value; } }
    #endregion
}
