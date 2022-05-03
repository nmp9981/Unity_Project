using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slime : MonoBehaviour
{
    private GameObject slime;
    private Rigidbody2D rigid;
    public Animator anim;

    public bool landing = true;
    //public GameObject enemy;
    public LayerMask LayerMask;
    Enemy enemy;

    // Start is called before the first frame update
    void Start()
    {
        slime = GameObject.Find("Slime");
        rigid = slime.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        move4();
        jump();
        attack();
    }
    void jump()
    {
        if (Input.GetKeyDown("w"))
        {
            if(landing == true)
            {
                slime.GetComponent<Rigidbody2D>().AddForce(transform.up * 5, ForceMode2D.Impulse);
                landing = false;
            }
        }
    }
    void move4()
    {
        float horizontalVec = rigid.velocity.y;//y축 위치
        if (Input.GetKey("a"))
        {
            rigid.velocity = new Vector2(-3.0f, horizontalVec);
            //anim.SetBool("ismove", true);
        }else if (Input.GetKey("d"))
        {
            rigid.velocity = new Vector2(3.0f, horizontalVec);
            //anim.SetBool("ismove", true);
        }
        else
        {
            rigid.velocity = new Vector2(0f, horizontalVec);
            //anim.SetBool("ismove", false);
        }
    }
    void attack()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //anim.SetBool("is_attack", true);
            //발사 시작점, 방향, 사정거리, 판별식(목표물)
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector2.right, 15f, LayerMask.GetMask("Enemy"));//오른쪽 방향 공격
            if(rayHit.collider != null)//충돌하면
            {
                enemy = rayHit.collider.gameObject.GetComponent<Enemy>();//맞은 적 지정
                enemy.get_damage(10);
            }
        }else if (Input.GetKeyUp(KeyCode.Space))
        {
            //anim.SetBool("ismove", false);
        }
    }

    //충돌시
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")){
            landing = true;
        }   
    }
    //충돌을 안할때
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            landing = false;//공중에서 점프를 못하게, 땅에 닿았을때만 점프
        }
    }
}
