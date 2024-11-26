using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalManager : MonoBehaviour
{
    //완주 시간
    float passGoalTime;
    /// <summary>
    /// 기능 : 출발지점, 도착지점 통과여부
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            switch (this.gameObject.tag)
            {
                case "Start":
                    GameManager.Instance.IsDriving = true;
                    GameManager.Instance.CurrentLap += 1;
                    BestLapTimeChange();
                    if (GameManager.Instance.CurrentLap > GameManager.Instance.MapLap) GameManager.Instance.IsDriving = false;
                    break;
                case "Arrive":
                    GameManager.Instance.IsDriving = false;
                    break;
                default:
                    break;
            }
        }
    }
    /// <summary>
    /// 기능 : 베타 변경
    /// 1) 더 빠른 기록일 경우 갱신
    /// 2) 첫 바퀴일 경우 갱신
    /// </summary>
    void BestLapTimeChange()
    {
        //첫 바퀴 시작
        if (GameManager.Instance.CurrentLap <= 1)
        {
            //완주 시간
            passGoalTime = 0;
            GameManager.Instance.BestLapTime = 0;
            return;
        }
       
        //첫 바퀴 완주
        if (GameManager.Instance.CurrentLap == 2)
        {
            GameManager.Instance.BestLapTime = GameManager.Instance.CurrentTime;
            passGoalTime = GameManager.Instance.CurrentTime;
            return;
        }
        //시간 차이
        float diffLapTime = GameManager.Instance.CurrentTime - passGoalTime;
        // 더 빠른 기록
        if(diffLapTime < GameManager.Instance.BestLapTime)
        {
            GameManager.Instance.BestLapTime = diffLapTime;
        }
        passGoalTime = GameManager.Instance.CurrentTime;
    }
}
