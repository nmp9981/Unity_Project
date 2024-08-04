using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBressPrice : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && !GameManager.Instance.IsInvincibility)
        {
            int damage = UnityEngine.Random.Range(4700, 5400);
            GameManager.Instance.PlayerHP -= damage;
            if (GameManager.Instance.PlayerHP <= 0) GameManager.Instance.PlayerHP = 0;
            StartCoroutine(collision.gameObject.GetComponent<PlayerHit>().ShowDamage(damage));
        }
    }
}
