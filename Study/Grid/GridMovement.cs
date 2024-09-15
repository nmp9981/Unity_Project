using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GridMovement : MonoBehaviour
{
    [SerializeField]
    private float moveMentTime = 0.5f;//1칸 이동 시 소요되는 시간

    public Vector3 moveDir { get; set; } = Vector3.zero;//이동 방향
    public bool isMove { get; set; } = false;//이동중인지

    private IEnumerator Start()
    {
        while (true)
        {
            // 이동방향이 있고 현재 이동중이 아니라면 이동
            if(moveDir!=Vector3.zero && !isMove)
            {
                //도착 지점
                Vector3 end = transform.position + moveDir;
                StartCoroutine(GridSmoothMovement(end));
            }
            yield return null;
        }
    }
    IEnumerator GridSmoothMovement(Vector3 end)
    {
        Vector3 start = transform.position;//시작 지점
        float current = 0f;
        float percent = 0f;

        isMove = true;//이동 시작
        
        while(percent < 1f)//0.5초 동안
        {
            current += Time.deltaTime;
            percent = current / moveMentTime;

            //부드러운 이동
            transform.position = Vector3.Lerp(start, end, percent);

            yield return null;
        }
        isMove = false;//이동 종료
    }
}
