using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinFunction : MonoBehaviour
{
    const float rotationAngle = 30;
   
    void Update()
    {
        RotateCoin();
    }
    //코인 회전
    //y,x,z축 회전
    void RotateCoin()
    {
        gameObject.transform.parent.transform.Rotate(rotationAngle*Time.deltaTime,0,0);
    }
    //충돌 시 삭제
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            GameManager.Instance.IsCollideCoin = true;
            gameObject.SetActive(false);
            Invoke("RegenerateCoin",10f);
        }
    }
    // 10초 후 코인 재생성
    void RegenerateCoin()
    {
        gameObject.SetActive(true);
    }
}
