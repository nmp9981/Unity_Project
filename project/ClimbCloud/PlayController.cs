using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerController : MonoBehaviour
{
    Rigidbody2D rigid2D;
    Animator animator;
    float jumpForce = 680.0f;
    float walkForce = 30.0f;
    float maxWalkSpeed = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        this.rigid2D = GetComponent<Rigidbody2D>();
        this.animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //점프한다
        if (Input.GetKeyDown(KeyCode.Space) && this.rigid2D.velocity.y==0)//y축 속도가 0인가?
        {
            this.animator.SetTrigger("JumpTrigger");//점프트리거로 전환
            this.rigid2D.AddForce(transform.up * this.jumpForce);//jumpForce만큼 위로 힘을 가해줌
        }
        //좌우이동
        int key = 0;//입력 없을 때
        if (Input.GetKey(KeyCode.RightArrow)) key = 1;
        if (Input.GetKey(KeyCode.LeftArrow)) key = -1;

        //속도
        float speedx = Mathf.Abs(this.rigid2D.velocity.x);

        //스피드 제한
        if (speedx < this.maxWalkSpeed)
        {
            this.rigid2D.AddForce(transform.right * key * this.walkForce);//엑셀 밟기
        }
        //방향에 따른 반전
        if (key != 0)
        {
            transform.localScale = new Vector3(key, 1, 1);//x축 반전
        }
        //플레이어 속도에 맞춰 애니메이션 속도를 바꾼다.
        if (this.rigid2D.velocity.y == 0)//걷기
        {
            this.animator.speed = speedx / 2.0f;
        }
        else//점프 중
        {
            this.animator.speed = 1.0f;
        }

        //화면 밖으로 나감
        if (transform.position.y < -10)
        {
            SceneManager.LoadScene("GameScene");//처음부터, 현재 씬 로드시 다시 처음부터
        }
    }
    //골 도착
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("골");
        SceneManager.LoadScene("ClearScene");
    }
}
