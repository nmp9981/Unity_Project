using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHeal : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            HealHP();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            HealMP();
        }
    }
    void HealHP()
    {
        int healHPAmount = 1000;
        GameManager.Instance.PlayerHP = Mathf.Min(GameManager.Instance.PlayerHP+ healHPAmount, GameManager.Instance.PlayerMaxHP);
    }
    void HealMP()
    {
        int healMPAmount = 300;
        GameManager.Instance.PlayerMP = Mathf.Min(GameManager.Instance.PlayerMP + healMPAmount, GameManager.Instance.PlayerMaxMP);
    }
}