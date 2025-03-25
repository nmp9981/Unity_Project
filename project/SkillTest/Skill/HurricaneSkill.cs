using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HurricaneSkill : MonoBehaviour
{
    [SerializeField]
    ObjectFullingInTest objectFulling;
    [SerializeField]
    GameObject player;
    [SerializeField]
    GameObject dirToPlayer;

    Vector3 targetPosition;
    List<Vector3> targetList = new List<Vector3>();

    const float angleRange = 45;
    const float radToDeg = 57.2958f;

    private void Awake()
    {
        GameObject monsterSet = GameObject.Find("MonsterSet");
        foreach (Transform monster in monsterSet.GetComponentsInChildren<Transform>()) 
        {
            if(monster.gameObject.tag == "Monster")
            {
                targetList.Add(monster.position);
            }
        }
    }

    /// <summary>
    /// 폭풍의 시 Flow
    /// </summary>
    public void HurricaneFlow()
    {
        //화살이 캐릭터 위치에 생성
        GameObject arrow = objectFulling.MakeObj(20);
        
        //화살이 적 방향으로 향해야함
        Projective projective = arrow.GetComponent<Projective>();
        arrow.transform.position = player.transform.position;

        //타겟 선정
        targetPosition = FindNearestTarget();

        //범위내에 드는가?
        bool isRange = IsRangeViewAngle();
        //플레이어 방향 벡터
        Vector3 playerDir = (dirToPlayer.transform.position - player.transform.position).normalized;

        projective.InitArrowInfo(targetPosition, playerDir,isRange,0,100);
    }

    /// <summary>
    /// 적이 시야범위내로 들어가는가?
    /// </summary>
    /// <returns></returns>
    bool IsRangeViewAngle()
    {
        //플레이어 방향 벡터
        Vector3 playerDir = (dirToPlayer.transform.position- player.transform.position).normalized;
        //타겟까지의 방향
        Vector3 targetDir = (targetPosition - player.transform.position).normalized;
       
        //각도
        float angle = Vector3.Dot(playerDir, targetDir);

        //범위내에 든다
        if (angle>0)
        {
            return true;
        }
        //범위내에 들지 않음
        return false;
    }
    /// <summary>
    /// 타겟 찾기
    /// </summary>
    /// <returns></returns>
    Vector3 FindNearestTarget()
    {
        Vector3 targetPos = Vector3.zero;
        float dist = int.MaxValue;
        foreach(var target in targetList)
        {
            float curDist = Vector3.Distance(player.transform.position, target);
            if(curDist < dist)
            {
                dist = curDist;
                targetPos = target;
            }
        }
        return targetPos;
    }
}
