using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacingDist : MonoBehaviour
{
    GameObject startPos;
    // Start is called before the first frame update
    void Awake()
    {
        startPos = GameObject.Find("StartPos");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CalRacingDist();
    }
    /// <summary>
    /// 주행거리 계산
    /// </summary>
    void CalRacingDist()
    {
        GameManager.Instance.RacingDist = Vector3.Distance(gameObject.transform.position , startPos.transform.position);
    }
}
