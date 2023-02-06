using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        this.player = GameObject.Find("player");
    }

    // Update is called once per frame
    void Update()
    {
        //프레임마다 등속낙하
        transform.Translate(0, -0.1f, 0);
        //화면 밖
        if (transform.position.y < -5.0f)
        {
            Destroy(gameObject);
        }
        //충돌판정
        Vector2 p1 = transform.position;//화살
        Vector2 p2 = this.player.transform.position;//플레이어
        Vector2 dir = p2 - p1;
        float d = dir.magnitude;
        float r1 = 0.5f;//화살 반경
        float r2 = 1.0f;//플레이어 반경

        //충돌
        if (d < r1 + r2)
        {
            //화살에 맞았다고 알린다.
            GameObject director = GameObject.Find("GameDirector");//GameDirector오브젝트 찾음
            director.GetComponent<GameDirector>().DecreaseHp();//GameDirector오브젝트의 GameDirector스크립트 구하고 DecreaseHp() 호출
            //화살 파괴
            Destroy(gameObject);
        }
    }
}
