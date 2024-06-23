using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterFunction : MonoBehaviour
{
    MonsterSpawner monsterSpawner;
    ObjectFulling objectfulling;
    public string name;
    public long monsterFullHP;
    public long monsterHP;
    public long monsterExp;
    public int monsterGetMeso;
    int monsterDieCount;

    public int monsterHitDamage;//피격 데미지

    float curMoveAmount;
    float goalMoveAmount;
    Vector3 moveAmount;

    [SerializeField] Image monsterHPBarBack;
    [SerializeField] Image monsterHPBar;
    [SerializeField] TextMeshProUGUI monsterInfo;
    [SerializeField] TextMeshProUGUI[] hitDamage;

    void Awake()
    {
        monsterSpawner = GameObject.Find("MonsterSpawn").GetComponent<MonsterSpawner>();
        objectfulling = GameObject.Find("ObjectManager").GetComponent<ObjectFulling>();
    }
    private void OnEnable()
    {
        monsterHP = monsterFullHP;
        monsterDieCount = 0;
        goalMoveAmount = -1;
        MonsterMove();
        foreach (var damage in hitDamage) damage.text = "";
    }
    void Update()
    {
        MonsterUISetting();
        MonsterMove();
        isDie();
    }
    void MonsterUISetting()
    {
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
                monsterSpawner.GetComponent<MonsterSpawner>().mobCount -= 1;
                GameManager.Instance.PlayerEXP += monsterExp;
                int mobDrop = Random.Range(0, 100);
                if (mobDrop < 60)//메소 드랍
                {
                    GameObject mesoObj = objectfulling.MakeObj(6);
                    mesoObj.transform.position = gameObject.transform.position;
                    mesoObj.GetComponent<MonsterDrop>().monsterMeso = monsterGetMeso;
                }
            }
            Invoke("DieMonster", 0.35f);
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
        if(curMoveAmount >= goalMoveAmount)
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
}
