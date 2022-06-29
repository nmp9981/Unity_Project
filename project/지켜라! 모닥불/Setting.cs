using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    private bool toggle = false;

    public GameObject main_menu;

    void Update()
    {
        stop_setting();//화면 멈추기
    }
    public void Panel_Set()
    {
        toggle = !toggle;

        if (toggle == true) // 옵션을 열면 시간을 멈춤
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }

        main_menu.SetActive(toggle); // 패널 이미지 보이고 안보이고 설정
    }

    //화면 멈추기
    void stop_setting()
    {
        if (main_menu.activeSelf == true)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1.0f;
        }
    }
}
