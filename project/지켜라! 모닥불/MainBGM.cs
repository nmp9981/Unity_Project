using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MainBGM : MonoBehaviour
{
    public GameObject main_menu;

    private AudioSource BGMPlayer;

    Slider bgmSlider;

    // Start is called before the first frame update
    void Start()
    {
        BGMPlayer = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (main_menu.gameObject.activeSelf)//볼륨 조절, 메인메뉴 활성화
        {
            //DontDestroyOnLoad(transform.gameObject);
            bgmSlider = GameObject.FindWithTag("BGM_Slider").GetComponent<Slider>();//맨 처음에 비활성화 => 값을 받아올 수 없어 오류
            BGMPlayer.volume = bgmSlider.value;//볼륨
        }
    }
    void PlayBGM()
    {
        BGMPlayer.Play();//재생
    }
}
