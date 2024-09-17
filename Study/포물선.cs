using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] float deg;//포신 각도
    [SerializeField] float turretSpeed;//포신 스피드
    public GameObject turret;//포신 오브젝트
    public GameObject bullet;//총알 오브젝트

    void Start()
    {
        
    }

    void Update()
    {
        ControllDeg();
        ShotMissile();
    }
    /// <summary>
    /// 기능 : 포신 각도 조절
    /// </summary>
    void ControllDeg()
    {
        float vAxis = Input.GetAxisRaw("Vertical");

        if (vAxis != 0)
        {
            //도를 라디안으로
            float rad = deg * Mathf.Deg2Rad;
            //회전 각도
            deg = deg + vAxis * turretSpeed * Time.deltaTime*5;

            //포신 초기 위치
            turret.transform.localPosition = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad),0);
            //회전 하기
            turret.transform.eulerAngles = new Vector3(0, 0, deg);
        }
    }
    /// <summary>
    /// 기능 : 미사일 발사
    /// </summary>
    void ShotMissile()
    {
        if (Input.GetKeyDown(KeyCode.Space)){
            GameObject gm = Instantiate(bullet);
            //초기 위치 = 포신의 위치
            gm.transform.position = turret.transform.position;
            gm.transform.eulerAngles = turret.transform.eulerAngles;
            Rigidbody rigid = gm.GetComponent<Rigidbody>();
            rigid.AddForce(turret.transform.localPosition * 500f);
        }
    }
}
