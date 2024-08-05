using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class RhinoSkill : MonoBehaviour
{
    GameObject player;
    GameObject laserObj;
    GameObject fireBress;
    ObjectFulling objectFulling;
    public GameObject eyes;


    int[] SpawnDirXpos = {-1,-1,1,1};
    int[] SpawnDirZpos = { -1, 1, -1, 1 };
    private void Awake()
    {
        GetComponent<BearBossFunction>().enabled = true;
        player = GameObject.Find("Player");
        laserObj = GameObject.Find("Laser");
        fireBress = GameObject.Find("FireBress");
        laserObj.SetActive(false);
        fireBress.SetActive(false);
        objectFulling = GameObject.Find("ObjectManager").GetComponent<ObjectFulling>();
        eyes.GetComponent<MeshRenderer>().material.color = Color.black;
    }
    void OnEnable()
    {
        InvokeRepeating("RhinoAttack", 5f, 8f);
    }
    void RhinoAttack()
    {
        float dist = (player.transform.position - gameObject.transform.position).sqrMagnitude;
        if (dist < 1600 && gameObject.activeSelf)
        {
            int ran = Random.Range(0, 8);
            ran = 1;
            switch (ran % 4)
            {
                case 0://잡몹 소환
                    SubordinateMonsterSpawn();
                    break;
                case 1://레이저(원거리)
                    Laser();
                    break;
                case 2://범위 공격(불 도트)
                    FireBressOn();
                    break;
                case 3://점프 공격
                    JumpAttack();
                    break;
            }
        }
    }
    //잡몹 소환
    void SubordinateMonsterSpawn()
    {
        for(int i = 0; i < 4; i++)
        {
            GameObject gm = objectFulling.MakeObj(22);
            gm.transform.position = gameObject.transform.position + new Vector3(SpawnDirXpos[i]*11, 0f, SpawnDirZpos[i]*11);
            gm.transform.localScale = 2 * Vector2.one;
        }
    }
    //레이저
    void Laser()
    {
        eyes.GetComponent<MeshRenderer>().material.color = Color.red;
        laserObj.transform.position = eyes.transform.position;
        laserObj.GetComponent<Laser>().LaserPos(eyes.transform.position);
        laserObj.SetActive(true);
        laserObj.transform.localScale = new Vector3(1, 0.5f, 1);
    }
    //범위 공격
    void FireBressOn()
    {
        fireBress.transform.position = gameObject.transform.position;
        fireBress.SetActive(true);
        Invoke("FireBressOff",5f);
    }
    void FireBressOff()
    {
        fireBress.SetActive(false);
    }

    //점프 공격
    void JumpAttack()
    {
        gameObject.transform.position += Vector3.up * 40f;
    }
}
