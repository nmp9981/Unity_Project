using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    void Update()
    {
        RacingFinish();
    }
    /// <summary>
    /// 기능 : 레이싱 종료 조건 체크
    /// 조건1 : 인게임 씬
    /// 조건2 : 남은 시간이 0이하 or 주행중인 상태가 아닐 때
    /// </summary>
    bool CheckFinish()
    {
        if (!SceneManager.GetActiveScene().name.Contains("KartGame"))
        {
            return false;
        }
        if (GameManager.Instance.CurrentRestTime <= 0 || 
            GameManager.Instance.CurrentLap > GameManager.Instance.mapLapList[GameManager.Instance.CurrentMapIndex])
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 주행 종료 알림
    /// </summary>
    async UniTask RacingFinish()
    {
        if (CheckFinish())
        {
            await UniTask.Delay(5000);
            GameManager.MoveToMainScene();
        }
    }
}
