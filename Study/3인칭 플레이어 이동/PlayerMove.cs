using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private float moveSpeed = 5f;//이동 속도
   
    //이동 방향
    float hAxis;
    float vAxis;
    private float yMoveAmount;

    void Update()
    {
        MoveInput();
        Move();    
    }
    //키보드에 따른 이동
    void MoveInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(KeyCode.Q)) yMoveAmount = 1.0f;
        else if (Input.GetKey(KeyCode.E)) yMoveAmount = -1.0f;
        else yMoveAmount = 0.0f;
    }
    //오브젝트가 바라보는 방향으로 키보드 이동
    //ex, 오브젝트가 북서쪽을 향하면 위키 눌렀을 때 북서쪽으로 이동해야함
    private void Move()
    {
        Vector3 moveVec = this.gameObject.transform.right * hAxis + this.gameObject.transform.forward * vAxis+ this.gameObject.transform.up*yMoveAmount;
        this.gameObject.transform.position += moveVec * moveSpeed * Time.deltaTime;//좌표 이동
    }
}
