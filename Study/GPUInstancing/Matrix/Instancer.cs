using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instancer : MonoBehaviour
{
    public int Instances;

    public Mesh mesh;

    public Material[] Materials;
    private List<List<Matrix4x4>> Batches = new List<List<Matrix4x4>>();

    //실제 렌더링
    private void RenderBatches()
    {
        foreach (var Batch in Batches){//4*4 행렬
            for(int i = 0; i < mesh.subMeshCount; i++)//각 하위 메쉬 통과
            {
                Graphics.DrawMeshInstanced(mesh, i, Materials[i], Batch);//실제 그리기
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        int AddedMatricies = 0;
        for(int i = 0; i < Instances; i++)
        {
            if(AddedMatricies<500 && Batches.Count != 0)//배치 추가
            {
                //행렬 목록 생성
                Batches[Batches.Count - 1].Add(Matrix4x4.TRS(pos:new Vector3(x:Random.Range(0,50), y: Random.Range(0, 50), z: Random.Range(0, 50)),
                    Random.rotation, s:new Vector3(1,1,1)));
                AddedMatricies++;
            }
            else//초기화
            {
                Batches.Add(new List<Matrix4x4>());
                AddedMatricies = 0;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        RenderBatches();
    }
}
