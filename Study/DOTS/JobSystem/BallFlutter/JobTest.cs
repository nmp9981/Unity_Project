using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;
using Random = UnityEngine.Random;

public struct simpleJob : IJobParallelForTransform
{
    [ReadOnly] public NativeArray<Vector3> P0;
    [ReadOnly] public NativeArray<Vector3> P1;
    [ReadOnly] public NativeArray<Vector3> P2;
    [ReadOnly] public NativeArray<Vector3> P3;
    [ReadOnly] public NativeArray<float> t;
    public NativeArray<float> value;
    public float dealtaTime;

    public void Execute(int index, TransformAccess transform)
    {
        Vector3 a;
        a.x = Bezier(P0[index].x, P1[index].x, P2[index].x, P3[index].x, t[index]);
        a.y = Bezier(P0[index].y, P1[index].y, P2[index].y, P3[index].y, t[index]);
        a.z = Bezier(P0[index].z, P1[index].z, P2[index].z, P3[index].z, t[index]);
        transform.position = a;
        
        for (int i = 0; i < 5000; i++)
        {
            value[index] += Mathf.Sin(dealtaTime);
        }
    }

    float Bezier(float P0, float P1, float P2, float P3, float t)
    {
        //베지어함수
        return Mathf.Pow((1 - t), 3) * P0 + Mathf.Pow((1 - t), 2) * 3 * t * P1 + Mathf.Pow(t, 2) * 3 * (1 - t) * P2 +
               Mathf.Pow(t, 3) * P3;
    }
}

public class jobTest : MonoBehaviour
{
    public GameObject cubePrefab;
    public List<CubeStat> cubes;

    private float Speed = 5f;
    private float[] valueArray;

    public bool IsJob;

    private void Start()
    {
        for (int i = 0; i < 2000; i++)
        {
            GameObject cube = Instantiate(cubePrefab, new Vector3(Random.Range(-20f, 20f), 0, Random.Range(-10f, 10f)),
                quaternion.identity);
            if (cube.transform.TryGetComponent(out CubeStat stat))
            {
                stat.P0 = cube.transform.position;
                stat.P1 = stat.P0 + new Vector3(0, 10, 0);
                stat.P3 = stat.P0 + new Vector3(0, 0, 100);
                stat.P2 = stat.P3 + new Vector3(0, 10, 0);
                cubes.Add(stat);

            }

        }

        valueArray = new float[cubes.Count];
    }


    // Update is called once per frame
    void Update()
    {
        if (IsJob == true)
        {
            NativeArray<Vector3> P0 = new NativeArray<Vector3>(cubes.Count, Allocator.TempJob);
            NativeArray<Vector3> P1 = new NativeArray<Vector3>(cubes.Count, Allocator.TempJob);
            NativeArray<Vector3> P2 = new NativeArray<Vector3>(cubes.Count, Allocator.TempJob);
            NativeArray<Vector3> P3 = new NativeArray<Vector3>(cubes.Count, Allocator.TempJob);
            NativeArray<float> value = new NativeArray<float>(cubes.Count, Allocator.TempJob);
            NativeArray<float> t = new NativeArray<float>(cubes.Count, Allocator.TempJob);

            TransformAccessArray transformAccessArray = new TransformAccessArray(cubes.Count);
            
            for (int i = 0; i < cubes.Count; i++)
            {
                P0[i] = cubes[i].P0;
                P1[i] = cubes[i].P1;
                P2[i] = cubes[i].P2;
                P3[i] = cubes[i].P3;
                t[i] = Random.Range(0f, 1f);
                //0~1 사이로 설정
                transformAccessArray.Add(cubes[i].transform);
            }

            simpleJob job = new simpleJob
            {
                P0 = P0,
                P1 = P1,
                P2 = P2,
                P3 = P3,
                t = t,
                value = value

            };
            JobHandle jobHandle = job.Schedule(transformAccessArray);
            jobHandle.Complete();

            value.CopyTo(valueArray);


            P0.Dispose();
            P1.Dispose();
            P2.Dispose();
            P3.Dispose();
            t.Dispose();
            value.Dispose();
            transformAccessArray.Dispose();
        }
        else
        {
            //잡을 사용안할 때


            for (int i = 0; i < cubes.Count; i++)
            {
                float t = Random.Range(0f, 1f);
                Vector3 a;
                a.x = Bezier(cubes[i].P0.x, cubes[i].P1.x, cubes[i].P2.x, cubes[i].P3.x, t);
                a.y = Bezier(cubes[i].P0.y, cubes[i].P1.y, cubes[i].P2.y, cubes[i].P3.y, t);
                a.z = Bezier(cubes[i].P0.z, cubes[i].P1.z, cubes[i].P2.z, cubes[i].P3.z, t);

                cubes[i].transform.position = a;
                for (int j = 0; j < 5000; j++)
                {
                    valueArray[i] += Mathf.Sin(Time.deltaTime);
                }

            }


        }
    }


    public float Bezier(float P0, float P1, float P2, float P3, float t)
    {
        return Mathf.Pow((1 - t), 3) * P0 + Mathf.Pow((1 - t), 2) * 3 * t * P1 + Mathf.Pow(t, 2) * 3 * (1 - t) * P2 +
               Mathf.Pow(t, 3) * P3;
    }
}
