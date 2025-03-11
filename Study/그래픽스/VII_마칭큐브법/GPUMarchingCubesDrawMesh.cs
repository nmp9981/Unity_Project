using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUMarchingCubesDrawMesh : MonoBehaviour
{
    #region public
    public int segmentNum = 32;//격자 분해 개수
    
    [Range(0, 1)]
    public float threashold = 0.5f;//역치, 메시화할 스칼라값의 임계치

    public Material mat;//렌더링용 머테리얼

    public Color DiffuseColor = Color.green;//디퓨즈 색
    public Color EmissionColor = Color.black; //발광색
    public float EmissionIntensity = 0;//발광색 세기

    [Range(0, 1)]
    public float metallic = 0;//재질
    [Range(0, 1)]
    public float glossiness = 0.5f;//광택
    #endregion
    
    #region private
    int vertexMax = 0;//정점 수
    Mesh[] meshs = null;//메시 배열
    Material[] materials = null;//각 메시마다 머테리얼 배열
    float renderScale = 1f / 32f;//표시 스케일
    MarchingCubesDefines mcDefines = null;//마칭큐브용 정수 배열
    #endregion

    //초기화
    void Initialize()
    {
        //총 정점 필요 개수 (32^3)
        vertexMax = segmentNum * segmentNum * segmentNum;
        Debug.Log("VertexMax " + vertexMax);
        //1cube 크기로 나눠서 렌더링시 크기 결정
        renderScale = 1f / segmentNum;
        //메시 제작
        CreateMesh();
        //쉐이더에서 사용할 마칭 큐브용 정수 배열 초기화
        mcDefines = new MarchingCubesDefines();
    }
    //메시 제작
    void CreateMesh()
    {
        //의
        int vertNum = 65535;
        int meshNum = Mathf.CeilToInt((float)vertexMax / vertNum); 
        Debug.Log("meshNum " + meshNum);
        meshs = new Mesh[meshNum];
        materials = new Material[meshNum];
        //의
        Bounds bounds = new Bounds(
         transform.position,
         new Vector3(segmentNum, segmentNum, segmentNum) * renderScale
         );
        int id = 0;
        for (int i = 0; i < meshNum; i++)
        {
            //
            Vector3[] vertices = new Vector3[vertNum];
            int[] indices = new int[vertNum];
            for (int j = 0; j < vertNum; j++)
            {
                vertices[j].x = id % segmentNum;
                vertices[j].y = (id / segmentNum) % segmentNum;
                vertices[j].z = (id / (segmentNum * segmentNum)) % segmentNum;
                indices[j] = j;
                id++;
            }
            //
            meshs[i] = new Mesh();
            meshs[i].vertices = vertices;
            // d 
            meshs[i].SetIndices(indices, MeshTopology.Points, 0);
            meshs[i].bounds = bounds;
            materials[i] = new Material(mat);
        }
    }
}
