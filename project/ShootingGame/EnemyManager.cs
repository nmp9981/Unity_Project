using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public ObjectManager objectManager;
    
    public int health;
    public string enemyName;
    public Sprite[] spriteImage;

    public GameObject bullet1;

    float enemySpeed = 1.7f;
    float coolTime = 1.0f;
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
        MoveEnemy();//이동
        Shoot();//총알 쏘기
        Reload();//장전
    }
    //이동
    private void MoveEnemy()
    {
        this.gameObject.transform.Translate(Vector3.down * enemySpeed*Time.deltaTime);
    }
    //총알 쏘기
    void Shoot()
    {
        if (coolTime > curTime) return;//쿨타임 안됨
        
        curTime = 0.0f;
        switch (enemyName)
        {
            case "EnemyA":
                GameObject bullet1 = objectManager.MakeGameObject("EnemyBulletA");//총알 소환
                bullet1.transform.position = this.gameObject.transform.position;

                Rigidbody2D rigid = bullet1.GetComponent<Rigidbody2D>();
                rigid.AddForce(Vector3.down * shootSpeed, ForceMode2D.Impulse);//앞으로 쏘기
                break;
        }
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
