using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    static GameManager _instance;
    InputManager _inputManager = new InputManager();

    public static GameManager Instance { get { Init(); return _instance; } }
    public static InputManager Input { get { return Instance._inputManager; } }

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
        
    }

    void Update()
    {
        
    }
    #region 데이터
    float _playerMoveSpeed = 4.0f;
    float _playerDir = -1.0f;
    float _playerScale = 1.0f;
    long _playerAttack = 2;

    public float PlayerMoveSpeed { get { return _playerMoveSpeed; } set { _playerMoveSpeed = value; } }
    public float PlayerDir { get { return _playerDir; } set { _playerDir = _playerDir == 0 ? 1.0f: value; } }
    public float PlayerScale { get { return _playerScale; } set { _playerScale = value; } }
    public long PlayerAttack { get { return _playerAttack; } set { _playerAttack = value; } }
    #endregion
}
