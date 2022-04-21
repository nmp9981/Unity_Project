using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveBall : MonoBehaviour
{
    public Text ExpPoint;//점수
    public Text Life;//생명
    Vector3 StartPosition = new Vector3(-5, -5, 0);//시작 위치
    
    [SerializeField] float BallSpeed = 0.07f;//공의 속도
    public float BallTheta = 0f;//공의 각도
    public int Exp = 0;//점수
    public int LifePoint = 3;//생명 개수
    int Wall_Count = 10;//벽의 개수
    float Ball_X,Ball_Y;//공의 위치

    //벽과 충돌
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "LeftWall" || other.gameObject.tag == "RightWall")//태그 인식
        {
            BallTheta = (Mathf.PI - BallTheta)%(2*Mathf.PI); 
        }
        else if(other.gameObject.tag == "Paddle" || other.gameObject.tag == "UpWall")//패들 인식
        {
            BallTheta = -BallTheta;
        }
        else
        {
            BallTheta = -BallTheta;
            Wall_Count -= 1;//벽의 개수 감소
            Exp += 300;//점수 증가
        }
    }
    //게임 종료
    void GameExit()
    {
        BallSpeed = 0;//공 정지
        Application.Quit();
    }
 
    // Start is called before the first frame update
    void Start()
    {
        BallTheta = (Random.Range(-30.0f, 30.0f) + 270.0f) * Mathf.PI / 180.0f;
    }

    // Update is called once per frame
    void Update()//프레임별 처리
    {
        Vector3 Pos;//위치
        //공의 위치 갱신
        Ball_X = BallSpeed * Mathf.Cos(BallTheta);
        Ball_Y = BallSpeed * Mathf.Sin(BallTheta);
        transform.Translate(Ball_X, Ball_Y, 0);//공의 이동
        Pos = transform.position;//공의 현재 위치 구하기
        ExpPoint.text = "Exp : " + Exp;//점수 텍스트를 화면에 표시
        Life.text = "Life : " + LifePoint;//남은 생명 개수

        //공이 밖으로 나가면 시작지점으로 리스폰
        if (Pos.y < -20.0f)
        {
            transform.position = StartPosition;//시작지점으로 리스폰
            LifePoint -= 1;//생명감소
        }
        //게임 클리어
        if (Wall_Count==0)
        {
            ExpPoint.text = "Clear";
            GameExit();
        }else if (LifePoint <= 0)//게임 종료
        {
            Life.text = "Game Over";
            GameExit();
        }
    }
}
