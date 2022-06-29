using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Puddle : MonoBehaviour
{
    FireBall fireball;
    GameObject goImage;
    Color color;

    // Start is called before the first frame update
    void Start()
    {
        goImage = GameObject.FindWithTag("Puddle");
        color = goImage.GetComponent<Image>().color;
        
        fireball = GameObject.FindWithTag("FireBall").GetComponent<FireBall>();//FireBall 스크립트에서 변수 가져오기
    }

    // Update is called once per frame
    void Update()
    {
        Puddle_alpha();//물웅덩이 투명도
    }
    //물웅덩이 투명도
    void Puddle_alpha()
    {
        if (fireball.Rate_HP <= 0.2f)
        {
            color.a = 1f;
        }
        else if (fireball.Rate_HP < 0.4f && fireball.Rate_HP >= 0.2f)
        {
            float alpha_Rate = 1.0f - (fireball.Rate_HP-0.2f) / 0.2f;
            color.a = Mathf.Lerp(0f, 1f, alpha_Rate);
        }
        else
        {
            color.a = 0f;
        }
        goImage.GetComponent<Image>().color = color;
    }
}
