using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSetting : MonoBehaviour
{
    void Awake()
    {
        GameManager.Instance.PlayerLV = 70;
        GameManager.Instance.ApPoint = 0;
        GameManager.Instance.PlayerJob = "Assassin";
        PlayerInfoInit(GameManager.Instance.PlayerLV);
    }

    void Update()
    {
        HPMPManager();
        LevelUP();
    }
    void PlayerInfoInit(int lv)
    {
        GameManager.Instance.PlayerMaxHP = lv * 40 + 300;
        GameManager.Instance.PlayerHP = GameManager.Instance.PlayerMaxHP;

        GameManager.Instance.PlayerMaxMP =lv * 25 + 150;
        GameManager.Instance.PlayerMP = GameManager.Instance.PlayerMaxMP;

        long restExp = GameManager.Instance.PlayerEXP - GameManager.Instance.PlayerReqExp;
        GameManager.Instance.PlayerEXP = restExp>0?restExp:0;
        GameManager.Instance.PlayerReqExp = lv*lv/2+10*lv;

        GameManager.Instance.PlayerDEX = 15+ GameManager.Instance.PlayerLV;
        GameManager.Instance.PlayerLUK = 5+4*GameManager.Instance.PlayerLV;
        GameManager.Instance.PlayerACC = (GameManager.Instance.PlayerDEX*8+ GameManager.Instance.PlayerLUK*5)/10;
        GameManager.Instance.PlayerAttack = 5* GameManager.Instance.PlayerLUK/2+ GameManager.Instance.PlayerDEX;
    }

    void LevelUP()
    {
        if (GameManager.Instance.PlayerEXP >= GameManager.Instance.PlayerReqExp)
        {
            GameManager.Instance.PlayerLV += 1;
            PlayerInfoInit(GameManager.Instance.PlayerLV);
            GameManager.Instance.ApPoint += 5;
        }
    }
    void HPMPManager()
    {
        GameManager.Instance.PlayerHP = Mathf.Max(GameManager.Instance.PlayerHP, 0);
        GameManager.Instance.PlayerHP = Mathf.Min(GameManager.Instance.PlayerHP, GameManager.Instance.PlayerMaxHP);
        GameManager.Instance.PlayerMP = Mathf.Max(GameManager.Instance.PlayerMP, 0);
        GameManager.Instance.PlayerMP = Mathf.Min(GameManager.Instance.PlayerMP, GameManager.Instance.PlayerMaxMP);
    }
    public void AutoStatSetting()
    {
        GameManager.Instance.PlayerDEX +=(GameManager.Instance.ApPoint/5);
        GameManager.Instance.ApPoint -= (GameManager.Instance.ApPoint / 5);
        GameManager.Instance.PlayerLUK += GameManager.Instance.ApPoint;
    }
    public void UpDEX()
    {
        if (GameManager.Instance.ApPoint >= 1)
        {
            GameManager.Instance.PlayerDEX += 1;
            GameManager.Instance.ApPoint -= 1;
        }
    }
    public void UpLuk()
    {
        if (GameManager.Instance.ApPoint >= 1)
        {
            GameManager.Instance.PlayerLUK += 1;
            GameManager.Instance.ApPoint -= 1;
        }
    }
}
