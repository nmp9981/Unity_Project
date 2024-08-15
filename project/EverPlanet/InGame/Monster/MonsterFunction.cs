using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterFunction : MonoBehaviour
{
    MonsterSpawner monsterSpawner;
    ObjectFulling objectfulling;
    InGameUI inGameUI;
    GameObject player;

    public int spawnPosNumber;
    public string name;
    public long monsterFullHP;
    public long monsterHP;
    public long monsterExp;
    public int monsterGetMeso;
    int monsterDieCount;

    public int monsterHitDamage;//피격 데미지

    float curMoveTime;
    float goalMoveTime;
    Vector3 moveAmount;

    [SerializeField] Image monsterHPBarBack;
    [SerializeField] Image monsterHPBar;
    [SerializeField] TextMeshProUGUI monsterInfo;
    [SerializeField] TextMeshProUGUI[] hitDamage;

    void Awake()
    {
        monsterSpawner = GameObject.Find("MonsterSpawn").GetComponent<MonsterSpawner>();
        objectfulling = GameObject.Find("ObjectManager").GetComponent<ObjectFulling>();
        inGameUI = GameObject.Find("UIManager").GetComponent<InGameUI>();
        player = GameObject.Find("Player");
    }
    private void OnEnable()
    {
        monsterHP = monsterFullHP;
        monsterDieCount = 0;
        goalMoveTime = 5;
        foreach (var damage in hitDamage) damage.text = "";
    }
    void Update()
    {
        MonsterUISetting();
        TimeFlow();
        MonsterMove();
        isDie();
    }
    void MonsterUISetting()
    {
        //플레이어와 일정 거리 이상이면 UI가 안보이게
        float dist = (player.transform.position - this.gameObject.transform.position).sqrMagnitude;
        if(dist > 1500)
        {
            monsterHPBarBack.gameObject.SetActive(false);
            monsterHPBar.gameObject.SetActive(false);
            monsterInfo.gameObject.SetActive(false);
        }
        else
        {
            monsterHPBarBack.gameObject.SetActive(true);
            monsterHPBar.gameObject.SetActive(true);
            monsterInfo.gameObject.SetActive(true);
        }

        monsterHPBarBack.transform.position = Camera.main.WorldToScreenPoint(this.gameObject.transform.position + new Vector3(0, 1f, 0));
        monsterHPBar.transform.position = Camera.main.WorldToScreenPoint(this.gameObject.transform.position + new Vector3(0, 1f, 0));
        monsterInfo.transform.position = Camera.main.WorldToScreenPoint(this.gameObject.transform.position + new Vector3(0, 1.5f, 0));

        for (int idx = 0; idx < hitDamage.Length; idx++) hitDamage[idx].transform.position = Camera.main.WorldToScreenPoint(this.gameObject.transform.position + new Vector3(0, idx+2f, 0));

        monsterHPBar.fillAmount = (float)monsterHP / (float)monsterFullHP;
        monsterInfo.text = name;
    }
    void isDie()
    {
        if (monsterHP <= 0)
        {
            monsterDieCount += 1;
            MonsterSpawner.spawnMonster.Remove(this.gameObject);
            if (monsterDieCount == 1)//죽었을 때 한번만 발돌
            {
                monsterSpawner.GetComponent<MonsterSpawner>().mobCount[spawnPosNumber] -= 1;
                GameManager.Instance.PlayerEXP += monsterExp;
                
                int mobDrop = Random.Range(0, 100);
                if (mobDrop < 60)//메소 드랍
                {
                    SoundManager._sound.PlaySfx(1);
                    GameObject mesoObj = objectfulling.MakeObj(26);
                    mesoObj.transform.position = gameObject.transform.position;
                    mesoObj.GetComponent<MonsterDrop>().monsterMeso = (int)((float)monsterGetMeso*GameManager.Instance.AddMeso/100.0f);
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
        if(collision.gameObject.tag == "Weapon")
        {
            for(int idx = 0; idx < hitDamage.Length; idx++)
            {
                if (hitDamage[idx].text == "")
                {
                    bool isShadow = collision.gameObject.GetComponent<DragFunction>().isShadow;
                    StartCoroutine(ShowDamage(hitDamage[idx],idx,isShadow,collision.gameObject));
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
        //벽과 부딪히면 방향을 바꾼다.
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
            else damage.color = new Color(219f/255f,132f/255f,0);
        }
        else if(gm.tag == "Avenger")
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
        if(curMoveTime >= goalMoveTime)//시간 초로 변경
        {
            curMoveTime = 0;
            float moveX = (float)(Random.Range(0, 3)-1);
            float moveZ = (float)(Random.Range(0, 3)-1);

            if (moveX == 0 && moveZ == 0) moveX = 1;//예외 처리
            goalMoveTime = Random.Range(2, 8);
            float speed = Random.Range(3, 7);
            moveAmount = new Vector3(moveX, 0, moveZ) * speed;
        }
        this.transform.position += moveAmount * Time.deltaTime;
    }
    //시간 흐름
    void TimeFlow()
    {
        curMoveTime += Time.deltaTime;
    }
}
