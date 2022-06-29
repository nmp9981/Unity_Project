using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UmbrellaMgr : MonoBehaviour
{
    SpriteRenderer rend;
    PersonMov personMove;
    private const float Umbrella_Pos_y = -3.3f;

    // Start is called before the first frame update
    void Start()
    {
        personMove = GameObject.FindWithTag("Player").GetComponent<PersonMov>();//PersonMov 스크립트에서 변수 가져오기
        rend = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        UmbrellaPosition();//우산의 위치
    }
    void UmbrellaPosition()//우산의 위치
    {
        if(personMove.left && !personMove.right)//왼쪽만 눌렀을때
        {
            gameObject.GetComponent<Transform>().position += Vector3.left * personMove.speed * Time.deltaTime;
            rend.flipX = true;
        }else if (personMove.right && !personMove.left)//오른쪽만 눌렀을때
        {
            gameObject.GetComponent<Transform>().position += Vector3.right * personMove.speed * Time.deltaTime;
            rend.flipX = false;
        }
        if (gameObject.transform.position.x > personMove.End_Position_x) // 우산이 화면 밖으로 나가지 않게 범위 제한
        {
            gameObject.transform.position = new Vector3(-personMove.End_Position_x, Umbrella_Pos_y, 0); // 해당 위치 Y 값 넣어줘야 함
        }
        else if (gameObject.transform.position.x < -personMove.End_Position_x)
        {
            gameObject.transform.position = new Vector3(personMove.End_Position_x, Umbrella_Pos_y, 0);
        }
    }
}
