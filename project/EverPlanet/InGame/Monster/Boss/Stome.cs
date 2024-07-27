using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stome : MonoBehaviour
{
    Vector3 dir;
    bool isAttack;
    public void Init(Vector3 targetPos, GameObject circle, Vector3 bossPos)
    {
        circle.SetActive(false);
        gameObject.transform.position = bossPos;
        isAttack = true;
        dir = (targetPos-bossPos).normalized;
        InvokeRepeating("StoneMove", 0.05f, 0.1f);
    }
    void StoneMove()
    {
        if (isAttack)
        {
            gameObject.transform.position += dir;
            //gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, target, 1f);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Player")
        {
            isAttack = false;
        }
    }
}
