using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    //플에이어와 컴퓨터 정보, Start()전에는 변수선언만
    public GameObject ImageObj;
    public int Player;
    public int Com;
    public Image ResultImage;
    public bool Is_Button = false;//버튼이 켜져있는지
    public bool Result = true;//결과

    public Sprite[] spr;//sprite 배열
    SpriteRenderer spriteRenderer;

    //상대팀 랜덤
    void RandomIndex()//가위,바위,보 랜덤
    {
        Com = Random.Range(0, 3);
    }
    //Start is called before the first frame update
    void Start()//처음 1회만
    {
        spriteRenderer = GetComponent<SpriteRenderer>();//내 spriteRender와 연결
        spr = Resources.LoadAll<Sprite>("RockScissorPaper");//이미지 가져오기, 반드시 Resource하위 폴더에 위치
        RandomIndex();
    }
    //각 버튼에 대한 함수는 따로 만들어준다
    public void Press_Rock()
    {
        Is_Button = true;
        Player = 0;
    }
    public void Press_Scissor()
    {
        Is_Button = true;
        Player = 1;
    }
    public void Press_Paper()
    {
        Is_Button = true;
        Player = 2;
    }
    //이미지 출력
    void output()
    {
        if (Is_Button)
        {
            spriteRenderer.sprite = spr[Com];
        }
    }
    //승패결정
    int Win()
    {
        return (Com - Player) % 3;
    }
    // Update is called once per frame
    void Update()//매 프레임마다 반복
    {
        output();
        //승패 가르기
        if (Is_Button && Result)
        {
            if (Win() == 1)
            {
                Debug.Log("WIN");
            }
            else if (Win() == 2)
            {
                Debug.Log("LOSE");
            }
            else
            {
                Debug.Log("DRAW");
            }
            Result = false;
        }
    }
}
