using Microsoft.Office.Interop.Excel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectTwoPoint : MonoBehaviour
{
    [SerializeField]
    List<GameObject> points = new List<GameObject>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ConnectTwoPoints(points[0].transform.position, points[1].transform.position,0.1f);
        }
    }
    public void ConnectTwoPoints(Vector3 point1, Vector3 point2, float size)
    {
        //벡터 차
        Vector3 dir = point2- point1;

        //내적을 이용해서 y축 회전각도 계산
        //두 벡터 모두 정규화된 벡터이므로 크기는 둘다 1
        float dotValueY = Vector3.Dot(dir.normalized, Vector3.forward);
        float angleY = Mathf.Acos(dotValueY) * 180f * (1f / Mathf.PI);//호도법->육십분법

        //좌우판정, 단순 내적만으로는 좌우판정 불가
        //1 : 오른쪽, -1 : 왼쪽
        float judgeRightOrLeftValueY = Vector3.Dot(Vector3.up, Vector3.Cross(Vector3.forward, dir.normalized));
        float sideDirY = (judgeRightOrLeftValueY >= 0) ? 1 : -1;

        //y축 사영 벡터
        Vector3 projectionYDir = new Vector3(dir.x, 0, dir.z);
        //내적을 이용해서 x축 회전각도 계산
        float dotValueX = Vector3.Dot(dir.normalized, projectionYDir.normalized);
        float angleX = Mathf.Acos(projectionYDir.magnitude/dir.magnitude) * 180f * (1f / Mathf.PI);//호도법->육십분법

        //좌우판정, 단순 내적만으로는 좌우판정 불가
        //1 : 오른쪽, -1 : 왼쪽
        float judgeRightOrLeftValueX = Vector3.Dot(Vector3.right, Vector3.Cross(Vector3.up, projectionYDir.normalized));
        float sideDirX = (judgeRightOrLeftValueX >= 0) ? 1 : -1;

        //선분 생성
        GameObject pipeGM = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pipeGM.transform.eulerAngles = new Vector3(angleX*sideDirX-90, angleY * sideDirY, 0);
        pipeGM.transform.position = (point1 + point2) * 0.5f;
        pipeGM.transform.localScale = new Vector3(size, dir.magnitude * 0.5f, size);
    }
}
