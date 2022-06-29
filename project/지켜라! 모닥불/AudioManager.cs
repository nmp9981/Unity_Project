using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//사운드 정보를 담는 클래스
[System.Serializable]//인스펙터 창에서 수정 가능하게
public class Sound
{
    public string name;
    public AudioClip clip;
}
public class AudioManager : MonoBehaviour
{
    public GameObject main_menu;
    public static AudioManager instance;
    public AudioSource bgmsource;
    public AudioSource sfxsource;
    Slider bgmSlider;
    Slider sfxSlider;

    private float bgmVolume;
    private float sfxVolume;

    [SerializeField] Sound bgm = null;
    [SerializeField] Sound[] sfx = null;

    [SerializeField] AudioSource bgmPlayer = null;//1개만
    [SerializeField] AudioSource[] sfxPlayer = null;//여러개 재생(배열로 구현)

    void Start()
    {
        instance = this;
        //초기 소리 크기
        bgmVolume = 0.5f;
        sfxVolume = 0.5f;
    }
    void Update()
    {
        if (main_menu.gameObject.activeSelf)//볼륨 조절, 메인메뉴 활성화
        {
            bgmSlider = GameObject.FindWithTag("BGM_Slider").GetComponent<Slider>();//맨 처음에 비활성화 => 값을 받아올 수 없어 오류
            sfxSlider = GameObject.FindWithTag("SFX_Slider").GetComponent<Slider>();
            bgmVolume = bgmSlider.value;//볼륨
            sfxVolume = sfxSlider.value;
        }
    }
    public void PlayBGM(string p_bgmName)
    {
        bgmPlayer.volume = bgmVolume;
        if (bgm.name == p_bgmName)
        {
            bgmPlayer.clip = bgm.clip;
            bgmPlayer.Play();//재생
        }
    }
    
    public void PlaySFX(string p_sfxName)
    {
        for (int i = 0; i < sfx.Length; i++)//sfx이 일치하는지 검사
        {
            if (sfx[i].name == p_sfxName)//sfx이 같으면
            {
                for(int j = 0; j < sfxPlayer.Length; j++)//재생중이지 않은 sfx를 찾음
                {
                    sfxPlayer[j].volume = sfxVolume;
                    if (!sfxPlayer[j].isPlaying)
                    {
                        sfxPlayer[j].clip = sfx[i].clip;
                        sfxPlayer[j].Play();//재생
                        return;//재생하면 더 이상 반복문을 실행할 이유가 없음
                    }
                }
                return;
            }
        }
    }
}
