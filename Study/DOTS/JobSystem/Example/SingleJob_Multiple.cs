using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

public class NativeArrayTest : MonoBehaviour
{
    struct JobSingle : IJob
    {
        public int a;
        public int b;
        public NativeArray<int> array;

        public void Execute()
        {
            array[0] = a*b;
        }
    }

    private void Start()
    {
        NativeArray<int> array = new NativeArray<int> (11, Allocator.TempJob);
        JobSingle jobSingle = new JobSingle();
        jobSingle.a = 15;
        jobSingle.b = 12;
        jobSingle.array = array;

        //메인 스레드에서 값을 사용할 수 있게 NativeArray 선언
        JobHandle handle = jobSingle.Schedule();

        // 메인스레드가 잡의 종료 대기
        handle.Complete();

        
        //Job을 실행 할 수 있도록 워커 스레드에 예약함, 
        //메인 스레드에서는 Schedule만 호출 할 수 있음


        Debug.Log(" result : " + jobSingle.array[0]);

        // NativeContainer를 사용한 후에는 Dispose를 호출해서 메모리에서 삭제해야함.
        // Dispose를 호출하지 않으면 Unity에서 에러를 냄 
        jobSingle.array.Dispose();
    }
}
