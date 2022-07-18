using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    BossHit bossHit;
    Attack attack;
    CSVReader CSVReader;//csv파일 읽기
    List<Dictionary<string, object>> data;

    public int lv = 1;//레벨
    int requestEXP;//요구 경험치

    public Text TotalDamageText;//총 데미지 텍스트
    public Text lvText;//레벨 표시
    public int TotalClicks;//총 데미지
    bool hasUpgrade;//업그레이드 여부
    public int autoClicksPerSecond;//초당 추가할 자동 클릭 수
    public int minimumClicksToUnlockUpgrade;//잠금를 해제하는데 필요한 클릭 수

    void Start()
    {
        data = CSVReader.Read("UserDataA"); //엑셀데이터 불러오기
        bossHit = GameObject.FindWithTag("User").GetComponent<BossHit>();//BossHit 스크립트에서 변수 가져오기
        requestEXP = (int)data[0]["경험치"];
    }

    //자동 클릭
    public void AutoClickUpgrade()
    {
        if(!hasUpgrade && TotalClicks>= minimumClicksToUnlockUpgrade)
        {
            TotalClicks -= minimumClicksToUnlockUpgrade;//잠금 해제시 차감
            hasUpgrade = true;
        }
    }
    //업데이트
    void Update()
    {
        if (hasUpgrade)
        {
            TotalClicks += autoClicksPerSecond * (int)Time.deltaTime;//시간에 따른 점수 추가
            TotalDamageText.text = TotalClicks.ToString("0");//화면에 보이게
        }
    }
    //레벨업
    public void LevelUP()
    {
        if (TotalClicks>= requestEXP)
        {
            TotalClicks -= requestEXP;
            TotalDamageText.text = TotalClicks.ToString("0");//화면에 보이게
            requestEXP = (int)data[lv]["경험치"];
            lv++;
            lvText.text = lv.ToString("0");//화면에 보이게
        }
    }
}
