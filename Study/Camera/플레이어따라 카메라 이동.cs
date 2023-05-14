using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManage : MonoBehaviour
{
    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        this.player = GameObject.Find("Bike");//해당 오브젝트를 찾는다.
    }

    // Update is called once per frame
    void LateUpdate()//플레이어 이동 뒤 카메라가 따라다니므로 LateUpdate()가 적당하다.
    {
        Vector3 playerPos = this.player.transform.position;//캐릭터 위치
        transform.position = new Vector3(playerPos.x, transform.position.y, transform.position.z);//카메라는 x축만 이동
    }
}
