using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip bgm;
    public AudioClip[] sfx;//효과음 배열

    public AudioSource audioSource;//스피커 클래스
    // Start is called before the first frame update
    void Start()
    {
        //AudioSource 컴포넌트를 받는다.
        audioSource = this.gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void sfx0Play()//왼쪽 버튼 누를 시
    {
        audioSource.GetComponent<AudioSource>().PlayOneShot(sfx[0]);//splash 재생
    }
    public void sfx1Play()//오른쪽 버튼 누를 시
    {
        audioSource.GetComponent<AudioSource>().PlayOneShot(sfx[1]);//buttom2 재생
    }
}
