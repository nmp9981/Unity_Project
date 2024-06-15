using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterFunction : MonoBehaviour
{
    MonsterSpawner monsterSpawner;
    public string name;
    public long monsterFullHP;
    public long monsterHP;
    public long monsterExp;
    int monsterDieCount;
    
    [SerializeField] Image monsterHPBarBack;
    [SerializeField] Image monsterHPBar;
    [SerializeField] TextMeshProUGUI monsterInfo;
    [SerializeField] TextMeshProUGUI[] hitDamage;
    void Awake()
    {
        monsterSpawner = GameObject.Find("MonsterSpawn").GetComponent<MonsterSpawner>();
    }
    private void OnEnable()
    {
        monsterHP = monsterFullHP;
        monsterDieCount = 0;
        foreach (var damage in hitDamage) damage.text = "";
    }
    void Update()
    {
        MonsterUISetting();
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
                    StartCoroutine(ShowDamage(hitDamage[idx],idx));
                    return;
                }
            }
        }
    }
    //데미지 보여주기
    IEnumerator ShowDamage(TextMeshProUGUI damage, int idx)
    {
        damage.text = GameManager.Instance.PlayerAttack.ToString();
        yield return new WaitForSeconds(0.5f);
        damage.text = "";
    }
}
