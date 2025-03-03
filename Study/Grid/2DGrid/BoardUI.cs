using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class BoardUI : MonoBehaviour
{
    [SerializeField]
    PlayerLogic playerLogic;

    [SerializeField]
    TextMeshProUGUI positionText;

    float moveCoolTime = 0.1f;
    Vector3 moveVec;

    //버튼 눌러서 이동중인가?
    public bool isButtonMoving { get; set; }

    private void Awake()
    {
        isButtonMoving = false;
    }

    private void Update()
    {
        ShowPositionText();
    }

    void ShowPositionText()
    {
        Vector2 pos = new Vector2(Mathf.Round(playerLogic.transform.position.x), Mathf.Round(playerLogic.transform.position.y));
        positionText.text = $"Position\n({pos.x}, {pos.y})";
    }

    /// <summary>
    /// 목표 지점까지 이동
    /// </summary>
    public void MoveToGoal()
    {
        //클릭한 위치 존재
        if (Board.buttonTargetPos == null)
        {
            return;
        }

        //클릭 위치와 현재 위치 비교
        if((Board.buttonTargetPos - playerLogic.gameObject.transform.position).sqrMagnitude < PlayerLogic.upsilon)
        {
            return;
        }


        //목표까지 스무스하게 이동
        StartCoroutine(MoveToTaget(playerLogic.gameObject.transform.position, Board.buttonTargetPos));
    }

    /// <summary>
    /// 클릭한 곳으로 스무스하게 이동
    /// </summary>
    /// <param name="start">시작 지점</param>
    /// <param name="end">끝 지점</param>
    /// <returns></returns>
    IEnumerator MoveToTaget(Vector3 start, Vector3 end)
    {
        Board.isPressMoveButton = true;

        while (true)
        {
            //목표 도달
            if ((playerLogic.gameObject.transform.position - end).sqrMagnitude < PlayerLogic.upsilon)
            {
                playerLogic.gameObject.transform.position = end;
                Board.isPressMoveButton = false;
                yield break;
            }

            //x축 부터 이동
            if (Mathf.Abs(playerLogic.gameObject.transform.position.x - end.x) < PlayerLogic.upsilon)//x축 이동 완료
            {
                //방향 전환
                if (moveVec == Vector3.right || moveVec == Vector3.left)
                {
                    playerLogic.gameObject.transform.position = new Vector3(end.x, start.y, 0);
                }
                
                //위 or 아래 이동방향 결정
                moveVec = (start.y < end.y)?Vector3.up:Vector3.down;
                playerLogic.gameObject.transform.position += moveVec * moveCoolTime;
            }
            else
            {
                //좌 or 우 이동방향 결정
                moveVec = (start.x < end.x) ? Vector3.right : Vector3.left;
                playerLogic.gameObject.transform.position += moveVec*moveCoolTime;
            }
            yield return new WaitForSecondsRealtime(0.05f);
        }

        Board.isPressMoveButton = false;
    }
}
