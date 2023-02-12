using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public float dropSpeed = -0.03f;
    
    // Update is called once per frame
    void Update()
    {
        transform.Translate(0, this.dropSpeed, 0);//y축 방향
        if (transform.position.y < -1.0f)//범위 초과
        {
            Destroy(gameObject);
        }
    }
}
