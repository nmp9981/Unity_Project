using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stome : MonoBehaviour
{
    Vector3 target;
   
    public void Init()
    {
        GameObject circle = GameObject.Find("Circle");
        target = circle.transform.position;
        circle.SetActive(false);
        gameObject.transform.position = new Vector3(0,1,0);
    }
    void Update()
    {
        gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, target, Time.deltaTime);
    }
    
}
