using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BumerangStepSkill : MonoBehaviour
{
    [SerializeField]
    GameObject knifeObject;
    [SerializeField]
    GameObject player;
    [SerializeField]
    GameObject dirToPlayer;

    const float maxBumerangStepDist = 15f;

    /// <summary>
    /// 부메랑 스텝
    /// </summary>
    /// <returns></returns>
    public IEnumerator BumerangStep()
    {
        BumerangStepFlow(0);
        yield return new WaitForSeconds(0.5f);
    }

    /// <summary>
    /// 부메랑스텝 Flow
    /// </summary>
    public void BumerangStepFlow(int hitNum)
    {
        //단검이 캐릭터 위치에 생성
        knifeObject.SetActive(true);

        //화살이 적 방향으로 향해야함
        Knife knife = knifeObject.GetComponent<Knife>();
        knifeObject.transform.position = player.transform.position + 0.2f * (hitNum - 1.5f) * Vector3.up;

        //플레이어 방향 벡터
        Vector3 playerDir = (dirToPlayer.transform.position - player.transform.position).normalized;

        knife.InitKnifeInfo(maxBumerangStepDist, playerDir, hitNum, 500);
    }
}
