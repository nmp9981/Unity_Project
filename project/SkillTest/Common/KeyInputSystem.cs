using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyInputSystem : MonoBehaviour
{
    //스킬
    [SerializeField]
    HurricaneSkill hurricaneSkill;
    [SerializeField]
    StraipeSkill straipeSkill;
    [SerializeField]
    BumerangStepSkill bumerangStepSkill;

    //스킬 쿨타임
    float coolTimeInHurricane = 0.15f;
    float curTimeInHurricane = 0;

    float coolTimeInStraipe = 0.5f;
    float curTimeInStraipe = 0;

    float coolTimeInBumerangStep = 1f;
    float curTimeInBumerangStep = 0;

    //데미지 UI순서
    public static int orderSortNum { get; set; }
   
    private void Awake()
    {
        orderSortNum = 1;
    }
    void Update()
    {
        //레이어 번호 초기화
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X))
        {
            orderSortNum = 1;
        }
        //폭풍의 시
        if (Input.GetKey(KeyCode.Z))
        {
            if(CheckCooltime(curTimeInHurricane, coolTimeInHurricane))
            {
                curTimeInHurricane = 0;
                hurricaneSkill.HurricaneFlow();
            }
        }
        //스트레이프
        if (Input.GetKey(KeyCode.X))
        {
            if (CheckCooltime(curTimeInStraipe, coolTimeInStraipe))
            {
                orderSortNum = 1;
                curTimeInStraipe = 0;
                StartCoroutine(straipeSkill.StraipeShot());
            }
        }
        //부메랑스텝
        if (Input.GetKey(KeyCode.C))
        {
            if (CheckCooltime(curTimeInBumerangStep, coolTimeInBumerangStep))
            {
                orderSortNum = 1;
                curTimeInBumerangStep = 0;
                StartCoroutine(bumerangStepSkill.BumerangStep());
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
        curTimeInStraipe += Time.deltaTime;
        curTimeInBumerangStep += Time.deltaTime;
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
