using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurricaneSkill : MonoBehaviour
{
    [SerializeField]
    ObjectFullingInTest objectFulling;
    [SerializeField]
    GameObject player;
    [SerializeField]
    GameObject target;


    /// <summary>
    /// 폭풍의 시 Flow
    /// </summary>
    public void HurricaneFlow()
    {
        //화살이 캐릭터 위치에 생성
        GameObject arrow = objectFulling.MakeObj(0);
       
        //화살이 적 방향으로 향해야함
        Projective projective = arrow.GetComponent<Projective>();
        arrow.transform.position = player.transform.position;
        projective.InitArrowInfo(target.transform.position);
    }
}
