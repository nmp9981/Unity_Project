using Microsoft.Office.Interop.Excel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpMap {
    public class PlayerMove : MonoBehaviour
    {
        [SerializeField] float jumpForce;//점프력
        [SerializeField] int maxJumpCount=3;//점프 최대 횟수
        [SerializeField] LayerMask layerMask = 0;

        int jumpCount = 0;

        Rigidbody2D rigid = null;

        float distance;
        float speed = 5f;

        void Start()
        {
            rigid = GetComponent<Rigidbody2D>();
            distance = GetComponent<BoxCollider2D>().bounds.extents.y + 0.05f;//플레이어 발밑까지만 레이를 쏜다.
        }

        private void Update()
        {
            Move();
            TryJump();
            CheckGround();

        }
        private void Move()
        {
            //입력(-1~1 반환)
            float hAxis = Input.GetAxisRaw("Horizontal");

            Vector3 moveVec = new Vector3(hAxis, 0, 0).normalized;//이동 방향, 정규화(대각선에서 더 빨라지는거 방지)
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
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, distance, layerMask);

                if(hit.collider != null)//충돌체 존재
                {
                    if (hit.collider.gameObject.CompareTag("Ground"))//발밑에 땅이 닿으면 점프 초기화
                    {
                        jumpCount = 0;
                    }
                }
               
            }

        }
    }
}
