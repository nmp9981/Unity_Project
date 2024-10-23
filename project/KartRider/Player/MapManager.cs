using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    static public List<Vector3> responPos;
    const int distInf = 1000000007;

    private void Awake()
    {
        SettingResponPosition();
    }
    /// <summary>
    /// 기능 : 각 맵별로 리스폰 위치 세팅
    /// 1) Respon태그를 달고 있는것만 추가
    /// </summary>
    void SettingResponPosition()
    {
        responPos = new List<Vector3>();
        foreach(Transform pos in gameObject.GetComponentsInChildren<Transform>())
        {
            if (pos.gameObject.CompareTag("Respon"))
            {
                responPos.Add(pos.position);
            }
        }
    }
    /// <summary>
    /// 기능 : 플레이어와 가장 가까운 리스폰 지점 찾기
    /// 1) 리스폰 지점들을 순회
    /// 2) 더 작은 거리가 보이면 갱신
    /// 3) 최종 리스폰 지점 반환
    /// </summary>
    /// <param name="playerPos"></param>
    /// <returns></returns>
    public static Vector3 SearchNearResponposition(Vector3 playerPos)
    {
        Vector3 resultRespon = Vector3.zero;
        float dist = distInf;
        foreach(var pos in responPos)
        {
            float playerToRespon = (playerPos - pos).magnitude;
            if(playerToRespon < dist)
            {
                dist = playerToRespon;
                resultRespon = pos;
            }
        }
        return resultRespon;
    }
}
