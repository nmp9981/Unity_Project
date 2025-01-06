using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CastleAttack : MonoBehaviour
{
    public GameObject throwObjectPrefab;
    GameObject finalTarget;
    float maxDist = 99999999;
    void Start()
    {
        InvokeRepeating("SearchNearTarget", 0.5f, 1f);
    }

    void CreateThrowObject()
    {
        GameObject throwObject = Instantiate(throwObjectPrefab);
        throwObject.transform.position = this.gameObject.transform.position;
        throwObject.GetComponent<ThrowObject>().target = finalTarget;
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
            float dist = (gm.gameObject.transform.position - this.gameObject.transform.position).sqrMagnitude;
            //더 가까운 거리 (사거리 내)
            if (dist < curDist && dist < 200)
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
