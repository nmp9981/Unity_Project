using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    int curJumpCount = 0;
    float rotateSpeed = 15f;
    Rigidbody rigid = null;

    float distance;
    [SerializeField] LayerMask layerMask = 0;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }
    void Update()
    {
        Move();
        TryJump();
        CheckGround();
    }
    private void Move()
    {
        float hAxis = Input.GetAxisRaw("Horizontal");
        float vAxis = Input.GetAxisRaw("Vertical");

        Vector3 moveVec = new Vector3(hAxis, 0, vAxis).normalized;//이동 방향, 정규화(대각선에서 더 빨라지는거 방지)

        if(hAxis!=0 || vAxis != 0)
        {
            transform.position += moveVec * GameManager.Instance.PlayerMoveSpeed * Time.deltaTime;//좌표 이동
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveVec), Time.deltaTime *rotateSpeed);//캐릭터가 바라보는 방향으로 회전
        }
    }
    void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (curJumpCount < GameManager.Instance.MaxJumpCount)//점프 횟수 남음
            {
                curJumpCount++;
                rigid.velocity = Vector3.up * GameManager.Instance.PlayerJumpSpeed;
            }
        }
    }
    void CheckGround()
    {
        distance = GetComponent<BoxCollider>().size.y * 0.6f;
        if (rigid.velocity.y < 0)//낙하할때만
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, Vector3.down, out hit, distance, layerMask))//레이를 쏜다
            {
                if (hit.transform.CompareTag("Ground"))//발밑에 땅이 닿으면 점프 초기화
                {
                    curJumpCount = 0;
                }
            }
        }
    }
}
