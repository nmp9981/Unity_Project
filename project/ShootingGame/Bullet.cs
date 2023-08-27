using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    float rotZ = 10.0f;//회전 각도

    /*
    //총알 상시 회전
    private void Update()
    {
        this.gameObject.transform.Rotate(0, 0, rotZ);        
    }
    */
    //벽에 부딪히면 비활성화
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Wall")
        {
            this.gameObject.SetActive(false);
        }
    }
}
