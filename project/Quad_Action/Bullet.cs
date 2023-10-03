using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;

    private void OnCollisionEnter(Collision collision)
    {
        //바닥에 닿으면
        if(collision.gameObject.tag == "Floor")
        {
            Destroy(gameObject,3);//3초뒤 사라짐
        }else if(collision.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }
}
