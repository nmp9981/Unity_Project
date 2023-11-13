using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Entities.UniversalDelegates;
using System.Collections.Generic;
using UnityEditor;
using System;

[BurstCompile]
public struct FindNearestJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<float3> TargetPositions;
    [ReadOnly] public NativeArray<float3> SeekerPositions;

    public NativeArray<float3> NearestTargetPositions;

    public void Execute(int index)
    {
        float3 seekerPos = SeekerPositions[index];

        int startIndex = TargetPositions.BinarySearch(seekerPos, new AxisXComparer { });

        //인덱스 범위 초과
        if (startIndex < 0) startIndex = ~startIndex;
        if (startIndex >= TargetPositions.Length) startIndex = TargetPositions.Length - 1;

        float3 nearestTargetPos = TargetPositions[startIndex];
        float nearestDistSq = math.distancesq(seekerPos, nearestTargetPos);

        Search(seekerPos, startIndex + 1, TargetPositions.Length, +1, ref nearestTargetPos, ref nearestDistSq);

        Search(seekerPos, startIndex - 1, -1, -1, ref nearestTargetPos, ref nearestDistSq);

        NearestTargetPositions[index] = nearestTargetPos;
    }
    void Search(float3 seekerPos,int startIdx, int endIdx, int step,ref float3 nearestTargetPos, ref float nearestDistSq)
    {
        for (int i = startIdx; i!=endIdx; i+=step)
        {
            float3 targetPos = TargetPositions[i];
            float xdiff = seekerPos.x-targetPos.x;//두 오브젝트 사이의 거리

            if ((xdiff * xdiff) > nearestDistSq) break;//거리 초과

            float distSq = math.distancesq(targetPos, seekerPos);
            if (distSq < nearestDistSq)//더 작은거리가 오면 갱신
            {
                nearestDistSq = distSq;
                nearestTargetPos = targetPos;
            }
        }
    }
}

public struct AxisXComparer : IComparer<float3>
{
    public int Compare(float3 a, float3 b)
    {
        return a.x.CompareTo(b.x);
    }
}
