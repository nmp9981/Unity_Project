using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public Text score;
    private float scoreTime = 0;

    void Update()
    {
        scoreTime += Time.deltaTime;
        score.text = "점수 : " + Mathf.Round(scoreTime);
    }
}
