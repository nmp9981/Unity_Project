using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type {Ammo,Coin, Grenade,Heart,Weapon}//열거형 타입
    public Type type;//타입
    public int value;//값

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * 25*Time.deltaTime);//10도씩 회전
    }
}
