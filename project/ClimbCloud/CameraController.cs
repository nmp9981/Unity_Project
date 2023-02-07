using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        this.player = GameObject.Find("cat");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPos = this.player.transform.position;//캐릭터 위치
        transform.position = new Vector3(transform.position.x, playerPos.y, transform.position.z);//카메라는 y축으로만 이동, 나머지x,z축은 원래 좌표
    }
}
