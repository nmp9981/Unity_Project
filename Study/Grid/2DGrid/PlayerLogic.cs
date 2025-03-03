using Microsoft.Office.Interop.Excel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{
    [SerializeField]
    Board board;

    bool isMoveFinish = true;
    float currentTime = 0f;
    float moveCoolTime = 0.02f;

    public const float upsilon = 0.01f;
    Vector3 moveVec;
    Vector3 targetVec;

    private void Start()
    {
        PlayerInit();
    }

    private void Update()
    {
        KeyInput();
    }
    /// <summary>
    /// 플레이어 위치 초기화
    /// </summary>
    void PlayerInit()
    {
        this.gameObject.transform.position = board.startTile.transform.position;
    }
    /// <summary>
    /// 플레이어 이동
    /// </summary>
    void MovePlayer(Vector3 moveVec, Vector3 targetVec)
    {
        //범위 제한
        if (LimitPlayerArea(targetVec))
        {
            isMoveFinish = true;
            return;
        }
        //목표 도달
        if( (targetVec - transform.position).sqrMagnitude<upsilon)
        {
            transform.position = targetVec;
            isMoveFinish = true;
            return;
        }

        transform.position = Vector3.SmoothDamp(transform.position, targetVec, ref moveVec, moveCoolTime);
    }
    /// <summary>
    /// 플레이어 범위 제한
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    bool LimitPlayerArea(Vector3 pos)
    {
        if (board.width <= pos.x || 0 > pos.x)
        {
            return true;
        }
        if (board.height <= pos.y || 0 > pos.y)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 키 입력
    /// </summary>
    void KeyInput()
    {
        //이동 버튼이 눌러져 잇음
        if (Board.isPressMoveButton)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            moveVec = Vector3.left;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            moveVec = Vector3.down;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            moveVec = Vector3.right;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            moveVec = Vector3.up;
        }

        //목표 위치 선정
        if (moveVec != Vector3.zero && isMoveFinish)
        {
            targetVec = transform.position + moveVec;
            isMoveFinish = false;
        }
        //이동중이 아닐때만
        if (!isMoveFinish)
        {
            MovePlayer(moveVec, targetVec);
        }
        moveVec = Vector3.zero;
    }
   
    /// <summary>
    /// 시간 흐름
    /// </summary>
    void FlowTime()
    {
        currentTime += Time.deltaTime;
    }
}
