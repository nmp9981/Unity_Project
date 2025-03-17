using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using komietty.Math;

public class DemoMCMC : MonoBehaviour
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

    //클래스
    Metropolis3d metropolis;

    void Start()
    {
        data = new Vector4[lEdge * lEdge * lEdge];
        Prepare();
        metropolis = new Metropolis3d(data, lEdge * Vector3.one);
        StartCoroutine(Generate());
    }

    //목표 분포 : Simplex Noise
    void Prepare()
    {
        var sn = new SimplexNoiseGenerator();
        // ledge크기의 정육면체 영역
        for (int x = 0; x < lEdge; x++)
            for (int y = 0; y < lEdge; y++)
                for (int z = 0; z < lEdge; z++)
                {
                    var i = x + lEdge * y + lEdge * lEdge * z;
                    var val = sn.noise(x, y, z);
                    data[i] = new Vector4(x, y, z, val);
                }
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
            foreach (var pos in metropolis.Chain(nInitialize, nlimit, threshold))
            {
                //물체를 pos위치에 생성
                Instantiate(prefab, pos, Quaternion.identity);
            }
        }
    }
}
