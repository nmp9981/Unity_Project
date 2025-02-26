using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BoardUI : MonoBehaviour
{
    [SerializeField]
    PlayerLogic playerLogic;

    [SerializeField]
    TextMeshProUGUI positionText;

    private void Update()
    {
        ShowPositionText();
    }

    void ShowPositionText()
    {
        Vector2 pos = new Vector2(Mathf.Round(playerLogic.transform.position.x), Mathf.Round(playerLogic.transform.position.y));
        positionText.text = $"Position\n({pos.x}, {pos.y})";
    }
}
