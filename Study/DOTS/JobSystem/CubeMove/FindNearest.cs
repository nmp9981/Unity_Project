using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

public class FindNearest : MonoBehaviour
{
    NativeArray<float3> TargetPositions;
    NativeArray<float3> SeekerPositions;
    NativeArray<float3> NearestTargetPositions;

    public void Start()
    {
        Spawner spawner =GetComponent<Spawner>();

        TargetPositions = new NativeArray<float3>(spawner.NumTargets, Allocator.Persistent);
        SeekerPositions = new NativeArray<float3>(spawner.NumSeekers, Allocator.Persistent);
        NearestTargetPositions = new NativeArray<float3>(spawner.NumSeekers, Allocator.Persistent);
    }

    public void OnDestroy()
    {
        TargetPositions.Dispose();//객체 파괴
        SeekerPositions.Dispose();
        NearestTargetPositions.Dispose();
    }
    //가장 가까운 타켓을 찾는다.
    // Update is called once per frame
    public void Update()
    {
        for(int i = 0; i < TargetPositions.Length; i++)
        {
            TargetPositions[i] = Spawner.TargetTransforms[i].localPosition;
        }
        for (int i = 0; i < SeekerPositions.Length; i++)
        {
            SeekerPositions[i] = Spawner.SeekerTransforms[i].localPosition;
        }

        SortJob<float3, AxisXComparer> sortJob = TargetPositions.SortJob(new AxisXComparer { });

        FindNearestJob findJob = new FindNearestJob
        {
            TargetPositions = TargetPositions,
            SeekerPositions = SeekerPositions,
            NearestTargetPositions = NearestTargetPositions,
        };

        JobHandle sortHandle = sortJob.Schedule();
        JobHandle findHandle = findJob.Schedule(SeekerPositions.Length,100,sortHandle);
        findHandle.Complete();

        for (int i = 0; i < SeekerPositions.Length; i++) Debug.DrawLine(SeekerPositions[i], NearestTargetPositions[i]);
        /*
        Vector3 nearestTargetPosition = default;
        float nearestDistSq = float.MaxValue;

        foreach(var targetTransform in Spawner.TargetTransforms)
        {
            Vector3 offset = targetTransform.localPosition - transform.localPosition;//떨어진 거리
            float distSq = offset.sqrMagnitude;//거리
            if(distSq < nearestDistSq)//더 가까운것이 있다면 거리와 위치 갱신
            {
                nearestDistSq = distSq;
                nearestTargetPosition = targetTransform.localPosition;
            }
        }
        */
        
    }
}
