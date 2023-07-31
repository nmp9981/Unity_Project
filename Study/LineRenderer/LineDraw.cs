using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    public GameObject linePrefab;
    LineRenderer lr;
    EdgeCollider2D col;
    List<Vector2> points = new List<Vector2>();//꼭짓점들을 담을 배열
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //마우스 따라 선이 그려지게
        if (Input.GetMouseButtonDown(0))//찍기
        {
            GameObject go = Instantiate(linePrefab);
            lr = go.GetComponent<LineRenderer>();
            col = go.GetComponent<EdgeCollider2D>();
            points.Add(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            lr.positionCount = 1;
            lr.SetPosition(0, points[0]);
        }else if (Input.GetMouseButton(0))//그리기
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            //무한 점 생성 방지
            if (Vector2.Distance(points[points.Count - 1], pos) > 0.1f)
            {
                points.Add(pos);
                lr.positionCount++;
                lr.SetPosition(lr.positionCount - 1, pos);
                col.points = points.ToArray();
            }
        }
        else if(Input.GetMouseButtonUp(0))//마우스가 떼어짐
        {
            points.Clear();//초기화
        }
    }
}
