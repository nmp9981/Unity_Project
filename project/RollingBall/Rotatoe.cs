using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    // 각 프레임을 렌더링하기 전에
    void Update()
    {
        // 이 스크립트가 연결된 게임 오브젝트를 X축으로 15도,
        // Y축으로 30도, Z축으로 45도 회전하고 deltaTime 값을 곱하면
        // 프레임이 아닌 초를 기준으로 회전합니다.
        transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime);
    }
}
