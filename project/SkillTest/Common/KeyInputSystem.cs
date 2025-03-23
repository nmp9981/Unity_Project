using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyInputSystem : MonoBehaviour
{
    //스킬
    [SerializeField]
    HurricaneSkill hurricaneSkill;

    //스킬 쿨타임
    float coolTimeInHurricane = 0.2f;
    float curTimeInHurricane = 0;

    //데미지 UI순서
    public static int orderSortNum { get; set; }
    float curTimeNotAnyKey = 0;
    float coolTimeNotAnyKey = 5;

    private void Awake()
    {
        orderSortNum = 1;
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.Z))
        {
            if(CheckCooltime(curTimeInHurricane, coolTimeInHurricane))
            {
                curTimeInHurricane = 0;
                hurricaneSkill.HurricaneFlow();
            }
        }
        else
        {
            curTimeNotAnyKey = 0;
        }
        TimeFlow();
        InitSortInQueueNum();
    }
    /// <summary>
    /// 시간 흐름
    /// </summary>
    void TimeFlow()
    {
        curTimeInHurricane += Time.deltaTime;
        curTimeNotAnyKey += Time.deltaTime;
    }
    /// <summary>
    /// 레이어 수 초기화
    /// </summary>
    void InitSortInQueueNum()
    {
        if(curTimeNotAnyKey >= coolTimeNotAnyKey)
        {
            orderSortNum = 1;
        }
    }
    /// <summary>
    /// 쿨타임 지났는지 검사
    /// </summary>
    /// <param name="curTime">현재 시간</param>
    /// <param name="goalTime">쿨 타임</param>
    /// <returns></returns>
    bool CheckCooltime(float curTime, float goalTime)
    {
        if (curTime >= goalTime)
        {
            return true;
        }
        return false;
    }
}
