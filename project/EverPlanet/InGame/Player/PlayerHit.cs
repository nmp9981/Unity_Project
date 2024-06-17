using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Monster")
        {
            int hit = collision.gameObject.GetComponent<MonsterFunction>().monsterHitDamage;
            GameManager.Instance.PlayerHP -= hit;
        }
    }
}
