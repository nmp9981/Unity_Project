using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class MenuUIManager : MonoBehaviour
{
    //선택한 맵 이름
    public static TextMeshProUGUI selectMapText;

    //시작 버튼
    Button startButton;
    private void Awake()
    {
        selectMapText = GameObject.Find("MapTextBackGround").GetComponentInChildren<TextMeshProUGUI>();
        
        startButton = GameObject.Find("RacingStart").GetComponentInChildren<Button>();
        startButton.onClick.AddListener(() => GoRacingStart(GameManager.Instance.CurrentMap));
    }

    /// <summary>
    /// 기능 : 고른 맵으로 이동
    /// </summary>
    /// <param name="mapName"></param>
    void GoRacingStart(string mapName)
    {
        //선택 안됨
        if (mapName == string.Empty) return;

        //선택된 맵의 시작지점으로 씬 이동
        SceneManager.LoadScene("KartGameMain");
    }
}
