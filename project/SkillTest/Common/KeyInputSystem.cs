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
        TimeFlow();
    }
    /// <summary>
    /// 시간 흐름
    /// </summary>
    void TimeFlow()
    {
        curTimeInHurricane += Time.deltaTime;
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
