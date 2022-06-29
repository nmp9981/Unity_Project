using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class item : MonoBehaviour
{
    FireBall fireball;
    // Start is called before the first frame update

    public Rigidbody2D fallingitem;
    public float itemspeed = 3f;

    void Start()
    {
        fireball = GameObject.FindWithTag("FireBall").GetComponent<FireBall>();//FireBall 스크립트에서 변수 가져오기
        fallingitem = GetComponent<Rigidbody2D>();
        InvokeRepeating("BalanceTime", 7.0f, 7.0f); //7.0f 마다 "BalanceTime" 함수 반복
    }

    // Update is called once per frame
    void Update()
    {
        fallingitem.velocity = Vector3.down * itemspeed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Umbrella")//우산에 닿으면 삭제
        {
            Destroy(gameObject);//객체 파괴
        }
        else if (other.gameObject.tag == "Ground")//땅에 닿으면 체력 감소
        {
            fireball.Current_HP -= fireball.water_damage;
            fireball.Current_HP = Mathf.Max(fireball.Current_HP, 0);//HP가 0보다 작아질 수 없음
            Destroy(gameObject);//객체 파괴
        }
    }
    void BalanceTime() //빗방울 속도 + 개수 밸런스 조절 함수
    {
        if(itemspeed >= 10.0f)
            return;
            
        itemspeed += 1.0f; //빗방울 속도 밸런스 조절
    }
}
