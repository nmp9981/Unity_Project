using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSetting : MonoBehaviour
{
    void Awake()
    {
        //GameManager.Instance.PlayerLV = 10;
        DataGet();
        GameManager.Instance.ApPoint = 0;
        GameManager.Instance.SkillPoint = (GameManager.Instance.PlayerLV-10) * 3;
        GameManager.Instance.PlayerJob = "Assassin";
        GameManager.Instance.PlayerDEX = 15 + GameManager.Instance.PlayerLV;
        GameManager.Instance.PlayerLUK = 5 + GameManager.Instance.PlayerLV * 4;
        GameManager.Instance.IsInvincibility = false;
        PlayerInfoInit(GameManager.Instance.PlayerLV);

    }

    void Update()
    {
        SetStat();
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

        ///<summary>
        ///경험치
        ///Lv20 : 1000
        ///Lv29 : 4205
        ///Lv30 : 5500
        ///Lv80 : 408966
        ///Lv99 : 4170440
        ///</summary>
        if (lv <= 20) GameManager.Instance.PlayerReqExp = lv * lv + 30 * lv;
        else if (lv > 20 && lv < 30) GameManager.Instance.PlayerReqExp = lv * lv * (lv / 5);
        else if (lv >= 30 && lv <= 80) GameManager.Instance.PlayerReqExp = 5500 * (long)Mathf.Pow(1.09f, lv - 30);
        else if (lv >= 81 && lv < 100) GameManager.Instance.PlayerReqExp = 408970 * (long)Mathf.Pow(1.13f, lv - 80);
        else GameManager.Instance.PlayerReqExp = 2147483647;

        //GameManager.Instance.PlayerReqExp = GameManager.Instance.PlayerReqExp / 150;
    }
    void SetStat()
    {
        int addAvoid = GameManager.Instance.IsAvoidBuffOn ? 10 : 0;
        GameManager.Instance.PlayerAvoid = 20+addAvoid;
        int addAcc = GameManager.Instance.IsAccBuffOn ? 10 : 0;
        GameManager.Instance.PlayerACC = (GameManager.Instance.PlayerDEX * 8 + GameManager.Instance.PlayerLUK * 5) / 10+GameManager.Instance.PlayerAddACC+addAcc;
        long addAttack = GameManager.Instance.IsAtaackBuffOn ? 8 : 0;
        GameManager.Instance.PlayerAttack = (5 * GameManager.Instance.PlayerLUK / 2 + GameManager.Instance.PlayerDEX)+addAttack;
    }
    void LevelUP()
    {
        if (GameManager.Instance.PlayerEXP >= GameManager.Instance.PlayerReqExp)
        {
            GameManager.Instance.PlayerLV += 1;
            if (GameManager.Instance.PlayerLV > 100) GameManager.Instance.PlayerLV = 100;
            PlayerInfoInit(GameManager.Instance.PlayerLV);
            GameManager.Instance.ApPoint += 5;
            GameManager.Instance.SkillPoint += 3;
            PlayerPrefs.SetInt("Lv", GameManager.Instance.PlayerLV);
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
        GameManager.Instance.ApPoint = 0;
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
    void DataGet()
    {
        if (PlayerPrefs.HasKey("Lv")) GameManager.Instance.PlayerLV = PlayerPrefs.GetInt("Lv");
        else GameManager.Instance.PlayerLV = 10;
        if (PlayerPrefs.HasKey("Meso")) GameManager.Instance.PlayerMeso = PlayerPrefs.GetInt("Meso");
        else GameManager.Instance.PlayerMeso = 10000;
    }
}
