using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public GameObject target;//타겟 : 플레이어 오브젝트
    //카메라 로직은 여기서
    private void LateUpdate()
    {
        CameraFollow();
    }
   
    //카메라 위치 조절
    private void CameraFollow()
    {
        //카메라는 오브젝트와 일정거리 떨어져 있어야함
        Vector3 Distance = new Vector3(0.0f, -2.0f, 10.0f); // 카메라가 바라보는 앞방향은 Z 축, 이동량에 따른 Z 축방향의 벡터를 구합니다.
        transform.position = target.transform.position - gameObject.transform.rotation * Distance; // 플레이어의 위치에서 카메라가 바라보는 방향에 벡터값을 적용한 상대 좌표를 빼준다.

    }

}
