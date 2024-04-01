using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager _instance;
    
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

    // Update is called once per frame
    void Update()
    {
        
    }
    #region 데이터
    int _rowCount = 10;//행
    int _colCount = 20;//열
    int _restBlockCount;//남은 블록 개수
    int _score = 0;//점수

    public int RowCount { get { return _rowCount; } set { _rowCount = value; } }
    public int ColCount { get { return _colCount; } set { _colCount = value; } }
    public int RestBlockCount { get { return _restBlockCount; } set { _restBlockCount = value; } }
    public int Score { get { return _score; } set { _score = value; } }
    #endregion
}
