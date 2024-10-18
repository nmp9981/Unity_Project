using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameLoading : MonoBehaviour
{
    static GameLoading _loadingInstance;

    public static GameLoading LoadingInstance { get { Init(); return _loadingInstance; } }
    public static Image _loadingImage;
    static void Init()
    {
        if (_loadingInstance == null)
        {
            GameObject gm = GameObject.Find("LoadingCanvas");
            if (gm == null)
            {
                gm = new GameObject { name = "LoadingCanvas" };

                gm.AddComponent<GameLoading>();
            }
            DontDestroyOnLoad(gm);
            _loadingInstance = gm.GetComponent<GameLoading>();
            _loadingImage = gm.GetComponentInChildren<Image>();
        }
    }
    private void Awake()
    {
        Init();
    }
    public static void LoadingOn()
    {
        _loadingImage.enabled = true;
    }
    public static void LoadingOff()
    {
        _loadingImage.enabled = false;
    }
}
