using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;//전체게임 총괄
    SoundManager _soundManager = new SoundManager();//사운드
    UIManager _uiManager = new UIManager();//UI
    public static GameManager Instance
    {
        get
        {
            init();//초기화
            return _instance;
        }
    }
    //귀는 1개
    public static SoundManager Sound
    {
        get
        {
            return Instance._soundManager;
        }
    }
    //UI도 1개
    public static UIManager UI
    {
        get { return Instance._uiManager; }
    }
    static void init()
    {
        if (_instance == null)
        {
            GameObject gm = GameObject.Find("GameManager");
            if (gm == null)//없으면 새로 만든다.
            {
                gm = new GameObject { name = "GameManager" };
                gm.AddComponent<GameManager>();
            }
            DontDestroyOnLoad(gm);//씬이 바뀌어도 파괴X

            _instance = gm.GetComponent<GameManager>();

            _instance._soundManager.init();//사운드 매니저 초기화
            _instance._uiManager.init();//UIManager 초기화

            //재화
            _instance.money = 10;
            _instance.StartCoroutine(_instance.GetMoney());
        }
    }
    // Start is called before the first frame update
    void Awake()
    {
        init();
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
    int startHP = 10;
    int money = 8;
    public static readonly int[] GETMONEY = new int[5] {5,10,20,50,100};
    public static readonly string NAME = "대사희";//수정불가

    public int StartHP { get { return Instance.startHP; }set { Instance.startHP = value; } }//선언변수는 대문자
    public int Money { get { return Instance.money; }set { Instance.money = value;Instance._uiManager.PointUpdate(); } }
    #endregion
}
