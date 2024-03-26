using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mic : MonoBehaviour
{
    AudioSource audioSource;
   
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    //재생
    public void PlaySound()
    {
        audioSource.Play();
    }

    //녹음
    public void RecordSound()
    {
        //이름, 반복 여부, 녹음 시간, 주파수
        audioSource.clip = Microphone.Start(Microphone.devices[0].ToString(), false, 10, 44100);
    }
}
