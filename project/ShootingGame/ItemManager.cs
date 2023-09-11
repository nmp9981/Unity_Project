using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public string itemName;
    
    public void GetItem(string type)
    {
        Debug.Log(type);
        switch (type.Substring(0,4))
        {
            case "Coin":
                GamaManager.Instance.Score += 1;
                break;
            case "Boom":
                GamaManager.Instance.startAttack += 1;
                break;
            case "RedP":
                GamaManager.Instance.HP += GamaManager.Instance.HealHP;
                if (GamaManager.Instance.HP > GamaManager.Instance.FullHP) GamaManager.Instance.HP = GamaManager.Instance.FullHP;
                break;
            case "Blue"://최대 HP증가
                GamaManager.Instance.FullHP += GamaManager.Instance.RaiseFullHP;
                GamaManager.Instance.HP += 1;
                break;
        }
    }
}
