using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public string itemName;
    
    public void GetItem(string type)
    {
        switch (type.Substring(0,4))
        {
            case "Coin":
                GamaManager.Instance.Score += 1;
                break;
            case "Boom":
                GamaManager.Instance.startAttack += 1;
                break;
            case "Posi":
                GamaManager.Instance.HP += 2;
                if(GamaManager.Instance.HP > GamaManager.Instance.FullHP) GamaManager.Instance.HP = GamaManager.Instance.FullHP
                break;
        }
    }
}
