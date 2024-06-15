using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI playerExpText;
    [SerializeField] Image playerExpFill;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ShowPlayerUI();
    }
    void ShowPlayerUI()
    {
        playerExpText.text = GameManager.Instance.PlayerEXP.ToString();
        playerExpFill.fillAmount = (float)GameManager.Instance.PlayerEXP / (float)2000;
    }
}
