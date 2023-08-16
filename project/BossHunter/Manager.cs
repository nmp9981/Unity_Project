using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    BossHit bossHit;
    Upgrage upgrade;
    CSVReader CSVReader;//csv파일 읽기
    List<Dictionary<string, object>> data;
    [SerializeField] GameObject guideMessage;//가이드 메시지
    [SerializeField] GameObject statMessage;//스탯창
    [SerializeField] Text userInfoText;//유저 정보

    const int increaseSkillPoint = 2;//스킬 포인트 증가량
    private string userName = "아그리콜라";
    public int userAttack;
    public int lv;//레벨
    public int getMoney;//얻는 돈
    public int requestEXP;//요구 경험치
    public int curExp;//현재 경험치
    public Text ExpText;//경험치 텍스트

    public Text Money;//돈 텍스트
    public Text lvText;//레벨 표시
    public long meso;//현재 돈

    bool hasUpgrade;//업그레이드 여부
    public int autoClicksPerSecond;//초당 추가할 자동 클릭 수
    public int minimumClicksToUnlockUpgrade;//잠금를 해제하는데 필요한 클릭 수

    void Awake()
    {
        lv = 1;
        Time.timeScale = 1.0f;
        guideMessage.SetActive(false);
        statMessage.SetActive(false);

        data = CSVReader.Read("UserDataA"); //엑셀데이터 불러오기
        bossHit = GameObject.FindWithTag("User").GetComponent<BossHit>();//BossHit 스크립트에서 변수 가져오기
        upgrade = GameObject.Find("Upgrade").GetComponent<Upgrage>();//Upgrade 스크립트에서 변수 가져오기
        requestEXP = (int)data[lv-1]["경험치"];
        userAttack = (int)data[lv-1]["공격력"];
        getMoney = (int)data[lv - 1]["돈"];
    }

    //자동 클릭
    public void AutoClickUpgrade()
    {
        if(!hasUpgrade && meso>= minimumClicksToUnlockUpgrade)
        {
            meso -= minimumClicksToUnlockUpgrade;//잠금 해제시 차감
            hasUpgrade = true;
        }
    }
    //업데이트
    void Update()
    {
        ExpText.text = curExp.ToString() + " / " + requestEXP.ToString();//경험치 표시
        Money.text = meso.ToString() + " 메소";
        lvText.text = lv.ToString("0");//화면에 보이게
        if (hasUpgrade)
        {
            meso += autoClicksPerSecond * (int)Time.deltaTime;//시간에 따른 점수 추가
        }
    }
    //레벨업
    public void LevelUP()
    {
        if (curExp>= requestEXP)
        {
            curExp = 0;
            userAttack = (int)data[lv]["공격력"];
            requestEXP = (int)data[lv]["경험치"];
            getMoney = (int)data[lv]["돈"];
            upgrade.skillPoint += increaseSkillPoint;
            lv++;
        }
    }
    //메세지 켜기
    public void OnGuideMessage()
    {
        Debug.Log("출력");
        guideMessage.SetActive(true);
        Time.timeScale = 0.0f;//일시 정지
    }
    //메세지 끄기
    public void OffGuideMessageBox()
    {
        Debug.Log("해제");
        guideMessage.SetActive(false);
        Time.timeScale = 1.0f;//다시 정상 시간으로
    }
    //스탯창 띄우기
    public void OnStatMessage()
    {
        //닫혀있으면 열고 열려있으면 닫는다.
        if (statMessage.activeSelf == false)
        {
            statMessage.SetActive(true);
            //정보들
            userInfoText.text = "이름 : " + userName + "\n레벨 : " + lv.ToString() + "\n공격력 : " + userAttack;
            Time.timeScale = 0.0f;//일시 정지

        }
        else OffStatMessage();
        
    }
    //스탯창 끄기
    void OffStatMessage()
    {
        statMessage.SetActive(false);
        Time.timeScale = 1.0f;//다시 원래대로
    }
}
