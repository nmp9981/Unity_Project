using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    GameObject player;
    int laserTimer;

    private void OnEnable()
    {
        laserTimer = 0;
        player = GameObject.Find("Player");
        transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, Quaternion.LookRotation(player.transform.position), Time.deltaTime * 30f);//플레이어를 향하게
        InvokeRepeating("LaserShot", 2f,0.1f);
    }
    void LaserShot()
    {
        laserTimer += 1;
        if (laserTimer < 30)
        {
            gameObject.transform.localScale = new Vector3(1, laserTimer, 1);
        }
        else gameObject.SetActive(false);
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
}
