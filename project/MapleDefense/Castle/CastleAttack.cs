
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CastleAttack : MonoBehaviour
{
    [SerializeField]
    ObjectFulling objectFulling;

    [SerializeField]
    float rangeDist;//사거리
    [SerializeField]
    float attackBetween;//공격 간격

    public int weaponAttack;//무기 공격력

    public GameObject throwObjectPrefab;
    GameObject finalTarget;
    float maxDist = 99999999;
    void Start()
    {
        //3번째 인자가 공격 속도
        InvokeRepeating("SearchNearTarget", 0.5f, attackBetween);
    }
    /// <summary>
    /// 기능 : 타겟 향해 총알 발사
    /// 무기도 타겟을 향하게
    /// </summary>
    void CreateThrowObject()
    {
        GameObject throwObject = objectFulling.MakeObj(2);
        throwObject.transform.position = this.gameObject.transform.position;
        throwObject.GetComponent<ThrowObject>().TargetSetting(finalTarget);
        //어느 무기에서 발사했는가?
        throwObject.GetComponent<ThrowObject>().fromWeapon = this;
        //무기도 타겟을 향하게
        gameObject.transform.LookAt(finalTarget.transform.position);
    }
    /// <summary>
    /// 가장 가까운 타겟 찾기
    /// </summary>
    void SearchNearTarget()
    {
        finalTarget = null;
        //대상 없음
        if (GameManager.Instance.ActiveUnitList.Count == 0)
        {
            return;
        }

        float curDist = maxDist;
        foreach (var gm in GameManager.Instance.ActiveUnitList)
        {
            //게임 오브젝트 존재여부
            if (gm == null)
            {
                continue;
            }
            float dist = (gm.gameObject.transform.position - this.gameObject.transform.position).sqrMagnitude;
            //더 가까운 거리 (사거리 내)
            if (dist < curDist && dist < rangeDist)
            {
                curDist = dist;
                finalTarget = gm;
            }
        }
        //타겟 존재시 총알생성
        if(finalTarget != null)
        {
            CreateThrowObject();
        }
    }
}
