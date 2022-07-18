using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Attack : MonoBehaviour
{
    Manager manager;
    AttackMgr attackMgr;
    public GameObject TextDamage;//데미지 표시
  
    [SerializeField] public GameObject Dagger;//표창 객체

    float throwSpeed = 10f;//표창 속도
    
    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.FindWithTag("Manager").GetComponent<Manager>();//Manager 스크립트에서 변수 가져오기
        attackMgr = GameObject.FindWithTag("AttackMgr").GetComponent<AttackMgr>();//Manager 스크립트에서 변수 가져오기
        TextDamage.SetActive(false);
    }

    void Update()
    {
        transform.Translate(transform.right * throwSpeed * Time.deltaTime);//표창 이동
    }
    //표창과 충돌
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Boss")
        {
            attackMgr.DamageText.text = attackMgr.Clicks.ToString("0");//화면에 보이게
            TextDamage.SetActive(true);//데미지 텍스트 활성화
            manager.TotalClicks += attackMgr.Clicks;
            manager.TotalDamageText.text = manager.TotalClicks.ToString("0");//화면에 보이게
            Destroy(gameObject);//충돌시 파괴
            Invoke("SeeDamage", 0.5f);//1초 지속
        }
    }
    void SeeDamage()//데미지는 1초만 보이게
    {
        TextDamage.SetActive(false);
    }
}
