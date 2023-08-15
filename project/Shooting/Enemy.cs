using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    GameManager gameManager;
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

    public int PatternIndex;//패턴 번호
    public int curPatternCount;//패턴 횟수
    public int[] maxPatternCount;//최대 패턴 횟수

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameManager = GetComponent<GameManager>();

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
                Invoke("StopBoss", 2);
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
        if (!gameObject.activeSelf) return;//활성화일때만 밑 로직 적용

        Rigidbody2D rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = Vector2.zero;

        Invoke("Think", 2);
    }

    //보스 패턴
    void Think()
    {
        PatternIndex = PatternIndex == 3 ? 0 : PatternIndex+1;//패턴 인덱스 늘리기
        curPatternCount = 0;//패턴 변경시 실행횟수 초기화
        
        switch (PatternIndex)
        {
            case 0:
                FireFoward();
                break;
            case 1:
                FireShot();
                break;
            case 2:
                FireArc();
                break;
            case 3:
                FireAround();
                break;
        }
    }
    //각 보스 패턴
    void FireFoward()
    {
        //4발쏘기
        GameObject bullet1 = objectManager.MakeObj("BulletBossA");//총알 생성
        bullet1.transform.position = transform.position + Vector3.left * 0.4f;
        GameObject bullet2 = objectManager.MakeObj("BulletBossA");//총알 생성
        bullet2.transform.position = transform.position + Vector3.left * 0.6f;
        GameObject bullet3 = objectManager.MakeObj("BulletBossA");//총알 생성
        bullet3.transform.position = transform.position + Vector3.right * 0.4f;
        GameObject bullet4 = objectManager.MakeObj("BulletBossA");//총알 생성
        bullet4.transform.position = transform.position + Vector3.right * 0.6f;

        Rigidbody2D rigid1 = bullet1.GetComponent<Rigidbody2D>();
        Rigidbody2D rigid2 = bullet2.GetComponent<Rigidbody2D>();
        Rigidbody2D rigid3 = bullet3.GetComponent<Rigidbody2D>();
        Rigidbody2D rigid4 = bullet4.GetComponent<Rigidbody2D>();

        rigid1.AddForce(Vector2.down * 5.0f, ForceMode2D.Impulse);//단위벡터로 (안그러면 크기 1을 넘어가 빠르게 쏘짐)
        rigid2.AddForce(Vector2.down * 5.0f, ForceMode2D.Impulse);
        rigid3.AddForce(Vector2.down * 5.0f, ForceMode2D.Impulse);
        rigid4.AddForce(Vector2.down * 5.0f, ForceMode2D.Impulse);

        //카운터 증가
        curPatternCount++;

        if(curPatternCount < maxPatternCount[PatternIndex])
        {
            Invoke("FireFoward", 2);
        }else Invoke("Think", 3);
    }
    void FireShot()
    {
        //5발 공격
        for(int i = 0; i < 5; i++)
        {
            GameObject bullet = objectManager.MakeObj("BulletBossB");
            bullet.transform.position = transform.position;//적의 위치가 시작위치

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector2 dirVec = player.transform.position-transform.position;//캐릭터 향한 방향
            Vector2 ranVec = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(0f, 2.0f));//랜덤 벡터
            dirVec += ranVec;
            rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);

        }
        curPatternCount++;

        if (curPatternCount < maxPatternCount[PatternIndex])
        {
            Invoke("FireShot", 3.5f);
        }
        else Invoke("Think", 3);
    }
    void FireArc()
    {
        //부채꼴 공격
        GameObject bullet = objectManager.MakeObj("BulletBossA");
        bullet.transform.position = transform.position;//적의 위치가 시작위치
        bullet.transform.rotation = Quaternion.identity;//회전 초기화

        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        Vector2 dirVec = new Vector2(Mathf.Sin(10.0f*Mathf.PI*curPatternCount/maxPatternCount[PatternIndex]),-1.0f);
        rigid.AddForce(dirVec.normalized * 5, ForceMode2D.Impulse);

        curPatternCount++;

        if (curPatternCount < maxPatternCount[PatternIndex])
        {
            Invoke("FireArc", 0.15f);
        }
        else Invoke("Think", 3);
    }
    void FireAround()
    {
        //원 공격
        int roundNumA = 30;
        int roundNumB = 20;
        int roundNum = curPatternCount % 2 == 0 ? roundNumA : roundNumB;//랜덤 횟수
        for(int i = 0; i < roundNum; i++)
        {
            GameObject bullet = objectManager.MakeObj("BulletBossA");
            bullet.transform.position = this.gameObject.transform.position;//적의 위치가 시작위치
            bullet.transform.rotation = Quaternion.identity;//회전 초기화

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector2 dirVec = new Vector2(Mathf.Cos(2.0f * Mathf.PI * i/roundNum), Mathf.Sin(2.0f * Mathf.PI * i / roundNum));
            rigid.AddForce(dirVec.normalized * 2, ForceMode2D.Impulse);

            //날아가는 방향으로 방향 바꾸기
            Vector3 rotVec = Vector3.forward * 360 * i / roundNum+Vector3.forward*90;
            bullet.transform.Rotate(rotVec);
        }

        curPatternCount++;

        if (curPatternCount < maxPatternCount[PatternIndex])
        {
            Invoke("FireAround", 0.7f);
        }
        else Invoke("Think", 3);
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
        Debug.Log(health);
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
            
            //Boss가 죽음 -> 스테이지 종료
            if(enemyName == "B") gameManager.StageEnd();
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
