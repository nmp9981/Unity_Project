using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MapManagement : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI mapName;
    [SerializeField] string[] mapNameList = new string[9];
    void Awake()
    {
        SoundManager._sound.PlayBGM(0);//맨처음에는 마을에서 시작 -> 마을 bgm
        mapName.text = $"{mapNameList[0]}";
    }

    void Update()
    {
        ShowMapName();
    }
    //맵 이름 표시
    void ShowMapName()
    {
        mapName.text = $"{mapNameList[GameManager.Instance.PlayerCurrentMap]}";
    }
}
