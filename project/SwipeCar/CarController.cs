using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    float speed = 0;
    Vector2 startPos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))//마우스 클릭시
        {
            //마우스를 클릭한 좌표
            this.startPos = Input.mousePosition;
        }else if (Input.GetMouseButtonUp(0))//마우스 뗐을 때
        {
            Vector2 endPos = Input.mousePosition;
            float swipeLength = endPos.x - startPos.x;

            //스와이프 길이를 처음속도로
            this.speed = swipeLength / 1000.0f;

            //효과음 재생
            GetComponent<AudioSource>().Play();
        }
        transform.Translate(this.speed, 0, 0);//이동
        this.speed *= 0.98f;//감속
    }
}
