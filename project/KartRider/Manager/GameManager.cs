using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static GameManager _instance;
    
    public static GameManager Instance { get { Init(); return _instance; } }

    static public Dictionary<string,int> mapDictoinaty;
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
        SettingMapList();
    }
    /// <summary>
    /// 기능 : 맵 리스트 세팅
    /// 1) 맵 이름, 번호 세팅
    /// </summary>
    void SettingMapList()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name != "MenuUI") return;

        mapDictoinaty = new Dictionary<string, int>();
        GameObject mapListObj = GameObject.Find("MapList");
        int number = 0;
        foreach(var mapName in mapListObj.GetComponentsInChildren<TextMeshProUGUI>())
        {
            mapDictoinaty.Add(mapName.text,number);
            number += 1;
        }
    }
    /// <summary>
    /// 기능 : 시작지점으로 이동
    /// </summary>
    public static void MoveStartPosition(int idx)
    {
        
    }
    #region 데이터
    float _carSpeed;//차량의 현재 속도
    float _speedLimit = 144f;//속도 제한
    float _racingDist = 0;//주행 거리
    float _touque = 1100;//차량 가속도
    
    bool _isBreaking = false;//브레이크 여부
    float _breakPower = 20000;//브레이크 파워

    bool _isBooster = false;//부스터 여부
    int _boosterCount = 0;//부스터 개수
    float _currentBoosterGage;//현재 부스터 게이지

    float _currentTime;//현재 경과 시간

    bool _isDriving = false;//주행중인가?
    int _currentLap = 0;//현재 랩 수
    int _mapLap;//최종 돌아야하는 바퀴 수

    string _currentMap = string.Empty;//현재 주행하는 맵 이름
    int _currentMapIndex;//현재 주행하는 맵 번호

    public float CarSpeed { get { return _carSpeed; } set { _carSpeed = value; } }
    public float SpeedLimit { get { return _speedLimit; } set { _speedLimit = value; } }
    public float RacingDist { get { return _racingDist; } set { _racingDist = value; } }
    public float Touque { get { return _touque; } set { _touque = value; } }

    public bool IsBooster { get { return _isBooster; } set { _isBooster = value; } }

    public bool IsBreaking { get { return _isBreaking; } set { _isBreaking = value; } }
    public float BreakPower { get { return _breakPower; } set { _breakPower = value; } }
    public int BoosterCount { get { return _boosterCount; } set { _boosterCount = value; } }
    public float CurrentBoosterGage { get { return _currentBoosterGage; } set { _currentBoosterGage = value; } }
    public float CurrentTime { get { return _currentTime; }set { _currentTime = value; } }

    public bool IsDriving { get { return _isDriving; } set { _isDriving = value; } }
    public int CurrentLap { get { return _currentLap; } set { _currentLap = value; } }
    public int MapLap { get { return _mapLap; } set { _mapLap = value; } }

    public string CurrentMap { get { return _currentMap; } set { _currentMap = value; } }
    public int CurrentMapIndex { get { return _currentMapIndex; } set { _currentMapIndex = value; } }
    #endregion
}
