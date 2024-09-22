using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    GameObject uiCanvas;

    Button resetButton;
    void Awake()
    {
        
        if (resetButton == null)
        {
            Instantiate(resetButton,new Vector2(800,200),Quaternion.identity,uiCanvas.transform);
            
            resetButton.AddComponent<Button>();

            resetButton.enabled = true;
            resetButton.GetComponentInChildren<TextMeshProUGUI>().text = "재시작";
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
