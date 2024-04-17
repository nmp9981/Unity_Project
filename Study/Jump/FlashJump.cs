using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{
    [SerializeField] float jumpForce;//점프력
    [SerializeField] int maxJumpCount;//점프 최대 횟수
    [SerializeField] float speed;
    int jumpCount = 0;

    Rigidbody rigid = null;

    float distance;
    [SerializeField] LayerMask layerMask = 0;
    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        distance = GetComponent<BoxCollider>().bounds.extents.y+0.05f;//플레이어 발밑까지만 레이를 쏜다.
    }
    private void Move()
    {
        //입력(-1~1 반환)
        float hAxis = Input.GetAxisRaw("Horizontal");
        float vAxis = Input.GetAxisRaw("Vertical");

        Vector3 moveVec = new Vector3(hAxis, 0, vAxis).normalized;//이동 방향, 정규화(대각선에서 더 빨라지는거 방지)
        transform.position += moveVec * speed * Time.deltaTime;//좌표 이동
    }
    void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (jumpCount < maxJumpCount)//점프 횟수 남음
            {
                jumpCount++;
                rigid.velocity = Vector3.up * jumpForce;
            }
        }
    }
    void CheckGround()
    {
        if (rigid.velocity.y < 0)//낙하할때만
        {
            RaycastHit hit;
            
            if (Physics.Raycast(transform.position,  Vector3.down, out hit, distance, layerMask))//레이를 쏜다
            {
                if (hit.transform.CompareTag("Ground"))//발밑에 땅이 닿으면 점프 초기화
                {
                    jumpCount = 0;
                }
            }
        }
        
    }
    // Update is called once per frame
    void Update()
    {
        Move();
        TryJump();
        CheckGround();
    }
}
