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
    bool isDodge;

    Vector3 moveVec;
    Vector3 dodgeVec;//회피중 움직이지 않게

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
        Dodge();//회피
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

        if (isDodge) moveVec = dodgeVec;//회피중일때는 회피방향으로

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
        //멈춤상태일때 점프
        if (jDown && !isJump && moveVec==Vector3.zero && !isDodge)//점프키 누르고 점프 상태가 아닐때
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
    //회피
    void Dodge()
    {
        //움직인 상태일때
        if (jDown && !isJump && moveVec!=Vector3.zero && !isDodge)//점프키 누르고 점프 상태가 아닐때
        {
            dodgeVec = moveVec;
            speed *= 2;//회피는 이동속도가 2배
            anim.SetTrigger("doDodge");
            isDodge = true;

            //회피중 점프 불가
            Invoke("DodgeOut",0.5f);//회피 상태 종료
        }
    }
    //회피상태 종료
    void DodgeOut()
    {
        speed *= 0.5f;//원래 속도
        isDodge = false;
    }
}
