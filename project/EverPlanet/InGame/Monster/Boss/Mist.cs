using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mist : MonoBehaviour
{
    float doteTime;
    float curTime;
    int doteDamage;
    [SerializeField] GameObject player;
    GameObject bossObj;
    private void Awake()
    {
        bossObj = GameObject.Find("Human_Mutant");
    }
    private void OnEnable()
    {
        curTime = 1.4f;
        doteTime = 1.5f;
        doteDamage = 800;
        gameObject.transform.position = bossObj.transform.position; //보스 위치로 위치 지정
        gameObject.transform.localScale = Vector3.one;
        InvokeRepeating("CloudSizeUp", 0.1f, 0.2f);
    }
    private void Update()
    {
        curTime += Time.deltaTime;
    }
    void CloudSizeUp()
    {
        if(gameObject.transform.localScale.x<11f) gameObject.transform.localScale += 2*Vector3.one;
    }
    private void OnCollisionStay(Collision collision)
    {
        if(curTime > doteTime && !GameManager.Instance.IsInvincibility)
        {
            GameManager.Instance.PlayerHP -= doteDamage;
            if (GameManager.Instance.PlayerHP <= 0) GameManager.Instance.PlayerHP = 0;
            curTime = 0;
            StartCoroutine(player.GetComponent<PlayerHit>().ShowDamage(doteDamage));
        }
    }
}
