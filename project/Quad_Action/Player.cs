using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    float hAxis;
    float vAxis;
    bool wDown;
    bool jDown;
    bool isJump;

    Vector3 moveVec;

    Animator anim;
    Rigidbody rigid;
    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        //자식오브젝트의 컴포넌트 가져옴
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();//입력
        Move();//이동
        Turn();//회전
        Jump();//점프
    }

    //입력
    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");//단 1회 입력
    }
    //이동
    private void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;//이동 방향, 정규화(대각선에서 더 빨라지는거 방지)

        transform.position += moveVec * speed * (wDown ? 0.3f : 1.0f) * Time.deltaTime;//좌표 이동

        //이동 애니메이션 적용하기
        anim.SetBool("isRun", moveVec != Vector3.zero);//멈춤만 아니면 기본 달리기
        anim.SetBool("isWalk", wDown);
    }
    //회전
    void Turn()
    {
        //회전하기
        //지정된 방향을 향해 회전, 우리가 갈 방향으로 회전
        transform.LookAt(this.transform.position + moveVec);
    }
    //점프
    void Jump()
    {
        if (jDown && !isJump)//점프키 누르고 점프 상태가 아닐때
        {
            rigid.AddForce(Vector3.up * 25, ForceMode.Impulse);//즉시점프
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
        }
    }

    //플레이어와 충돌시
    private void OnCollisionEnter(Collision collision)
    {
        //바닥에 닿았을 떄
        if(collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump", false);//점프 완료
            isJump = false;
        }
    }
}
