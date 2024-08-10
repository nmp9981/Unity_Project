using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    GameObject player;
    Vector3 eyePos;
    Vector3 dir;
    Vector3 dirNomal;
    int laserTimer;

    private void OnEnable()
    {
        laserTimer = 0;
        player = GameObject.Find("Player");
        gameObject.transform.localScale = Vector3.one;

        /*
        dir = player.transform.position - eyePos;
        dir.y *=10;
        dirNomal = dir.normalized;

        Debug.Log(dirNomal.x + " " + dirNomal.y + " " + dirNomal.z);
        Quaternion rot = Quaternion.LookRotation(dirNomal, Vector3.up);
        Debug.Log(rot.x + " " + rot.y + " " + rot.z);
        gameObject.transform.rotation = rot;
        */

        dir = player.transform.position - eyePos;
        Vector3 rotateAngle = new Vector3(RotateX(dir), RotateY(dir), RotateZ(dir));
        gameObject.transform.Rotate(rotateAngle);
        InvokeRepeating("LaserShot", 2f,0.15f);
    }
    void LaserShot()
    {
        laserTimer += 1;
        if (laserTimer < 30)
        {
            gameObject.transform.localScale = new Vector3(1, laserTimer, 1);
            gameObject.transform.position += dirNomal;
        }
        else gameObject.SetActive(false);
    }
    public void LaserPos(Vector3 pos)
    {
        eyePos = pos;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player" && !GameManager.Instance.IsInvincibility)
        {
            int damage = UnityEngine.Random.Range(4700, 5400);
            GameManager.Instance.PlayerHP -= damage;
            if (GameManager.Instance.PlayerHP <= 0) GameManager.Instance.PlayerHP = 0;
            StartCoroutine(collision.gameObject.GetComponent<PlayerHit>().ShowDamage(damage));
        }
    }
    //각 축별로 회전 각도
    float RotateX(Vector3 dir)
    {
        float cosTheta = dir.x / dir.magnitude;
        float theta = Mathf.Acos(cosTheta) * 180 / Mathf.PI;
        return theta;
    }
    float RotateY(Vector3 dir)
    {
        float cosTheta = dir.y / dir.magnitude;
        float theta = Mathf.Acos(cosTheta) * 180 / Mathf.PI;
        return theta;
    }
    float RotateZ(Vector3 dir)
    {
        float cosTheta = dir.z / dir.magnitude;
        float theta = Mathf.Acos(cosTheta) * 180 / Mathf.PI;
        return theta;
    }
}
