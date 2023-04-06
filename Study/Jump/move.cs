using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    Rigidbody2D rigid;
    [SerializeField]
    bool isJumping;
    // Start is called before the first frame update
    void Start()
    {
        rigid = this.gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Left();//왼쪽
        Right();//오른쪽
        Jump();//점프
    }
    void Left()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            this.gameObject.transform.Translate(-2*Time.deltaTime, 0, 0);
        }
    }
    void Right()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            this.gameObject.transform.Translate(2*Time.deltaTime, 0, 0);
        }
    }
    //스페이스 키로 점프 구현
    void Jump()
    {
        if (Input.GetKey(KeyCode.Space))//스페이스 키를 누르고
        {
            if (isJumping == false)//점프상태가 아니라면
            {
                isJumping = true;//점프중으로 바꾼다.
                rigid.AddForce(new Vector2(0, 8), ForceMode2D.Impulse);//y축 방향으로 힘을 실어준다.(점프)
            }
        }
    }
    //충돌 처리
    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Land")//땅에 닿으면
        {
            isJumping = false;//다시 점프를 할 수 있다.
        }
    }
    
}
