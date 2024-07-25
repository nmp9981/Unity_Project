using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stome : MonoBehaviour
{
    Vector3 target;
    bool isAttack;
    public void Init()
    {
        GameObject circle = GameObject.Find("Circle");
        target = circle.transform.position;
        circle.SetActive(false);
        gameObject.transform.position = new Vector3(0,1,0);
        isAttack = true;
    }
    void Update()
    {
        if (isAttack)
        {
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, target, Time.deltaTime);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        isAttack = false;
    }
}
