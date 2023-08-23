using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public int health;
    public string enemyName;
    public Sprite[] spriteImage;

    float coolTime = 0.2f;
    float curTime;
    float shootSpeed = 5.0f;

    SpriteRenderer spriteRenderer;//이미지 변경
    // Start is called before the first frame update
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    //컴포넌트 활성화할때 호출
    private void OnEnable()
    {
        switch (enemyName)
        {
            case "EnemyA":
                health = 4;
                break;
        }
        
    }
    // Update is called once per frame
    void Update()
    {
        Shoot();//총알 쏘기
        Reload();//장전
    }
    //총알 쏘기
    void Shoot()
    {
        if (coolTime > curTime) return;//쿨타임 안됨

        curTime = 0.0f;
    }

    //장전
    void Reload()
    {
        curTime += Time.deltaTime;
    }
    //피격
    private void OnHit(int dmg)
    {
        health -= dmg;
        if(health <= 0)//사망 처리
        {
            this.gameObject.SetActive(false);
            transform.rotation = Quaternion.identity;//기본 회전값 0
        }
        else
        {
            spriteRenderer.sprite = spriteImage[1];//투명하게
            Invoke("ChangeImage", 0.1f);
        }
    }
    void ChangeImage()
    {
        spriteRenderer.sprite = spriteImage[0];//원래대로
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Wall")//벽에 맞음
        {
            this.gameObject.SetActive(false);
            transform.rotation = Quaternion.identity;
        }
        else if(collision.tag == "PlayerBullet")//캐릭터한테 맞음
        {
            OnHit(1);
            collision.gameObject.SetActive(false);//총알이 사라짐
        }
    }
}
