using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public string ownerName;
    public int hitAmount;
    float rotZ = 10.0f;//회전 각도

    //피격 데미지 설정
    private void Awake()
    {
        switch (ownerName)
        {
            case "EnemyA":
                hitAmount = 2;
                break;
            case "EnemyC":
                hitAmount = 3;
                break;
        }
    }
    /*
    //총알 상시 회전
    private void Update()
    {
        this.gameObject.transform.Rotate(0, 0, rotZ);        
    }
    */
    //벽에 부딪히면 비활성화
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Wall")
        {
            this.gameObject.SetActive(false);
        }

        //적 총알이 플레이어에게 맞으면
        if(collision.tag == "Player" && ownerName.Contains("Enemy"))
        {
            this.gameObject.SetActive(false);
        }
    }
}
