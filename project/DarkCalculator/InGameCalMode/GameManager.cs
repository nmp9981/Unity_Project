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
    /// <summary>
    /// 기능 : 인게임 데이터 초기화
    /// </summary>
    public void InitInGameData()
    {
        _currentProblemNum = 0;
        _currentSolveCount = 0;
        _score = 0;
    }
    
    #region 데이터
    int _inputAnswer;//입력값
    string _inputAnswerString;//입력 문자열 값
    int _realAnswer;//찐 정답

    ulong _inputPrime;//입력 정수값
    string _inputPrimeString;//입력 소수 값
    string _outputPrimeString;//입력 소수 값

    float _recordTime;//기록
    int _currentProblemNum = 0;//현재 문제 번호
    int _currentSolveCount = 0;//현재 맞춘 개수
    int _targetSolveCount=5;//총 맞춰야하는 개수
    int _score;//점수

    CalMode _calMode;//계산 모드

    bool _isGamePlay;//게임 진행중인가?
    bool _isSettingPossible = true;//세팅 가능한 값인가?

    float _bgmValue = 0.7f;//소리 크기

    public List<int> calSymbolJudgeList = new List<int>();//사용 기호 판정 리스트
    public bool[] calSymbolList = new bool[4];//기호
    public bool[] calCountList = new bool[2];//계산할 숫자 개수
    int _digitMinCount=2;//계산할 최소 자릿수
    int _digitMaxCount=2;//계산할 최대 자릿수
    public int InputAnswer{ get { return _inputAnswer; } set { _inputAnswer = value; } }
    public string InputAnswerString { get { return _inputAnswerString; } set { _inputAnswerString = value; } }
    public int RealAnswer { get { return _realAnswer; } set { _realAnswer = value; } }

    public ulong InputPrime { get { return _inputPrime; } set { _inputPrime = value; } }
    public string InputPrimeString { get { return _inputPrimeString; } set { _inputPrimeString = value; } }
    public string OutputPrimeString { get { return _outputPrimeString; } set { _outputPrimeString = value; } }

    public float RecordTime { get { return _recordTime; }set { _recordTime = value; } }
    public int CurrentProblemNum { get { return _currentProblemNum; } set { _currentProblemNum = value; } }
    public int CurrentSolveCount { get { return _currentSolveCount; } set { _currentSolveCount = value; } }
    public int TargetSolveCount { get { return _targetSolveCount; } set { _targetSolveCount = value; } }
    public int Score { get { return _score; } set { _score = value; } }

    public CalMode CurrentCalMode { get { return _calMode; } set { _calMode = value; } }

    public bool IsGamePlay { get { return _isGamePlay; } set { _isGamePlay = value; } }
    public bool IsSettingPossible { get { return _isSettingPossible; } set { _isSettingPossible = value; } }

    public float BGMVolume { get { return _bgmValue; } set { _bgmValue = value; } }


    public int DigitMinCount { get { return _digitMinCount; } set { _digitMinCount = value; } }
    public int DigitMaxCount { get { return _digitMaxCount; } set { _digitMaxCount = value; } }
    #endregion
}
