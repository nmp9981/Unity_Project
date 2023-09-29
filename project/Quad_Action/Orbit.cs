using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    public Transform target;//플레이어
    public float orbitSpeed;
    Vector3 offset;//위치 보정값(반지름)

    void Start()
    {
        offset = transform.position - target.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = target.position + offset;//위치 설정
        //타겟(플레이어) 주위를 회전하는 함수(목표가 움직이면 일그러짐)
        transform.RotateAround(target.position, Vector3.up, orbitSpeed * Time.deltaTime);
        offset = transform.position - target.position;//위치 수정
    }
}
