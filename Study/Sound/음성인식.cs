using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mic : MonoBehaviour
{
    public AudioClip audioClip;
    public float rmsValue;

    public float raiseNum;
    public int cutValue;
    public int resultValue;

    int sampleRate = 44100;
    private float[] samples;
   
    void Awake()
    {
        samples = new float[sampleRate];
        audioClip = Microphone.Start(Microphone.devices[0].ToString(), true, 1, sampleRate);//녹음된 데이터를 넣기
    }

    private void Update()
    {
        //현재 녹음된 데이터를 실수형 배열로 가져온다. offset은 시작위치 이므로 0으로 해준다.
        audioClip.GetData(samples, 0);//-1~1

        //데이터 값의 평균
        float sum = 0;
        for(int i = 0; i < samples.Length; i++)
        {
            sum += samples[i] * samples[i];//sample의 범위가 -1~1이므로 제곱평균을 구해준다.
        }
        rmsValue = Mathf.Sqrt(sum / samples.Length);
        rmsValue = rmsValue * raiseNum;
        rmsValue = Mathf.Clamp(rmsValue, 0, 100);
        resultValue = Mathf.RoundToInt(rmsValue);//0~100사이만 나오게

        if (resultValue < cutValue) resultValue = 0;//범위 초과값 잡아주기
    }
}
