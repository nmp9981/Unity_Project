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

    [SerializeField]
    Sprite resetButtonImage;
    void Awake()
    {
        GameObject resetButton = new GameObject("resetButton");
        resetButton.transform.parent = uiCanvas.transform;
        resetButton.transform.position = new Vector2(800, 200);
        
        resetButton.AddComponent<Button>();
        resetButton.GetComponent<Button>().onClick.AddListener(check);

        resetButton.AddComponent<Image>();
        resetButton.GetComponent<Image>().sprite = resetButtonImage;

        GameObject childResetButton = new GameObject("resetText");
        childResetButton.AddComponent<TextMeshProUGUI>();
        childResetButton.transform.parent = resetButton.transform;
        resetButton.GetComponentInChildren<TextMeshProUGUI>().text = "재시작";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void check()
    {
        Debug.Log("확인");
    }
}
