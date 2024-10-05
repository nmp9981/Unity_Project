using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosterManager : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        GetKeyBooster();
    }
    /// <summary>
    /// 기능 : 부스터 키 입력 받음
    /// </summary>
    async void GetKeyBooster()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            await BoosterOn();
        }
    }
    /// <summary>
    /// 기능 : 부스터 작동
    /// 1) 최고 제한속도를 늘린다.
    /// 2) 가속도를 늘린다.
    /// 3) 3초후 브레이크를 걸어 속도를 줄인다.(부스터 끝)
    /// 4) 최고 제한속도, 가속도 원래대로
    /// 5) 속도 줄이는 효과를 1초간 브레이크 주는것으로 구현
    /// TODO : 부스터 브레이크 파워,평소 브레이크 파워를 나눠야 함
    /// </summary>
    async UniTask BoosterOn()
    {
        GameManager.Instance.SpeedLimit = 175;
        GameManager.Instance.Touque = 1700;
   
        await UniTask.Delay(4000);
        
        GameManager.Instance.SpeedLimit = 140;
        GameManager.Instance.IsBooster = true;
        GameManager.Instance.BreakPower = 500000;

        await UniTask.Delay(1000);
        GameManager.Instance.Touque = 1000;
        GameManager.Instance.BreakPower = 20000;
        GameManager.Instance.IsBooster = false;
    }
}
