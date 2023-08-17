using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamaManager : MonoBehaviour
{
    private static GamaManager _instance;//전체 게임 총괄

    //싱글톤
    public static GamaManager Instance
    {
        get
        {
            Init();//초기화
            return _instance;
        }
    }

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
}
