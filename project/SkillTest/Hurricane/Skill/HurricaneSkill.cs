using UnityEngine;

public class HurricaneSkill : MonoBehaviour
{
    [SerializeField]
    ObjectFullingInTest objectFulling;
    [SerializeField]
    GameObject player;
    [SerializeField]
    GameObject target;
    [SerializeField]
    GameObject dirToPlayer;

    const float angleRange = 45;
    const float radToDeg = 57.2958f;

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

        //범위내에 드는가?
        bool isRange = IsRangeViewAngle();
        //플레이어 방향 벡터
        Vector3 playerDir = (dirToPlayer.transform.position - player.transform.position).normalized;

        projective.InitArrowInfo(target.transform.position, playerDir,isRange,0,100);
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
        Vector3 targetDir = (target.transform.position- player.transform.position).normalized;
       
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
}
