using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    [SerializeField] GameObject arrowObj;//화살 오브젝트
    [SerializeField] Transform m_tfArrow;//활의 위치

    void LookAtMouse()
    {
        //3D에서는 카메라 z좌표를 빼준다.
        Vector2 t_mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y ,- Camera.main.transform.position.z));//마우스 입력 좌표를 월드좌표로
        //바라볼 방향 = 활 좌표 - 월드상 마우스 좌표
        Vector2 t_direction = new Vector2(t_mousePos.x - m_tfArrow.position.x, t_mousePos.y - m_tfArrow.position.y);
        m_tfArrow.right = t_direction;
    }
    void TryFire()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject t_arrow = Instantiate(arrowObj, m_tfArrow.position, m_tfArrow.rotation);
            t_arrow.GetComponent<Rigidbody2D>().velocity = t_arrow.transform.right * 10f;//오른쪽으로 10만큼 속도 조절
        }
    }
    // Update is called once per frame
    void Update()
    {
        LookAtMouse();
        TryFire();
    }
}
