using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveBall : MonoBehaviour
{
    public Text LifePoint;//점수
    Vector3 StartPosition = new Vector3(0, 0, 0);//시작 위치
    [SerializeField] float BallSpeed = 0.04f;//공의 속도
    [SerializeField] float BallTheta = Mathf.PI/10.0f;//공의 각도
    [SerializeField] float Dir_X = 1.0f;//x방향
    [SerializeField] float Dir_Y = 1.0f;//y방향
    int Life = 3;//초기 생명

    //벽과 충돌
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "SideWall" || other.gameObject.tag == "Bar")//태그 인식
        {
            Dir_X *= (-1f);
        }else if (other.gameObject.tag == "WallUp" || other.gameObject.tag=="WallDown")
        {
            Dir_Y *= (-1f);
        }
    }
    //게임 종료
    void GameExit()
    {
        Application.Quit();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 Pos;//위치
        //공의 위치 갱신
        float Ball_X = Dir_X*BallSpeed*Mathf.Cos(BallTheta);
        float Ball_Y = Dir_Y*BallSpeed*Mathf.Sin(BallTheta);
        transform.Translate(Ball_X, Ball_Y, 0);//공의 이동
        Pos = transform.position;//공의 현재 위치 구하기
        LifePoint.text = "Life : "+Life.ToString();//생명 텍스트를 화면에 표시

        //공이 밖으로 나가면 시작지점으로 리스폰
        if (Pos.x < -9.5f)
        {
            transform.position = StartPosition;//시작지점으로 리스폰
            Life -= 1;//생명 감소
        }else if (Pos.y > 4.5f)//버그 수정
        {
            transform.Translate(Ball_X,-0.2f, 0);//공의 이동
        }
        else if (Pos.y < -4.5f)//버그 수정
        {
            transform.Translate(Ball_X, 0.2f, 0);//공의 이동
        }
        //생명이 모두 떨어지면 아웃
        if (Life < 0)
        {
            LifePoint.text = "OUT";
            GameExit();
        }
    }
}
