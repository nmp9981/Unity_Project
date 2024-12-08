using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    int _inputAnswer;//입력값
    string _inputAnswerString;//입력 문자열 값
    int _realAnswer;//찐 정답

    float _recordTime;//기록
    int _currentProblemNum = 0;//현재 문제 번호
    int _currentSolveCount = 0;//현재 맞춘 개수
    int _targetSolveCount;//총 맞춰야하는 개수
    int _score;//점수

    CalMode _calMode;//계산 모드

    bool _isGamePlay;//게임 진행중인가?

    public int InputAnswer{ get { return _inputAnswer; } set { _inputAnswer = value; } }
    public string InputAnswerString { get { return _inputAnswerString; } set { _inputAnswerString = value; } }
    public int RealAnswer { get { return _realAnswer; } set { _realAnswer = value; } }

    public float RecordTime { get { return _recordTime; }set { _recordTime = value; } }
    public int CurrentProblemNum { get { return _currentProblemNum; } set { _currentProblemNum = value; } }
    public int CurrentSolveCount { get { return _currentSolveCount; } set { _currentSolveCount = value; } }
    public int TargetSolveCount { get { return _targetSolveCount; } set { _targetSolveCount = value; } }
    public int Score { get { return _score; } set { _score = value; } }

    public CalMode CurrentCalMode { get { return _calMode; } set { _calMode = value; } }

    public bool IsGamePlay { get { return _isGamePlay; } set { _isGamePlay = value; } }
    #endregion
}
