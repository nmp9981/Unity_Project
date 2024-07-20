using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHeal : MonoBehaviour
{
    int healhpAmount, healmpAmount;
    int healhpIdx, healmpIdx;
    void Start()
    {
        GameManager.Instance.HPPosionCount = 0;
        GameManager.Instance.MPPosionCount = 0;
        GameManager.Instance.ReturnVillegeCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            healhpAmount = GameManager.Instance.HPPosionHealAmount;
            healhpIdx = GameManager.Instance.KeyHPIdx;
            HealHP(healhpAmount, healhpIdx);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            healmpAmount = GameManager.Instance.MPPosionHealAmount;
            healmpIdx = GameManager.Instance.KeyMPIdx;
            HealMP(healmpAmount, healmpIdx);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            ReturnToVillege();
        }
    }
    public void HealHP(int healHPAmount, int idx)
    {
        if (GameManager.Instance.HPPosionCount <= 0) return;
        GameManager.Instance.PlayerHP = Mathf.Min(GameManager.Instance.PlayerHP+ healHPAmount, GameManager.Instance.PlayerMaxHP);
        GameManager.Instance.HPPosionCount -= 1;
        GameManager.Instance.storeItemList[idx].theNumber -= 1;
    }
    public void HealMP(int healMPAmount,int idx)
    {
        if (GameManager.Instance.MPPosionCount <= 0) return;
        GameManager.Instance.PlayerMP = Mathf.Min(GameManager.Instance.PlayerMP + healMPAmount, GameManager.Instance.PlayerMaxMP);
        GameManager.Instance.MPPosionCount -= 1;
        GameManager.Instance.storeItemList[idx].theNumber -= 1;
    }
    public void ReturnToVillege()
    {
        if (GameManager.Instance.ReturnVillegeCount < 0) return;
        GameManager.Instance.ReturnVillegeCount -= 1;
        this.gameObject.transform.position = PortalManager.PortalInstance.portalist[0].transform.position;//마을로 귀환
    }
}
