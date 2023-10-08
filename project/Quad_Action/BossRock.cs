using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRock : Bullet
{
    Rigidbody rigid;
    float angularPower = 2;
    float scalevalue = 0.1f;
    bool isShoot;

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        StartCoroutine(GainPowerTimer());
        StartCoroutine(GainPower());
    }

    //기를 모음
    IEnumerator GainPowerTimer()
    {
        yield return new WaitForSeconds(1.8f);
        isShoot = true;//쏘는 중
    }
    IEnumerator GainPower()
    {
        while (!isShoot)
        {
            angularPower += 0.02f;
            scalevalue += 0.002f;
            transform.localScale = Vector3.one * scalevalue;//크기 증가
            rigid.AddTorque(transform.right * angularPower, ForceMode.Acceleration);//회전, 점점 회전 속도가 증가해야함
            yield return null;//게임 정지 방지
        }
    }
}
