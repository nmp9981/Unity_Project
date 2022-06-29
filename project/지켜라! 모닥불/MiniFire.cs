using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniFire : MonoBehaviour
{
    FireBall fireball;
    GameObject goImage;
    AudioManager theAudioManager;
    Color color;
    Color[] FireballColor = new Color[] { Color.red, new Color(1.0f, 0.5f, 0f), Color.yellow, Color.white, Color.blue };//색 배열

    Image img;
    // Start is called before the first frame update
    void Start()
    {
        img = GetComponent<Image>();
        theAudioManager = AudioManager.instance;//오디오 매니저 참조
        goImage = GameObject.FindWithTag("Minifire");
        color = goImage.GetComponent<Image>().color;

        fireball = GameObject.FindWithTag("FireBall").GetComponent<FireBall>();//FireBall 스크립트에서 변수 가져오기
    }

    // Update is called once per frame
    void Update()
    {
        img.color = FireballColor[fireball.FireBall_Lv - 1];//색 변화
        MiniFire_alpha();//잔불 투명도
        //theAudioManager.PlayBGM("background");
    }
    //잔불 투명도 조절
    void MiniFire_alpha()
    {
        if (fireball.Rate_HP >= 0.8f)
        {
            color.a = 1f;
            theAudioManager.PlaySFX("bonfire");//잔불이 있을때만 불타는 소리 들리게
        }else if(fireball.Rate_HP<0.8f && fireball.Rate_HP >= 0.6f)
        {
            float alpha_Rate = (fireball.Rate_HP-0.6f) / 0.2f;
            color.a = Mathf.Lerp(0f, 1f, alpha_Rate);
            theAudioManager.PlaySFX("bonfire");//잔불이 있을때만 불타는 소리 들리게
        }
        else
        {
            color.a = 0f;
        }
        theAudioManager.PlaySFX("raining");//비는 항상 들려야함
        goImage.GetComponent<Image>().color = color;
    }
}
