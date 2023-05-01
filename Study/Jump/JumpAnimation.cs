using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator animator;
    SpriteRenderer rend;
    public Animation anim;
    [SerializeField]
    private bool isJumping;
    [SerializeField]
    private float speed = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        anim = gameObject.GetComponent<Animation>();
        rend = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Left();//왼쪽
        Right();//오른쪽
        Jump();//점프

        this.animator.speed = speed / 2.0f;//플레이어의 속도에 맞춰 재생
    }
    void Left()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rend.flipX = true;
            this.gameObject.transform.Translate(-speed * Time.deltaTime, 0, 0);
        }
    }
    void Right()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            rend.flipX = false;
            this.gameObject.transform.Translate(speed * Time.deltaTime, 0, 0);
        }
    }
    void Jump()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if(isJumping == false)//점프중이 아니라면
            {
                isJumping = true;
                this.animator.speed = 1.0f;//애니메이션 속도 조절
                //anim.CrossFade("Jumps");//명칭, 다른 FadeOut으로 바뀌는 시간
                this.animator.SetTrigger("JumpTrigger");
                rigid.AddForce(new Vector2(0, 8), ForceMode2D.Impulse);//y축 방향으로 힘을 준다.
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Land")//땅에 닿으면
        {
            isJumping = false;//다시 점프 가능
        }
    }
}
