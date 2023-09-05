using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamaManager : MonoBehaviour
{
    private static GamaManager _instance;//전체 게임 총괄

    UIManager _uiManager = new UIManager();
    DataManager _dataManager = new DataManager();
    ResourceManager _reSourceManager = new ResourceManager();
    AttackManager _attackManager = new AttackManager();
    ObjectManager _objectManager = new ObjectManager();
    ResPawnManager _reSpawnManager = new ResPawnManager();

    //싱글톤
    public static GamaManager Instance
    {
        get
        {
            Init();//초기화
            return _instance;
        }
    }

    //UI는 1개
    public static UIManager UI{get { return Instance._uiManager; }}
    public static DataManager Data { get { return Instance._dataManager; } }
    public static ResourceManager Resource { get { return Instance._reSourceManager; } }
    public static AttackManager Attack { get { return Instance._attackManager; } }
    public static ObjectManager Object { get { return Instance._objectManager; } }

    //초기화(싱글톤 적용)
    static void Init()
    {
        if (_instance == null)
        {
            GameObject gameManager = GameObject.Find("GameManager");
            if(gameManager == null)
            {
                gameManager = new GameObject { name = "GameManager" };
                gameManager.AddComponent<GamaManager>();
            }
            DontDestroyOnLoad(gameManager);//파괴 방지

            _instance = gameManager.GetComponent<GamaManager>();
            _instance._uiManager.Init();//UI초기화

            //재화
            _instance.money = 0;
            _instance.StartCoroutine(_instance.GetMoney());

        }
    }
    // Start is called before the first frame update
    void Awake()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator GetMoney()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            Money += 5;
        }
        
    }
    

    #region 공유데이터
    int money = 0;
    int attack = 2;
    int hitDamage = 1;
    int score = 0;
    int fullHP = 14;
    int hp = 14;
    int healHP = 2;
    int stage = 1;
    float playerSpeed = 3.0f;//이동속도
    public static readonly int[] GETDAMAGE = new int[3] { 1,2,4 };
    public static readonly int[] GETMOBEY = new int[5] { 1, 2, 3, 4, 5 };
    public static readonly string Name = "대사희";

    public float PlayerSpeed { get { return Instance.playerSpeed; }set { Instance.playerSpeed = value; } }
    public int startAttack { get { return Instance.attack; }set { Instance.attack = value; } }
    public int Money { get { return Instance.money; }set { Instance.money = value; } }
    public int Score { get { return Instance.score; } set { Instance.score = value; } }
    public int FullHP { get { return Instance.fullHP; }set { Instance.fullHP = value; } }
    public int HP { get { return Instance.hp; } set { Instance.hp = value; } }
    public int HitDamage { get { return Instance.hitDamage; } set { Instance.hitDamage = value; } }
    public int HealHP { get { return healHP; } }
    public int Stage { get { return Instance.stage; } set { Instance.stage = value; } }
    #endregion 
}

