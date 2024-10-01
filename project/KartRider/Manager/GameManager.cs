using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    float _carSpeed;//차량의 현재 속도
    float _speedLimit = 120f;//속도 제한
    float _racingDist = 0;//주행 거리
    float _touque = 900;//차량 가속도
    
    bool _isBreaking = false;//브레이크 여부
    float _breakPower = 20000;//브레이크 파워

    bool _isBooster = false;//부스터 여부

    public float CarSpeed { get { return _carSpeed; } set { _carSpeed = value; } }
    public float SpeedLimit { get { return _speedLimit; } set { _speedLimit = value; } }
    public float RacingDist { get { return _racingDist; } set { _racingDist = value; } }
    public float Touque { get { return _touque; } set { _touque = value; } }

    public bool IsBooster { get { return _isBooster; } set { _isBooster = value; } }

    public bool IsBreaking { get { return _isBreaking; } set { _isBreaking = value; } }
    public float BreakPower { get { return _breakPower; } set { _breakPower = value; } }
    #endregion
}
