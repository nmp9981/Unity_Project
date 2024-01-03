using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;

public class NativeDoubleSystem : MonoBehaviour
{
    struct JobSingle : IJob
    {
        public int a;
        public int b;
        public NativeArray<int> array;
        
        public void Execute()
        {
            array[0] = a * b;
        }
    }

    struct JobSingle2 : IJob
    {
        public int a;
        public int b;
        public int c;
        [NativeDisableContainerSafetyRestriction]
        public unsafe NativeArray<NativeHashSet<int>> duoSet;

        public void Execute()
        {
            duoSet[0].Add(a);
            duoSet[0].Add(b);
            duoSet[1].Add(c);
            duoSet[1].Add(a);
        }
    }
    private void Start()
    {
        NativeArray<int> array = new NativeArray<int>(11, Allocator.TempJob);
        JobSingle jobSingle = new JobSingle();
        jobSingle.a = 15;
        jobSingle.b = 12;
        jobSingle.array = array;

        //메인 스레드에서 값을 사용할 수 있게 NativeArray 선언
        JobHandle handle = jobSingle.Schedule();//작업1 예약

        //작업 2에대한 데이터 세팅
        NativeArray<NativeHashSet<int>> douSet = new NativeArray<NativeHashSet<int>>(11, Allocator.TempJob);
        JobSingle2 jobSingle2 = new JobSingle2();
        jobSingle2.a = 3;
        jobSingle2.b = 4;
        jobSingle2.duoSet = douSet;

        JobHandle handle2 = jobSingle2.Schedule();//작업2 예약
        //JobHandle handle2 = Unity.Jobs.LowLevel.Unsafe.JobsUtility.Schedule(ref jobSingle2);

        // 메인스레드가 잡의 종료 대기
        handle.Complete();
        handle2.Complete();

        //Job을 실행 할 수 있도록 워커 스레드에 예약함, 
        //메인 스레드에서는 Schedule만 호출 할 수 있음

        Debug.Log(" result : " + jobSingle.array[0]);
        
        for(int i = 0; i < 2; i++)
        {
            foreach (int x in jobSingle2.duoSet[i])
            {
                Debug.Log(" result2 : " + x + "\n");
            }
        }
        
        // NativeContainer를 사용한 후에는 Dispose를 호출해서 메모리에서 삭제해야함.
        // Dispose를 호출하지 않으면 Unity에서 에러를 냄 
        jobSingle.array.Dispose();
        jobSingle2.duoSet.Dispose();
    }
}
