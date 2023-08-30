using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI expPoint;

    // Update is called once per frame
    void Update()
    {
        expPoint.text = "<size=100%>Score : </size=90%>" + GamaManager.Instance.Score.ToString();
    }
    public void Init()
    {
        GamaManager.Instance.Score = 0;
    }
    //득점
    public void getScore(string type)
    {
        switch (type)
        {
            case "EnemyA":
                GamaManager.Instance.Score += 2;
                break;
            case "EnemyB":
                GamaManager.Instance.Score += 3;
                break;
        }
    }
}
