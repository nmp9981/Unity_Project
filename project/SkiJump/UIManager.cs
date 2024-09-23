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
    BallMove ballMove;
    [SerializeField]
    Sprite resetButtonImage;
    void Awake()
    {
        GameObject goButton = new GameObject("goButton");
        GameObject resetButton = new GameObject("resetButton");

        ballMove = GameObject.Find("Ball").GetComponent<BallMove>();
        GoButtonsetting(goButton);
        ResetButtonSetting(resetButton);
    }

    void GoButtonsetting(GameObject goButton)
    {
        goButton.transform.parent = uiCanvas.transform;
        goButton.AddComponent<RectTransform>();
        goButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(-400, -300);

        goButton.AddComponent<Button>();
        goButton.GetComponent<Button>().onClick.AddListener(GoButton);

        goButton.AddComponent<Image>();
        goButton.GetComponent<Image>().sprite = resetButtonImage;

        GameObject childResetButton = new GameObject("goText");
        childResetButton.AddComponent<TextMeshProUGUI>();
        childResetButton.transform.parent = goButton.transform;
        childResetButton.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        childResetButton.GetComponentInChildren<TextMeshProUGUI>().text = "Go";
        childResetButton.GetComponentInChildren<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
    }
    void ResetButtonSetting(GameObject resetButton)
    {
        resetButton.transform.parent = uiCanvas.transform;
        resetButton.AddComponent<RectTransform>();
        resetButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(-100, -300);
        resetButton.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 100);

        resetButton.AddComponent<Button>();
        resetButton.GetComponent<Button>().onClick.AddListener(ResetButton);
       
        resetButton.AddComponent<Image>();
        resetButton.GetComponent<Image>().sprite = resetButtonImage;

        GameObject childResetButton = new GameObject("resetText");
        childResetButton.AddComponent<TextMeshProUGUI>();
        childResetButton.transform.parent = resetButton.transform;
        childResetButton.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        childResetButton.GetComponentInChildren<TextMeshProUGUI>().text = "Re";
        childResetButton.GetComponentInChildren<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
    }

    void GoButton()
    {
        ballMove.Move();
    }
    void ResetButton()
    {
        ballMove.gameObject.transform.position = new Vector3(-14, 3.6f, 0);
    }
}
