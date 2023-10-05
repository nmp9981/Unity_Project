using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public bool isMelee;//근접 공격 여부

    private void OnCollisionEnter(Collision collision)
    {
        //바닥에 닿으면
        if(collision.gameObject.tag == "Floor")
        {
            Destroy(gameObject,3);//3초뒤 사라짐
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!isMelee && other.gameObject.tag == "Wall")//근접이 아닌것만
        {
            Destroy(gameObject);//투사체만 파괴
        }
    }
}
