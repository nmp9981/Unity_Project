using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickMovement : MonoBehaviour
{
    private Camera camera;

    bool isMove;
    Vector3 destination;//목적지
    void Awake()
    {
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            RaycastHit hit;
            if(Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition),out hit))//마우스가 클릭한 위치를 찾음
            {
                SetDestination(hit.point);//마우스 클릭한 위치
            }
        }
        Move();
    }
    //목적지 설정
    void SetDestination(Vector3 dest)
    {
        destination = dest;
        isMove = true;
    }
    //목적지까지 이동
    private void Move()
    {
        if (isMove)
        {
            var dir = destination - transform.position;//방향
            transform.position += dir.normalized * Time.deltaTime*5f;//이동
        }
        //목적지 도달
        if (Vector3.Distance(transform.position, destination) <= 0.1f)
        {
            isMove = false;
        }
    }
}
