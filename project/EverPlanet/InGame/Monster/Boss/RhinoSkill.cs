using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhinoSkill : MonoBehaviour
{
    GameObject player;
    private void Awake()
    {
        GetComponent<BearBossFunction>().enabled = true;
        player = GameObject.Find("Player");
    }
    void OnEnable()
    {
        InvokeRepeating("RhinoAttack", 5f, 8f);
    }
    void RhinoAttack()
    {
        float dist = (player.transform.position - gameObject.transform.position).sqrMagnitude;
        if (dist < 1500 && gameObject.activeSelf)
        {
            int ran = Random.Range(0, 8);
            switch (ran % 4)
            {
                case 0:
                    
                    break;
                case 1:
                   
                    break;
                case 2:
                    
                    break;
                case 3:

                    break;
            }
        }
    }
}
