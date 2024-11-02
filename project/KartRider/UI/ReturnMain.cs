using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReturnMain : MonoBehaviour
{
    void Awake()
    {
        BindingReturnHome();
    }
    /// <summary>
    /// 기능 : 홈 버튼 바인딩
    /// </summary>
    void BindingReturnHome()
    {
        foreach (var gm in gameObject.GetComponentsInChildren<Button>(true))
        {
            string gmName = gm.gameObject.name;
            switch (gmName)
            {
                case "YesButton":
                    gm.onClick.AddListener(() => GoMainCheckButton());
                    break;
                case "NoButton":
                    gm.onClick.AddListener(() => CancelButton());
                    break;
                default:
                    break;
            }
        }
    }
    /// <summary>
    /// 메인씬으로 돌아감감
    /// </summary>
    public void GoMainCheckButton()
    {
        GameManager.MoveToMainScene();
    }
    public void CancelButton()
    {
        gameObject.SetActive(false);
    }
}
