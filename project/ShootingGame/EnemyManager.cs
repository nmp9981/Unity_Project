using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    ObjectManager objectManager;
    UIManager uiManager;
    DamageText damageText;

    public int health;
    public int attack;
    public string enemyName;
    public Sprite[] spriteImage;

    public GameObject bullet1;

    float enemySpeed = 1.7f;
    float coolTime = 2.0f;
    float curTime;
    float shootSpeed = 2.5f;

    SpriteRenderer spriteRenderer;//이미지 변경
    // Start is called before the first frame update
    void Awake()
    {
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        damageText = GameObject.Find("DamageText").GetComponent<DamageText>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        objectManager = GameObject.Find("ObjectManager").GetComponent<ObjectManager>();
        
    }
    
    //컴포넌트 활성화할때 호출
    private void OnEnable()
    {
        switch (enemyName)
        {
            case "EnemyA":
                health = 4;
                attack = 1;
                break;
            case "EnemyB":
                health = 7;
                attack = 2;
                break;
            case "EnemyC":
                health = 13;
                attack = 4;
                break;
            case "EnemyD":
                health = 22;
                attack = 6;
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
            case "EnemyB":
                GameObject bullet2 = objectManager.MakeGameObject("EnemyBulletA");//총알 소환
                bullet2.transform.position = this.gameObject.transform.position;

                Rigidbody2D rigid2 = bullet2.GetComponent<Rigidbody2D>();
                rigid2.AddForce(Vector3.down * (shootSpeed+0.5f), ForceMode2D.Impulse);//앞으로 쏘기
                break;
            case "EnemyC":
                GameObject bullet3 = objectManager.MakeGameObject("EnemyBulletB");//총알 소환
                bullet3.transform.position = this.gameObject.transform.position;

                Rigidbody2D rigid3 = bullet3.GetComponent<Rigidbody2D>();
                rigid3.AddForce(Vector3.down * (shootSpeed + 0.3f), ForceMode2D.Impulse);//앞으로 쏘기
                break;
            case "EnemyD":
                GameObject bullet4 = objectManager.MakeGameObject("EnemyBulletB");//총알 소환
                bullet4.transform.position = this.gameObject.transform.position;

                Rigidbody2D rigid4 = bullet4.GetComponent<Rigidbody2D>();
                rigid4.AddForce(Vector3.down * (shootSpeed + 0.4f), ForceMode2D.Impulse);//앞으로 쏘기
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
        if (health <= 0)//사망 처리
        {
            uiManager.getScore(enemyName);
            DropItem();//아이템 드랍
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
        else if(collision.tag == "PlayerBullet")//캐릭터 총알한테 맞음
        {
            OnHit(GamaManager.Instance.startAttack);
            collision.gameObject.SetActive(false);//총알이 사라짐
        }
    }
    //드랍 아이템
    void DropItem()
    {
        //드랍울 설정
        int dropProb = Random.Range(0, 100);
        
        if(dropProb >= 84)
        {
            GameObject powerUp = objectManager.MakeGameObject("PowerItem");//파워 업
            powerUp.transform.position = this.gameObject.transform.position;

            Rigidbody2D rigid = powerUp.GetComponent<Rigidbody2D>();
            rigid.AddForce(Vector3.down * shootSpeed, ForceMode2D.Impulse);//밑으로 떨어짐
        }else if(dropProb >= 67)
        {
            GameObject posion = objectManager.MakeGameObject("PosionItem");//회복
            posion.transform.position = this.gameObject.transform.position;

            Rigidbody2D rigid = posion.GetComponent<Rigidbody2D>();
            rigid.AddForce(Vector3.down * shootSpeed, ForceMode2D.Impulse);//밑으로 떨어짐
        }
        else
        {
            GameObject coin = objectManager.MakeGameObject("CoinItem");//돈
            coin.transform.position = this.gameObject.transform.position;

            Rigidbody2D rigid = coin.GetComponent<Rigidbody2D>();
            rigid.AddForce(Vector3.down * shootSpeed, ForceMode2D.Impulse);//밑으로 떨어짐
        }
    }

}
