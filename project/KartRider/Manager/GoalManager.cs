using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalManager : MonoBehaviour
{
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
                    if(GameManager.Instance.CurrentLap > GameManager.Instance.MapLap) GameManager.Instance.IsDriving = false;
                    break;
                case "Arrive":
                    GameManager.Instance.IsDriving = false;
                    break;
                default:
                    break;
            }
        }
    }
}
