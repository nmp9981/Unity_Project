using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using UnityEngine.Jobs;
using Unity.Burst;//버스트 컴파일러 사용

public class MyJob : MonoBehaviour
{
    SampleJob job;
    public Transform[] transforms;//위치배열
    private TransformAccessArray _transformAccessArray;//일반 클래스 Transform은 사용불가

    // Start is called before the first frame update
    void Start()
    {
        _transformAccessArray = new TransformAccessArray(transforms);
        job = new SampleJob() { time = Time.time };//time 구조체를 따로 빼서 사용;
    }

    //스레드 실행
    public struct SampleJob : IJobParallelForTransform//IJob으로만 하면 오류가 뜬다.(단일 tranform에 대한 접근을 지원 X)
    {
        public float time;
        public void Execute(int index, TransformAccess transform)
        {
            Vector3 position = transform.position;
            position.y = Mathf.Cos(time);//워커 스레드이므로 Time구조체 사용불가
            transform.position = position;
        }
    }
    // Update is called once per frame
    void Update()
    {
        job.time = Time.time;
        var handler = job.Schedule(_transformAccessArray);//워커 스레드에 작업 지시(Thread.Start())
    }
}
