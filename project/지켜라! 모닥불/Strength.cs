using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Strength : MonoBehaviour
{
    public GameObject enchant;
    private bool strength_flag = false;
    private int RandNum1,RandNum2,RandNum3;
    FireBall fireball;

    // Start is called before the first frame update
    void Start()
    {
        fireball = GameObject.FindWithTag("FireBall").GetComponent<FireBall>();//FireBall 스크립트에서 변수 가져오기
    }
    void Update()
    {
        if (fireball.LvUP)
        {
            strength_flag = true;
            StartCoroutine(delay_UI());
        }

        stop_enchant();
    }
    //잠시 멈춤
    IEnumerator delay_UI()
    {
        enchant.SetActive(strength_flag);
        yield return new WaitForSeconds(1000);
        strength_flag = false;
        enchant.SetActive(strength_flag);
    }
    //화면 멈추기
    void stop_enchant()
    {
        if (enchant.activeSelf)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1.0f;
        }
    }
}
