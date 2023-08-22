using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    [SerializeField] Animator moveAnim;

    bool isleftColid, isrightColid, isupColid, isdownColid;//각 방향 충돌여부
    // Start is called before the first frame update
    void Awake()
    {
        moveAnim = GetComponent<Animator>();
        this.transform.position = new Vector2(0, -3);
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
    }
    private void MovePlayer()
    {
        float h = Input.GetAxisRaw("Horizontal");//좌우 즉시 이동
        if ((h == 1 && isrightColid) || (h == -1 && isleftColid)) h = 0;//이동 범위 초과
        float v= Input.GetAxisRaw("Vertical");//상하 즉시 이동
        if ((v == 1 && isupColid) || (v == -1 && isdownColid)) v = 0;//이동 범위 초과

        Vector2 CurPos = transform.position;
        Vector2 deltaPos = new Vector2(h, v) * GamaManager.Instance.PlayerSpeed * Time.deltaTime;
        this.transform.position = CurPos + deltaPos;//최종 위치

        if(Input.GetButtonUp("Horizontal") || Input.GetButtonDown("Horizontal"))
        {
            moveAnim.SetInteger("Move", (int)h);
        }
    }
    //충돌 영역에 들어옴
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //이동 가능 범위 초과
        if(collision.tag == "Wall")
        {
            switch (collision.name)
            {
                case "RightWall":
                    isrightColid = true;
                    break;
                case "LeftWall":
                    isleftColid = true;
                    break;
                case "TopWall":
                    isupColid = true;
                    break;
                case "BottomWall":
                    isdownColid = true;
                    break;
            }
        }
    }

    //충돌 영역을 나감
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Wall")
        {
            switch (collision.name)
            {
                case "RightWall":
                    isrightColid = false;
                    break;
                case "LeftWall":
                    isleftColid = false;
                    break;
                case "TopWall":
                    isupColid = false;
                    break;
                case "BottomWall":
                    isdownColid = false;
                    break;
            }
        }
    }
}
