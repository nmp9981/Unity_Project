using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackGroundManager : MonoBehaviour
{
    public List<Sprite> backGroundImage = new List<Sprite>();
    public int backGroundIndex = 0;
    Image currentBackgroundImage;

    void Awake()
    {
        currentBackgroundImage = GetComponent<Image>();
        currentBackgroundImage.sprite = backGroundImage[0];
    }

    public void ChangeBackground()
    {
        backGroundIndex = (GameManager.Instance.CurrentStage-1) / 3;
        currentBackgroundImage.sprite = backGroundImage[backGroundIndex];
    }
}
