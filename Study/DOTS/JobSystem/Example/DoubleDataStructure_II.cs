using ProjectDawn.Collections;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities.UniversalDelegates;
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
        [NativeDisableContainerSafetyRestriction]
        public NativeArray<UnsafeHashSet<int>> duoSet;//하나씩 분리

        public UnsafeHashSet<int> hashSet;
       
        public void Execute()
        {
            
            for (int i = 0; i < 3; i++)
            {
                hashSet = new UnsafeHashSet<int>(11,Allocator.TempJob);
                for (int j = 0; j < 3; j++)
                {
                    hashSet.Add(a * i + j);//3*i+j
                }
                duoSet[i] = hashSet;
                foreach (int x in duoSet[i])
                {
                    Debug.Log(x + "\n");
                }
                hashSet.Dispose();
            }
            for(int i = 0; i < 3; i++)
            {
                foreach (int x in duoSet[i])
                {
                    Debug.Log(x + "\n");
                }
            }
        }
    }
    UnsafeHashSet<int> hashSet;
    public NativeArray<UnsafeHashSet<int>> douSet;
   
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
        hashSet = new UnsafeHashSet<int>(11,Allocator.TempJob);//11개 할당
        douSet = new NativeArray<UnsafeHashSet<int>>(11, Allocator.TempJob);

        JobSingle2 jobSingle2;
        JobHandle handle2;

        jobSingle2 = new JobSingle2();
        
        jobSingle2.a = 4;
        jobSingle2.hashSet = hashSet;
        jobSingle2.duoSet = douSet;
        
        handle2 = jobSingle2.Schedule();
        
        // 메인스레드가 잡의 종료 대기
        handle.Complete();
        handle2.Complete();//여기서 Execute가 실행
        
        //Job을 실행 할 수 있도록 워커 스레드에 예약함, 
        //메인 스레드에서는 Schedule만 호출 할 수 있음

        //출력
        Debug.Log(" result : " + jobSingle.array[0]);

        for(int i=0;i<3;i++)
        {
            foreach (int x in douSet[i])
            {
                Debug.Log("Num "+ i +" result2 : " + x + "\n");
            }
            
        }
        
        // NativeContainer를 사용한 후에는 Dispose를 호출해서 메모리에서 삭제해야함.
        // Dispose를 호출하지 않으면 Unity에서 에러를 냄 
        jobSingle.array.Dispose();
        jobSingle2.duoSet.Dispose();
    }
}
