using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string enemyName;
    public float speed;
    public int health;
    public int enemyScore;

    public Sprite[] sprites;
    public float maxShotDelay;//실제 쿨타임
    public float curShotDelay;//경과 시간

    public GameObject player;
    public GameObject bulletObjA;
    public GameObject bulletObjB;

    public GameObject itemCoin;
    public GameObject itemBoom;
    public GameObject itemPower;

    SpriteRenderer spriteRenderer;//이미지 변경
    Animator anim;

    public ObjectManager objectManager;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (enemyName == "B")
        {
            anim = GetComponent<Animator>();
        }
    }
    private void OnEnable()//컴포넌트가 활성화될 때 호출
    {
        switch (enemyName)
        {
            case "B":
                health = 300;
                Invoke("Stop", 2);
                break;
            case "S":
                health = 4;
                break;
            case "M":
                health = 12;
                break;
            case "L":
                health = 24;
                break;
        }
    }
    //보스 멈추기
    private void StopBoss()
    {
        //stop함수가 2번 사용되지 않도록
        if (!gameObject.activeSelf) return;

        Rigidbody2D rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = Vector2.zero;

        Invoke("Think", 2);
    }

    //보스 패턴
    void Think()
    {

    }
    void Update()
    {
        if (enemyName == "B") return;
        Fire();//총쏘기
        Reload();//장전
    }

    void Fire()
    {
        if (curShotDelay < maxShotDelay) return;//쿨타임이 안됨

        if(enemyName == "S")
        {
            GameObject bullet = objectManager.MakeObj("BulletEnemyA");//총알 생성
            bullet.transform.position = transform.position;

            //GameObject bullet = Instantiate(bulletObjA, this.transform.position, this.transform.rotation);//총알 생성
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();

            Vector3 dirVec = player.transform.position - transform.position;//방향 벡터
            rigid.AddForce(dirVec.normalized * speed, ForceMode2D.Impulse);
        }else if(enemyName == "L")//2발 쏨
        {
            GameObject bulletR = objectManager.MakeObj("BulletEnemyB");//총알 생성
            bulletR.transform.position = transform.position + Vector3.right * 0.3f;
            GameObject bulletL = objectManager.MakeObj("BulletEnemyB");//총알 생성
            bulletL.transform.position = transform.position + Vector3.left * 0.3f;

            //GameObject bulletR = Instantiate(bulletObjB, this.transform.position+Vector3.right*0.3f, this.transform.rotation);//총알 생성
            //GameObject bulletL = Instantiate(bulletObjB, this.transform.position+Vector3.left*0.3f, this.transform.rotation);//총알 생성

            Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
            Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();

            Vector3 dirVecR = player.transform.position - (transform.position+Vector3.right*0.3f);
            Vector3 dirVecL = player.transform.position - (transform.position+Vector3.left*0.3f);
            rigidR.AddForce(dirVecR.normalized * speed, ForceMode2D.Impulse);//단위벡터로 (안그러면 크기 1을 넘어가 빠르게 쏘짐)
            rigidL.AddForce(dirVecL.normalized * speed, ForceMode2D.Impulse);
        }
        curShotDelay = 0;//쿨타임 초기화
    }
    //장전
    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }

    //피격
    public void OnHit(int dmg)
    {
        if (health <= 0) return;//예외처리, 이미 죽은건 드랍X

        health -= dmg;
        if (enemyName == "B")//보스
        {
            anim.SetTrigger("OnHit");
        }
        else//일반 몹
        {
            spriteRenderer.sprite = sprites[1];//흰색 이미지로
            Invoke("ReturnSprite", 0.1f);//0.1초 뒤 원래대로
        }

        if (health <= 0)
        {
            Player playerLogic = player.GetComponent<Player>();
            playerLogic.score += enemyScore;

            //아이템 드랍
            int ran = enemyName=="B"?0: Random.Range(0, 100);
            if (ran >= 77)
            {
                GameObject itemBoom = objectManager.MakeObj("ItemBoom");
                itemBoom.transform.position = transform.position;
            
                //Instantiate(itemBoom, transform.position, itemBoom.transform.rotation);
            }
            else if (ran >= 40)
            {
                GameObject itemPower = objectManager.MakeObj("ItemPower");
                itemPower.transform.position = transform.position;
                
                //Instantiate(itemPower, transform.position, itemPower.transform.rotation);
            }
            else if (ran >= 11)
            {
                GameObject itemCoin = objectManager.MakeObj("ItemCoin");
                itemCoin.transform.position = transform.position;
                
                //Instantiate(itemCoin, transform.position, itemCoin.transform.rotation);
            }

            this.gameObject.SetActive(false);//사망
            transform.rotation = Quaternion.identity;//기본 회전값 0
            //Destroy(this.gameObject);//사망
        }
    }
    void ReturnSprite()
    {
        spriteRenderer.sprite = sprites[0];//원래 이미지로
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "WallBullet" && enemyName != "B")//벽에 맞음, 보스가 아닐 때
        {
            this.gameObject.SetActive(false);//사망
            transform.rotation = Quaternion.identity;//기본 회전값 0
            //Destroy(this.gameObject);
        }
        else if(collision.gameObject.tag == "PlayerBullet")
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            OnHit(bullet.dmg);

            collision.gameObject.SetActive(false);//총알이 안보이게
            //Destroy(collision.gameObject);//총알도 삭제
        }
    }
}
