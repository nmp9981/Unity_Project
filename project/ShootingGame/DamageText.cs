using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        this.gameObject.GetComponent<Text>().text = "";
    }
    public void DamageTextOn(int dmg, Vector3 textPosition)
    {
        
        this.gameObject.transform.position = Camera.main.WorldToScreenPoint(textPosition +new Vector3(0, 0.5f, 0));
        this.gameObject.GetComponent<Text>().text = dmg.ToString();
        Invoke("DamageTextOff", 1.0f);
    }
    void DamageTextOff()
    {
        this.gameObject.GetComponent<Text>().text = "";
    }
}
