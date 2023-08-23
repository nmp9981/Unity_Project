using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    public ObjectManager objectManager;
    int power = 1;

    float coolTime = 0.2f;
    float curTime;
    float shootSpeed = 5.0f;
    // Start is called before the first frame update
    void Awake()
    {
        curTime = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        Shoot();//공격
        Reload();//장전
    }
    //공격
    void Shoot()
    {
        if (!Input.GetButton("Fire1")) return;//마우스 좌클릭
        if (coolTime > curTime) return;//쿨타임 안됨

        curTime = 0.0f;
        switch (power)
        {
            case 1:
                GameObject bullet = objectManager.MakeGameObject("BulletPlayerA");//총알 소환
                bullet.transform.position = this.gameObject.transform.position;

                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                rigid.AddForce(Vector2.up*shootSpeed, ForceMode2D.Impulse);//앞으로 쏘기
                break;
        }
    }
    //장전
    void Reload()
    {
        curTime += Time.deltaTime;
    }
}
