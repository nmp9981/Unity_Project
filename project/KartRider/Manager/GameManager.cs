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
  
    public float CarSpeed { get { return _carSpeed; } set { _carSpeed = value; } }
    public float SpeedLimit { get { return _speedLimit; } set { _speedLimit = value; } }
    public float RacingDist { get { return _racingDist; } set { _racingDist = value; } }
    #endregion
}
