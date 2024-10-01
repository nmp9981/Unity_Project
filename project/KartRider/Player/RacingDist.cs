using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacingDist : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(MeasureRacingDist());
    }
    /// <summary>
    /// 기능 : 주행 거리 측정
    /// 1) 이전 위치를 받기
    /// 2) 0.1초후의 위치 받기
    /// 3) 이전, 이후 위치를 통해 주행 거리 측정
    /// </summary>
    IEnumerator MeasureRacingDist()
    {
        while (true)
        {
            Vector3 beforePos = gameObject.transform.position;
            yield return new WaitForSeconds(0.1f);
            Vector3 afterPos = gameObject.transform.position;
            //0.1초 간 이동한 거리 측정
            GameManager.Instance.RacingDist += (afterPos - beforePos).magnitude;
        }
    }
}
