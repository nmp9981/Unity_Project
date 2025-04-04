using komietty.Math;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyMCMC : MonoBehaviour
{
    //영역 크기
    public int lEdge = 20;
    //뽑을 샘플 수
    public int loop = 400;

    public int nInitialize = 100;
    public int nlimit = 100;
    public float threshold = -100;
    public GameObject[] prefabArr = new GameObject[0];

    //샘플 데이터
    Vector4[] data;

    //현재 위치
    Vector3 _curr;
    //현재 비율
    float _currDensity = 0f;

    int limitResetLoopCount = 100;
    int weightReferenceloopCount = 500;

    Vector3 Scale;

    void Start()
    {
        Prepare();
        StartCoroutine(Generate());
    }

    //목표 분포 : Simplex Noise
    void Prepare()
    {
        // ledge크기의 정육면체 영역
        data = new Vector4[lEdge * lEdge * lEdge];
        for (int x = 0; x < lEdge; x++)
            for (int y = 0; y < lEdge; y++)
                for (int z = 0; z < lEdge; z++)
                {
                    var i = x + lEdge * y + lEdge * lEdge * z;//인덱스
                    var val = PerlinNoise3D(x,y,z);
                    data[i] = new Vector4(x, y, z, val);
                }
    }
    //잡음 계산
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

    //뽑기 실행
    IEnumerator Generate()
    {
        //loop개를 뽑음
        for (int i = 0; i < loop; i++)
        {
            //랜덤 물체
            int rand = (int)Mathf.Floor(Random.value * prefabArr.Length);
            var prefab = prefabArr[rand];
            //0.1초 쿨타임
            yield return new WaitForSeconds(0.1f);
            //마코프 체인 사용
            foreach (var pos in Chain(nInitialize, nlimit, threshold))
            {
                //물체를 pos위치에 생성
                Instantiate(prefab, pos, Quaternion.identity);
            }
        }
    }

    //마코프 체인
    public IEnumerable<Vector3> Chain(int nInitialize, int limit, float threshold)
    {
        //리셋 - 임의 시작포인트 선택
        for (var i = 0; i < limitResetLoopCount; i++)
        {
            _curr = new Vector3(Scale.x * Random.value, Scale.y * Random.value, Scale.z * Random.value);
            _currDensity = Density(_curr);
        }

        //전이
        //제안 분포로부터 다음 포인트 추천받기
        for (var i = 0; i < nInitialize; i++)
            Next(threshold);

        //패자 부활전
        for (var i = 0; i < limit; i++)
        {
            yield return _curr;
            Next(threshold);
        }
    }

    // 전이 : 다음 지점
    void Next(float threshold)
    {
        //다음 점 : 제안 분포 + 이전 분포
        Vector3 next = GenerateRandomPointStandard() + _curr;

        //목표 분포상에서의 확률 밀도 계산, 0~1
        var densityNext = Density(next);
        
        //균등분포 난수보다 큰가?
        //p = min(a,1) => a=(다음 확률밀도/현재 확률밀도)
        //무작위 값 u와 비교
        bool flag1 = Mathf.Min(1f, densityNext / _currDensity) >= Random.value;
        //역치보다 큰가?
        bool flag2 = densityNext > threshold;

        //전이 가능
        if (flag1 && flag2)
        {
            _curr = next;
            _currDensity = densityNext;
        }
    }
    //목표 분포상에서의 확률 비율 계산
    float Density(Vector3 pos)
    {
        float weight = 0f;
        for (int i = 0; i < weightReferenceloopCount; i++)
        {
            int id = (int)Mathf.Floor(Random.value * (data.Length - 1));
            Vector3 posi = data[id];
            float mag = Vector3.SqrMagnitude(pos - posi);
            weight += Mathf.Exp(-mag) * data[id].w;
        }
        return weight;
    }

    //제안 분포
    public Vector3 GenerateRandomPointStandard()
    {
        var x = rand_gaussian(0f, 1f);
        var y = rand_gaussian(0f, 1f);
        var z = rand_gaussian(0f, 1f);
        return new Vector3(x, y, z);
    }
    //표준 정규 분포
    //sin(2*PI*x)*root(-2lnx)
    float rand_gaussian(float mu, float sigma)
    {
        float x = Random.value;
        
        float z = Mathf.Sqrt(-2.0f * Mathf.Log(x)) * Mathf.Sin(2.0f * Mathf.PI * x);
        return mu + sigma * z;
    }
}
