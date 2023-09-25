using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    float hAxis;
    float vAxis;
    bool wDown;

    Vector3 moveVec;

    Animator anim;
    // Start is called before the first frame update
    void Awake()
    {
        //자식오브젝트의 컴포넌트 가져옴
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");

        moveVec = new Vector3(hAxis,0,vAxis).normalized;//이동 방향, 정규화(대각선에서 더 빨라지는거 방지)
        
        transform.position += moveVec*speed*(wDown?0.3f:1.0f)*Time.deltaTime;//좌표 이동

        //이동 애니메이션 적용하기
        anim.SetBool("isRun",moveVec!=Vector3.zero);//멈춤만 아니면 기본 달리기
        anim.SetBool("isWalk", wDown);

        //회전하기
        //지정된 방향을 향해 회전, 우리가 갈 방향으로 회전
        transform.LookAt(this.transform.position+moveVec);
    }
}
