using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class BoosterManager : MonoBehaviour
{
    List<Image> boosterUIImage = new List<Image>();
    [SerializeField]
    ParticleSystem boosterEffect;
    private void Awake()
    {
        BoosterUIImageInit();
    }
    void Update()
    {
        GetKeyBooster(); 
    }
    /// <summary>
    /// 기능 : 부스터 아이템 이미지 초기화
    /// </summary>
    void BoosterUIImageInit()
    {
        Image item1UI = GameObject.Find("Item1UI").GetComponent<Image>();
        Image item2UI = GameObject.Find("Item2UI").GetComponent<Image>();
        
        boosterUIImage.Add(item1UI);
        boosterUIImage.Add(item2UI);

        foreach(Image img in boosterUIImage)
        {
            img.enabled = false;
        }
    }
    /// <summary>
    /// 기능 : 부스터 키 입력 받음
    /// 1) 왼쪽 ctrl 키 누를 경우 작동
    /// 2) 현재 부스터 개수 1개 이상일 때만 작동
    /// 3) 주행중일 때만
    /// </summary>
    async void GetKeyBooster()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && GameManager.Instance.BoosterCount>=1 && GameManager.Instance.IsDriving)
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
    /// 6) 부스터 개수 1개 감소 및 UI처리
    /// 7) 부스터 On/Off에 따라 이펙트 On/Off
    /// TODO : 부스터 브레이크 파워,평소 브레이크 파워를 나눠야 함
    /// </summary>
    async UniTask BoosterOn()
    {
        GameManager.Instance.SpeedLimit = 214;
        GameManager.Instance.Touque = 2500;
        GameManager.Instance.BoosterCount -= 1;
        boosterUIImage[GameManager.Instance.BoosterCount].enabled = false;
        boosterEffect.Play();
        SoundManger._sound.PlaySfx((int)SFXSound.Booster);
        //부스터 사운드 On

        await UniTask.Delay(4000);
        
        GameManager.Instance.SpeedLimit = 172;
        GameManager.Instance.IsBooster = true;
        GameManager.Instance.BreakPower = 1100000;
        boosterEffect.Stop();
        //부스터 사운드 Off

        await UniTask.Delay(2500);
        GameManager.Instance.Touque = 1500;
        GameManager.Instance.BreakPower = 200000;
        GameManager.Instance.IsBooster = false;
    }
    /// <summary>
    /// 기능 : 부스터 획득 
    /// 1) 부스터 개수 증가
    /// 2) 현재 부스터 게이지 0으로 초기화
    /// 3) 부스터 이미지 업데이트
    /// 4) 부스터는 최대 2개
    /// </summary>
    public void BoosterGet()
    {
        //현재 부스터가 1개이하인 경우
        if (GameManager.Instance.BoosterCount < 2)
        {
            GameManager.Instance.BoosterCount += 1;
            boosterUIImage[GameManager.Instance.BoosterCount - 1].enabled = true;
        }
        GameManager.Instance.CurrentBoosterGage -= 50*Time.deltaTime;
    }
}
