using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFulling : MonoBehaviour
{
    public GameObject graphSet;
    public GameObject pointObjectPrefab;
    public GameObject[] pointList;
    const int countMaxPoint = 4005;//-40~40
    // Start is called before the first frame update
    void Awake()
    {
        pointList = new GameObject[countMaxPoint];//오브젝트 공간 생성
        //처음엔 모두 비활성화
        for (int i = 0; i < countMaxPoint; i++)
        {
            pointList[i] = Instantiate(pointObjectPrefab);
            pointList[i].SetActive(false);
        }
    }

    //오브젝트 활성화
    public void ActiveObject()
    {
        for (int i = 0; i < countMaxPoint-1; i++)
        {
            float xpos = (float)(-countMaxPoint/2+i)/50.0f;//실제 x좌표
            float ypos = graphSet.GetComponent<GraphSet>().F(xpos);//실제 y좌표
            pointList[i].transform.position = new Vector3(xpos/3.0f, ypos/3.0f, 0);//실제 찍히는 위치
            pointList[i].SetActive(true);
        }
    }
    //오브젝트 비활성화(그래프 초기화)
    public void InitGraph()
    {
        for (int i = 0; i < countMaxPoint; i++) pointList[i].SetActive(false);
        graphSet.GetComponent<GraphSet>().elemental.Clear();
    }
}
