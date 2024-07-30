using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mist : MonoBehaviour
{
    float doteTime;
    float curTime;
    int doteDamage;
    float curMistRange;
    float maxMistRange = 21f;
    [SerializeField] GameObject player;
    ObjectFulling objectfulling;

    private void Awake()
    {
        objectfulling = GameObject.Find("ObjectManager").GetComponent<ObjectFulling>();
        GetComponent<Mist>().enabled = true;
    }
    private void OnEnable()
    {
        curTime = 1f;
        doteTime = 1.5f;
        doteDamage = 800;
        gameObject.transform.localScale = Vector3.one;
        curMistRange = 0;
        InvokeRepeating("CloudSizeUp", 0.1f, 0.2f);
    }
    private void Update()
    {
        curTime += Time.deltaTime;
        InCloud();
    }
    void CloudSizeUp()
    {
        if (gameObject.transform.localScale.x < maxMistRange)
        {
            gameObject.transform.localScale += 2 * Vector3.one;
            curMistRange += 2;
        }
    }
    void InCloud()
    {
        float dist = (player.transform.position - gameObject.transform.position).sqrMagnitude;
        if (dist < curMistRange * curMistRange)
        {
            if (curTime > doteTime && !GameManager.Instance.IsInvincibility)
            {
                GameManager.Instance.PlayerHP -= doteDamage;
                if (GameManager.Instance.PlayerHP <= 0) GameManager.Instance.PlayerHP = 0;
                curTime = 0;
                StartCoroutine(player.GetComponent<PlayerHit>().ShowDamage(doteDamage));
            }
        }
        else GameManager.Instance.IsInvincibility = false;
    }
}
