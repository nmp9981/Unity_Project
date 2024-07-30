using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prickle : MonoBehaviour
{
    int prickleCount = 0;
    private void OnEnable()
    {
        prickleCount = 0;
        GameManager.Instance.IsInvincibility = false;
        InvokeRepeating("PrickleMove", 0.1f, 0.05f);
    }
    void PrickleMove()
    {
        if (prickleCount<20)
        {
            prickleCount++;
            gameObject.transform.position += (Vector3.up * 0.25f);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player" && !GameManager.Instance.IsInvincibility)
        {
            int damage = UnityEngine.Random.Range(2750, 3450);
            GameManager.Instance.PlayerHP -= damage;
            if (GameManager.Instance.PlayerHP <= 0) GameManager.Instance.PlayerHP = 0;
            StartCoroutine(collision.gameObject.GetComponent<PlayerHit>().ShowDamage(damage));
        }
    }
}
