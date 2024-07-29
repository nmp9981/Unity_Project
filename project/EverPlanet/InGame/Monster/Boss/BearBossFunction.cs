using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BearBossFunction : MonoBehaviour
{
    MonsterSpawner monsterSpawner;
    ObjectFulling objectfulling;
    InGameUI inGameUI;
    GameObject player;

    public int mobID;
    public string name;
    public long monsterFullHP;
    public long monsterHP;
    public long monsterExp;
    public int monsterGetMeso = 18500;
    int monsterDieCount;

    public int monsterHitDamage;//피격 데미지

    float curMoveAmount;
    float goalMoveAmount;
    Vector3 moveAmount;

    [SerializeField] Image monsterHPBarBack;
    [SerializeField] Image monsterHPBar;
    [SerializeField] Image monsterInfo;
    [SerializeField] TextMeshProUGUI hpInfo;
    [SerializeField] TextMeshProUGUI[] hitDamage;

    GameObject circle;
    GameObject stoneObject;
    [SerializeField] GameObject armObject;

    float bearCurTime = 1f;
    float bearCoolTime = 9f;
    float betweenDist;

    void Awake()
    {
        monsterSpawner = GameObject.Find("MonsterSpawn").GetComponent<MonsterSpawner>();
        objectfulling = GameObject.Find("ObjectManager").GetComponent<ObjectFulling>();
        inGameUI = GameObject.Find("UIManager").GetComponent<InGameUI>();
        player = GameObject.Find("Player");
        circle = GameObject.Find("Circle");
        stoneObject = GameObject.Find("Rock_3");
    }
    private void OnEnable()
    {   
        monsterHP = monsterFullHP;
        monsterDieCount = 0;
        goalMoveAmount = -1;

        if (gameObject.name.Contains("Bear")) circle.SetActive(false);
        MonsterMove();
        foreach (var damage in hitDamage) damage.text = "";
    }
    private void Start()
    {
        if(gameObject.name.Contains("Bear")) InvokeRepeating("SkillActive", 5f, bearCoolTime);//곰일 경우에만 발동
    }
    void Update()
    {
        MonsterUISetting();
        MonsterMove();
        isDie();
        TimeFlow();
    }
    void MonsterUISetting()
    {
        //플레이어와 일정 거리 이상이면 UI가 안보이게
        betweenDist = (player.transform.position - this.gameObject.transform.position).sqrMagnitude;
        if (betweenDist > 1500)
        {
            monsterHPBarBack.gameObject.SetActive(false);
            monsterHPBar.gameObject.SetActive(false);
            monsterInfo.gameObject.SetActive(false);
            hpInfo.text = string.Empty;
        }
        else
        {
            monsterHPBarBack.gameObject.SetActive(true);
            monsterHPBar.gameObject.SetActive(true);
            monsterInfo.gameObject.SetActive(true);
            hpInfo.text = $"{monsterHP} / {monsterFullHP}";
        }

        for (int idx = 0; idx < hitDamage.Length; idx++) hitDamage[idx].transform.position = Camera.main.WorldToScreenPoint(this.gameObject.transform.position + new Vector3(0, idx + 2f, 0));

        monsterHPBar.fillAmount = (float)monsterHP / (float)monsterFullHP;
    }
    void isDie()
    {
        if (monsterHP <= 0)
        {
            monsterDieCount += 1;
            MonsterSpawner.spawnMonster.Remove(this.gameObject);
            if (gameObject.name.Contains("Bear")) circle.SetActive(false);
            if (monsterDieCount == 1)//죽었을 때 한번만 발돌
            {
                GameManager.Instance.PlayerEXP += monsterExp;

                int mobDrop = Random.Range(0, 100);
                if (mobDrop < 60)//메소 드랍
                {
                    SoundManager._sound.PlaySfx(1);
                    GameObject mesoObj = objectfulling.MakeObj(26);
                    mesoObj.transform.position = gameObject.transform.position;
                    mesoObj.GetComponent<MonsterDrop>().monsterMeso = (int)((float)monsterGetMeso * GameManager.Instance.AddMeso / 100.0f);
                }
                inGameUI.ShowGetText("Exp", (int)monsterExp);
            }
            Invoke("DieMonster", 0.45f);
        }
    }
    void DieMonster()
    {
        gameObject.SetActive(false);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Weapon")
        {
            for (int idx = 0; idx < hitDamage.Length; idx++)
            {
                if (hitDamage[idx].text == "")
                {
                    bool isShadow = collision.gameObject.GetComponent<DragFunction>().isShadow;
                    StartCoroutine(ShowDamage(hitDamage[idx], idx, isShadow, collision.gameObject));
                    return;
                }
            }
        }
        if (collision.gameObject.tag == "Avenger")
        {
            for (int idx = 0; idx < hitDamage.Length; idx++)
            {
                if (hitDamage[idx].text == "")
                {
                    bool isShadow = collision.gameObject.GetComponent<AvengerSkill>().isShadow;
                    StartCoroutine(ShowDamage(hitDamage[idx], idx, isShadow, collision.gameObject));
                    return;
                }
            }
        }
    }
    //데미지 보여주기
    IEnumerator ShowDamage(TextMeshProUGUI damage, int idx, bool isShadow, GameObject gm)
    {
        long finalDamage = 0;
        yield return new WaitForSeconds(0.05f);
        if (gm.tag == "Weapon")
        {
            finalDamage = gm.GetComponent<DragFunction>().attackDamage;
            if (gm.GetComponent<DragFunction>().isCritical) damage.color = Color.red;
            else damage.color = new Color(219f / 255f, 132f / 255f, 0);
        }
        else if (gm.tag == "Avenger")
        {
            finalDamage = gm.GetComponent<AvengerSkill>().attackDamage;
            if (gm.GetComponent<AvengerSkill>().isCritical) damage.color = Color.red;
            else damage.color = new Color(219f / 255f, 132f / 255f, 0);
        }

        damage.text = finalDamage.ToString();
        yield return new WaitForSeconds(0.5f);
        damage.text = "";
    }
    //몬스터 이동
    void MonsterMove()
    {
        if (curMoveAmount >= goalMoveAmount)
        {
            curMoveAmount = 0;
            float moveX = Random.Range(-1, 2);
            float moveZ = Random.Range(-1, 2);

            if (moveX == 0 && moveZ == 0) moveX = 1;//예외 처리
            goalMoveAmount = Random.Range(3, 18);
            float speed = Random.Range(3, 7);
            moveAmount = new Vector3(moveX, 0, moveZ) * speed;
        }
        this.transform.position += moveAmount * Time.deltaTime;
        curMoveAmount += moveAmount.magnitude;
    }
   void SkillActive()
    {
        if(betweenDist < 1500 && gameObject.activeSelf)
        {
            int ran = Random.Range(0, 10);
            if (ran % 2 == 0) StartCoroutine(MeteoStorm());
            else StartCoroutine(Claw());
        }
    }
    void TimeFlow()
    {
        bearCurTime += Time.deltaTime; 
    }
    //메테오 스톰
    IEnumerator MeteoStorm()
    {
        //캐릭터 위치에 원을 띄운다
        circle.SetActive(true);
        Vector3 targetPos = player.transform.position - new Vector3(0,0.5f,0);
        circle.transform.position = targetPos;//목표 위치
        yield return new WaitForSeconds(3f);
        //돌을 목표 타겟한테 던진다.
        stoneObject.GetComponent<Stome>().Init(targetPos, circle);
    }
    //할퀴기
    IEnumerator Claw()
    {
        //휘두름, 애니로 해결
        armObject.transform.localScale = new Vector3(1, 11, 1);
        yield return new WaitForSeconds(1f);
        armObject.transform.localScale = new Vector3(1, 1, 1);
    }
}
