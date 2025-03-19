using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyRejection : MonoBehaviour
{
    public int lEdge = 20;
    public int limit = 1000;
    public int loop = 400;
    public float threshold = 0.75f;

    public GameObject prefab;
   
    void Start()
    {
        StartCoroutine(Generate());
    }

    IEnumerator Generate()
    {
        //총 생성 개수 : Sequence(limit, threshold) * loop
        for (int i = 0; i < loop; i++)
        {
            yield return new WaitForSeconds(0.01f);
            //Sequence(limit, threshold)개 만큼 생성
            var sequenceList = Sequence(limit, threshold);
                foreach (var pos in sequenceList)
                {
                    var pos_ = pos * lEdge;
                    Instantiate(prefab, pos_, Quaternion.identity);
                }
        }
    }

    private IEnumerable<Vector3> Sequence(int limit, float threshold)
    {
        float randomX;
        float randomY;
        float randomZ;
        //잡음
        float noiseValue;

        for (int i = 0; i < limit; i++)
        {
            randomX = Random.value;
            randomY = Random.value;
            randomZ = Random.value;
            noiseValue = getDensityFloat(new Vector3(randomX, randomY, randomZ));
            if (noiseValue > threshold)//생성 조건
                yield return new Vector3(randomX, randomY, randomZ);
        }
    }

    //역치 비교 값
    public float getDensityFloat(Vector3 loc)
    {
        float val = PerlinNoise3D(loc.x, loc.y, loc.z);
        return Mathf.Lerp(0f, 1f, val);//0~1사이 정규화
    }
    //noise값
    float PerlinNoise3D(float x, float y, float z)
    {
        float xy = Mathf.PerlinNoise(x, y);
        float xz = Mathf.PerlinNoise(x, z);
        float yz = Mathf.PerlinNoise(y, z);
        float yx = Mathf.PerlinNoise(y, x);
        float zx = Mathf.PerlinNoise(z, x);
        float zy = Mathf.PerlinNoise(z, y);

        return (xy + xz + yz + yx + zx + zy) / 6;
    }
