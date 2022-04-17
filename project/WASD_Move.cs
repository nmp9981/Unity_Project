using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;//씬 전환

public class Runner : MonoBehaviour
{
    [SerializeField] float MoveSpeed = 0.04f;//속력, inspector에서만 관리 외부에서 접근 불가능

    //벽과 충돌
    void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("Collision");
    }
    //트리거와 충돌
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger");
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float MoveAmountRL = MoveSpeed * (1.5f);//반환값은 true, 누르는 동안 true반환
        float MoveAmountUD = MoveSpeed * (1.2f);//반환값은 true, 누르는 동안 true반환
        if (Input.GetKey(KeyCode.A))//a를 누름
        {
            transform.Translate(-MoveAmountRL, 0, 0);//x축 이동
        }
        if (Input.GetKey(KeyCode.D))//d를 누름
        {
            transform.Translate(MoveAmountRL, 0, 0);//x축 이동
        }
        if (Input.GetKey(KeyCode.W))//w를 누름
        {
            transform.Translate(0, MoveAmountUD, 0);//y축 이동
        }
        if (Input.GetKey(KeyCode.S))//s를 누름
        {
            transform.Translate(0, -MoveAmountUD, 0);//y축 이동
        }
    }
}
