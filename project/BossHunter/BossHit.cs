using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHit : MonoBehaviour
{
    Manager manager;
    SkillManager skillmgr;

    private float delayTime = 0.7f;
    int weight;//가중치
    [SerializeField] Text damage;
    [SerializeField] Text damage2;
    [SerializeField] Text damage3;

    bool isHit;
    // Start is called before the first frame update
    void Awake()
    {
        damage.text = "";
        damage2.text = "";
        damage3.text = "";
        manager = GameObject.FindWithTag("Manager").GetComponent<Manager>();//Manager 스크립트에서 변수 가져오기
        skillmgr = GameObject.FindWithTag("Skill").GetComponent<SkillManager>();//skillManager 스크립트에서 변수 가져오기
        damage.transform.position = Camera.main.WorldToScreenPoint(this.gameObject.transform.position) + new Vector3(550f, 100f, 0);
        damage2.transform.position = Camera.main.WorldToScreenPoint(this.gameObject.transform.position) + new Vector3(850f, 170f, 0);
        damage3.transform.position = Camera.main.WorldToScreenPoint(this.gameObject.transform.position) + new Vector3(550f, 300f, 0);

        isHit = false;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Dragger")//충돌시 데미지띄우고 경험치 먹기
        {
            StartCoroutine(LookDamage());
            weight = 10;
            isHit = true;
        }else if(collision.gameObject.tag == "Double")//충돌시 데미지띄우고 경험치 먹기
        {
            StartCoroutine(LookDoubleDamage());
            isHit = true;
            weight = 12;
        }
        else if (collision.gameObject.tag == "Triple")//충돌시 데미지띄우고 경험치 먹기
        {
            StartCoroutine(LookTripleDamage());
            isHit = true;
            weight = 15;
        }
        else if (collision.gameObject.tag == "Avenger")//충돌시 데미지띄우고 경험치 먹기
        {
            StartCoroutine(LookAvengerDamage());
            isHit = true;
            weight = 18;
        }
    }
    
    private void LateUpdate()
    {
        if (isHit)
        {
            manager.meso += (manager.getMoney*weight/10);
            manager.curExp += (manager.userAttack*weight/10);
            isHit = false;
        }
    }
    
    //데미지 띄우기(0.5초 지속)
    IEnumerator LookDamage()
    {
        damage.text = manager.userAttack.ToString();
        yield return new WaitForSecondsRealtime(delayTime);
        damage.text = "";
    }
    
    IEnumerator LookDoubleDamage()
    {
        int doubleDamage = Random.Range(manager.userAttack*11/10, manager.userAttack*13/10);
        damage.text = doubleDamage.ToString();
        damage2.text = doubleDamage.ToString();
        Debug.Log(damage2.transform.position);
        yield return new WaitForSecondsRealtime(delayTime);
        damage.text = "";
        damage2.text = "";
    }
    
    IEnumerator LookTripleDamage()
    {
        int tripleDamage = Random.Range(manager.userAttack*8/7, manager.userAttack * 3 / 2);
        damage.text = tripleDamage.ToString();
        damage2.text = tripleDamage.ToString();
        damage3.text = tripleDamage.ToString();
        yield return new WaitForSecondsRealtime(delayTime);
        damage.text = "";
        damage2.text = "";
        damage3.text = "";
    }
    IEnumerator LookAvengerDamage()
    {
        int avengerDamage = Random.Range(manager.userAttack * 5,manager.userAttack*6);
        damage.text = avengerDamage.ToString();
        yield return new WaitForSecondsRealtime(delayTime);
        damage.text = "";
    }
}
