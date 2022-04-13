using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] float MoveSpeed = 0.05f;//속력

    void OnCollisionEnter2D(Collision2D other)//벽 지점 통과
    {
        Debug.Log("Yeah!");
    }
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
        float MoveAmount = Input.GetAxis("Horizontal") * MoveSpeed;//x축 위치(키를 누르면 매 프레임마다 이동)
        transform.Translate(MoveAmount, 0, 0);//이동
        float MoveAmountVT = Input.GetAxis("Vertical") * MoveSpeed;//y축 위치(키를 누르면 매 프레임마다 이동)
        transform.Translate(0, MoveAmountVT, 0);//이동
    }
}
