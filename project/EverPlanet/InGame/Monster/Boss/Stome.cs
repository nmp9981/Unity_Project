using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Stome : MonoBehaviour
{
    Vector3 dir;
    Vector3 circlePos;
    Vector3 returnPos;
    bool isAttack;
    GameObject player;

    private void Awake()
    {
        player = GameObject.Find("Player");
        gameObject.transform.position = new Vector3(100,50,0);
    }
    public void Init(Vector3 targetPos, GameObject circle, Vector3 bossPos)
    {
        gameObject.SetActive(true);
        circlePos = targetPos;
        circle.SetActive(false);
        isAttack = true;
        returnPos = circlePos+new Vector3(0,30,0);
        gameObject.transform.position = returnPos;
    }
    private void Update()
    {
        if(isAttack) gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, circlePos, 10f);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            int damage = Random.Range(2400, 2850);
            GameManager.Instance.PlayerHP -= damage;
            if (GameManager.Instance.PlayerHP <= 0) GameManager.Instance.PlayerHP = 0;
            StartCoroutine(player.GetComponent<PlayerHit>().ShowDamage(damage));
            isAttack = false;
            Invoke("InitStone", 0.01f);
        }
        if (collision.gameObject.tag == "Ground") Invoke("InitStone", 1f);
    }
    void InitStone()
    {
        gameObject.transform.position = returnPos;
        gameObject.SetActive(false);
    }
}
