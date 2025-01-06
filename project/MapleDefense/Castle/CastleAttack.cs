using UnityEngine;

public class CastleAttack : MonoBehaviour
{
    public GameObject throwObjectPrefab;
    float maxDist = 99999999;
    void Start()
    {
        SearchNearTarget();
    }

    /// <summary>
    /// 가장 가까운 타겟 찾기
    /// </summary>
    void SearchNearTarget()
    {
        GameObject finalTarget = null;
        //대상 없음
        if (GameManager.Instance.ActiveUnitList.Count == 0)
        {
            return;
        }

        float curDist = maxDist;
        foreach (var gm in GameManager.Instance.ActiveUnitList)
        {
            float dist = (gm.gameObject.transform.position - this.gameObject.transform.position).sqrMagnitude;
            //더 가까운 거리
            if (dist<curDist)
            {
                curDist = dist;
                finalTarget = gm;
            }
        }

        GameObject throwObject = Instantiate(throwObjectPrefab);
        throwObject.GetComponent<ThrowObject>().target = finalTarget;
    }
}
