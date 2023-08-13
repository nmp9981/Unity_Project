using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int life;
    public int score;

    public float speed;
    public int power;
    public int maxPower;
    public int boom;
    public int maxBoom;
    //충돌 검사
    public bool isTouchTop;
    public bool isTouchBottom;
    public bool isTouchLeft;
    public bool isTouchRight;

    public GameObject bulletObjA;
    public GameObject bulletObjB;
    public GameObject boomEffect;

    public float maxShotDelay;//실제 쿨타임
    public float curShotDelay;//경과 시간

    public GameManager manager;
    public ObjectManager objectManager;
    public bool isHit;
    public bool isBoomTime;

    public GameObject[] followers;
    public bool isRespawn;//리스폰 여부

    Animator anim;
    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        life = 3;
        score = 0;
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    //생성시 나오는 함수
    private void OnEnable()
    {
        Unbeatable();//투명하게
        Invoke("Unbeatable", 3);//3초뒤 불투명
    }
    void Unbeatable()
    {
        isRespawn = !isRespawn;
        if (isRespawn)//리스폰때는 약간 투명하게(보조무기까지)
        {
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);
            for (int i = 0; i < followers.Length; i++)
            {
                followers[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
            }
        }
        else
        {
            spriteRenderer.color = new Color(1, 1, 1, 1f);
            for (int i = 0; i < followers.Length; i++)
            {
                followers[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        Move();//이동
        Fire();//총쏘기
        Boom();//폭탄
        Reload();//장전
    }
    //이동
    void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");//가로, 즉시 이동하므로 Raw를 붙여야함
        if ((h == 1 && isTouchRight) || (h == -1 && isTouchLeft))
        {
            h = 0;
        }
        float v = Input.GetAxis("Vertical");//세로
        if ((v == 1 && isTouchTop) || (v == -1 && isTouchBottom))
        {
            v = 0;
        }

        Vector3 curPos = transform.position;
        Vector3 nextPos = new Vector3(h, v, 0) * speed * Time.deltaTime;//트랜스폼 이동

        transform.position = curPos + nextPos;//최종 위치

        //좌우 방향 애니메이션
        if(Input.GetButtonDown("Horizontal") || Input.GetButtonUp("Horizontal"))
        {
            anim.SetInteger("Input", (int) h);
        }
    }
    void Fire()
    {
        if (!Input.GetButton("Fire1")) return;//누르지 않았을 경우 실행 X, 마우스 좌클릭
        if (curShotDelay < maxShotDelay) return;//쿨타임이 안됨

        switch (power)
        {
            case 1://power one
                GameObject bullet = objectManager.MakeObj("BulletPlayerA");
                bullet.transform.position = transform.position;

                //GameObject bullet = Instantiate(bulletObjA, this.transform.position, this.transform.rotation);//총알 생성
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            case 2://power two, 총알 2개 생성(조금씩 다른 위치에)
                GameObject bulletL = objectManager.MakeObj("BulletPlayerA");
                bulletL.transform.position = transform.position + Vector3.left * 0.1f;
                GameObject bulletR = objectManager.MakeObj("BulletPlayerA");
                bulletR.transform.position = transform.position + Vector3.right * 0.1f;
                
                //GameObject bulletL = Instantiate(bulletObjA, this.transform.position+Vector3.left*0.1f, this.transform.rotation);//총알 생성
                //GameObject bulletR = Instantiate(bulletObjA, this.transform.position+Vector3.right*0.1f, this.transform.rotation);//총알 생성
                Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
                rigidL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            case 3:
            case 4:
            case 5:
            case 6://power three, 총알 3개(가운데는 큰것)
                GameObject bulletLL = objectManager.MakeObj("BulletPlayerA");
                bulletLL.transform.position = transform.position + Vector3.left * 0.25f;
                GameObject bulletCC = objectManager.MakeObj("BulletPlayerB");
                bulletCC.transform.position = transform.position;
                GameObject bulletRR = objectManager.MakeObj("BulletPlayerA");
                bulletRR.transform.position = transform.position + Vector3.right * 0.25f;

                Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidCC = bulletCC.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();
                rigidLL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidCC.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidRR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
        }
        curShotDelay = 0;//쿨타임 초기화
    }
    //장전
    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }
    void Boom()
    {
        if (!Input.GetButton("Fire2")) return;//우클릭
        if (isBoomTime) return;//실행중이면
        if (boom == 0) return;//폭탄 개수

        isBoomTime = true;
        boom--;
        manager.UpdateBoomIcon(boom);//남은 폭탄
        boomEffect.SetActive(true);//이펙트 켜기

        //범위 내 적 모두 제거
        GameObject[] enemisL = objectManager.GetPool("EnemyL");
        GameObject[] enemisM = objectManager.GetPool("EnemyM");
        GameObject[] enemisS = objectManager.GetPool("EnemyS");
        //GameObject[] enemis = GameObject.FindGameObjectsWithTag("Enemy");//적 태그는 모두 모음
        /*
        for (int idx = 0; idx < enemis.Length; idx++)
        {
            Enemy enemyLogic = enemis[idx].GetComponent<Enemy>();
            enemyLogic.OnHit(1000);
        }
        */
        for (int idx = 0; idx < enemisL.Length; idx++)
        {
            if (enemisL[idx].activeSelf)//활성화 된것만
            {
                Enemy enemyLogic = enemisL[idx].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);
            }
        }
        for (int idx = 0; idx < enemisM.Length; idx++)
        {
            if (enemisM[idx].activeSelf)//활성화 된것만
            {
                Enemy enemyLogic = enemisM[idx].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);
            }
        }
        for (int idx = 0; idx < enemisS.Length; idx++)
        {
            if (enemisS[idx].activeSelf)//활성화 된것만
            {
                Enemy enemyLogic = enemisS[idx].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);
            }
        }

        //총알도 제거
        /*
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("EnemyBullet");//적 총알 태그는 모두 모음
        for (int idx = 0; idx < bullets.Length; idx++)
        {
            Destroy(bullets[idx]);
        }
        */
        //풀 내에 있는것만 관리
        GameObject[] bulletA = objectManager.GetPool("BulletEnemyA");
        GameObject[] bulletB = objectManager.GetPool("BulletEnemyB");
        for (int idx = 0; idx < bulletA.Length; idx++)
        {
            if (bulletA[idx].activeSelf)//활성화 된것만
            {
                bulletA[idx].SetActive(false);
            }
        }
        for (int idx = 0; idx < bulletB.Length; idx++)
        {
            if (bulletB[idx].activeSelf)//활성화 된것만
            {
                bulletB[idx].SetActive(false);
            }
        }

        Invoke("OffBoomEffect", 4f);//이펙트 끄기
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Wall")
        {
            switch (collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = true;
                    break;
                case "Bottom":
                    isTouchBottom = true;
                    break;
                case "Left":
                    isTouchLeft = true;
                    break;
                case "Right":
                    isTouchRight = true;
                    break;
            }
        }else if(collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBullet")
        {
            if (isRespawn) return;//리스폰 타임일때는 안함

            if (isHit) return;

            isHit = true;
            //피격
            life--;
            manager.UpdateLifeIcon(life);//남은 HP

            if (life == 0) manager.GameOver();//게임 오버
            else manager.RespawnPlayer();//부활

            gameObject.SetActive(false);
            collision.gameObject.SetActive(false);
            //Destroy(collision.gameObject);
        }else if (collision.gameObject.tag == "Item")
        {
            Item item = collision.gameObject.GetComponent<Item>();
            switch (item.type)
            {
                case "Coin":
                    score += 1;
                    break;
                case "Power":
                    if (power == maxPower) score += 3;//최대 파워
                    else
                    {
                        power++;
                        AddFollower();//보조무기 추가
                    }
                    break;
                case "Boom":
                    if (boom == maxBoom) return;//최대 폭탄 개수
                    else
                    {
                        boom++;
                        manager.UpdateBoomIcon(boom);//남은 폭탄 업데이트
                    }
                    break;
            }
            collision.gameObject.SetActive(false);
            //Destroy(collision.gameObject);
        }
    }
    void AddFollower()
    {
        if (power == 4)
        {
            followers[0].SetActive(true);
        }else if (power == 5)
        {
            followers[1].SetActive(true);
        }
        else if (power == 6)
        {
            followers[2].SetActive(true);
        }
    }
    void OffBoomEffect()
    {
        boomEffect.SetActive(false);
        isBoomTime = false;
    }
    //충돌영역 빠져나감
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            switch (collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = false;
                    break;
                case "Bottom":
                    isTouchBottom = false;
                    break;
                case "Left":
                    isTouchLeft = false;
                    break;
                case "Right":
                    isTouchRight = false;
                    break;
            }
        }
    }
}
