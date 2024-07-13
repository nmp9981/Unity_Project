using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHeal : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.HPPosionCount = 100;
        GameManager.Instance.MPPosionCount = 100;
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
        if (Input.GetKeyDown(KeyCode.L))
        {
            ReturnToVillege();
        }
    }
    void HealHP()
    {
        if (GameManager.Instance.HPPosionCount <= 0) return;
        int healHPAmount = 1000;
        GameManager.Instance.PlayerHP = Mathf.Min(GameManager.Instance.PlayerHP+ healHPAmount, GameManager.Instance.PlayerMaxHP);
        GameManager.Instance.HPPosionCount -= 1;
    }
    void HealMP()
    {
        if (GameManager.Instance.MPPosionCount <= 0) return;
        int healMPAmount = 300;
        GameManager.Instance.PlayerMP = Mathf.Min(GameManager.Instance.PlayerMP + healMPAmount, GameManager.Instance.PlayerMaxMP);
        GameManager.Instance.MPPosionCount -= 1;
    }
    void ReturnToVillege()
    {
        if (GameManager.Instance.ReturnVillegeCount < 0) return;
        GameManager.Instance.ReturnVillegeCount -= 1;
        this.gameObject.transform.position = PortalManager.PortalInstance.portalist[0].transform.position;//마을로 귀환
    }
}
