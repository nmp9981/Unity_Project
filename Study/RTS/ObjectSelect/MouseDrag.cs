using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class MouseDrag : MonoBehaviour
{
    [SerializeField]
    private RectTransform dragRectangle;//마우스 드래그한 범위를 가시화하는 Image UI의 Rectransform

    private Rect dragRect;//마우스 드래그한 범위
    private Vector2 start = Vector2.zero;//드래그 시작지점
    private Vector2 end = Vector2.zero;//드래그 종료 위치

    private Camera mainCamera;
    private RTSUnitController rtsUnitController;

    private void Awake()
    {
        mainCamera = Camera.main;
        rtsUnitController = GetComponent<RTSUnitController>();

        //start, end 가 (0,0)인 상태로 이미지 크기를 (0,0)으로 설정해 화면에 보이지 않도록 함
        DrawDragRectangle();
    }
   
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            start = Input.mousePosition;
            dragRect = new Rect();
        }
        if (Input.GetMouseButton(0))
        {
            end = Input.mousePosition;

            //마우스 클릭한 상태로 드래그 하는 동안 드래그 범위를 이미지로 표현
            DrawDragRectangle();
        }
        if (Input.GetMouseButtonUp(0))
        {
            //마우스 클릭을 종료할 때 드래그 범위내에 있는 유닛 선택
            CalculateGradRect();
            SelectUnits();

            //마우스 클릭시 종료 시 드래그 범위가 보이지 않게
            //start,end (0,0)으로 설정하고 드래그 범위를 그린다.
            start = Vector2.zero;
            end = Vector2.zero;
            DrawDragRectangle();
        }
    }
    void DrawDragRectangle()
    {
        //드래그 범위를 나타내는 Image UI위치
        dragRectangle.position = (start + end) * 0.5f;
        //드래그 범위를 나타내는 Image UI크기
        dragRectangle.sizeDelta = new Vector2(Mathf.Abs(start.x - end.x), Mathf.Abs(start.y - end.y));
    }
    void CalculateGradRect()
    {
        if (Input.mousePosition.x < start.x)
        {
            dragRect.xMin = Input.mousePosition.x;
            dragRect.xMax = start.x;
        }
        else
        {
            dragRect.xMin = start.x;
            dragRect.xMax = Input.mousePosition.x;
        }
        if (Input.mousePosition.y < start.y)
        {
            dragRect.yMin = Input.mousePosition.y;
            dragRect.yMax = start.y;
        }
        else
        {
            dragRect.yMin = start.y;
            dragRect.yMax = Input.mousePosition.y;
        }
    }
    void SelectUnits()
    {
        //모든 유닛 검사
        foreach(UnitController unit in rtsUnitController.UnitList)
        {
            //유닛의 월드좌표를 화면좌표로 변환해 드래그 범위에 있는지 검사
            if (dragRect.Contains(mainCamera.WorldToScreenPoint(unit.transform.position)))
            {
                rtsUnitController.DragSelectUnit(unit);
            }
        }
    }
}
