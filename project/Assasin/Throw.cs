using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Throw : MonoBehaviour
{
    public float ThrowSpeed;//던지는 속도
    public float distance;//거리
    public LayerMask isLayer;//객체

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //물체 충돌, 시작점, 방향, 길이, 물체
        RaycastHit2D ray = Physics2D.Raycast(transform.position, transform.right,distance,isLayer);
        //특정물체와 충돌시
        if(ray.collider != null)
        {
            if(ray.collider.tag == "KingSlime")
            {
                ObjectPool.ReturnObject(this);
            }
        }
        transform.Translate(transform.right * ThrowSpeed * Time.deltaTime);
    }
}
