using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type {Ammo,Coin, Grenade,Heart,Weapon}//열거형 타입
    public Type type;//타입
    public int value;//값

    Rigidbody rigid;
    SphereCollider sphereCollider;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        //GetComponent는 첫번째 컴포넌트만 가져옴
        sphereCollider = GetComponent<SphereCollider>();
    }
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * 25*Time.deltaTime);//10도씩 회전
    }

    //아이템이 바닥에 닿으면 콜라이더 해제
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            rigid.isKinematic = true;//물리 해제
            sphereCollider.enabled = false;//콜라이더 비할성화
        }
    }
}
