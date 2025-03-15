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

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        //즉석에서 그려지지 않고 렌더링 처리에 우선 등록함
        RenderMesh();
    }

    //버퍼 초기화
    private void OnDestroy()
    {
        mcDefines.ReleaseBuffer();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }

    //초기화 -> 쉐이더에서는 리터럴값이 4096개가 한계이므로 여기서 초기화
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
        //Mesh의 정점 개수는 65535가 상한, Mesh분해
        int vertNum = 65535;
        //분해 하는 메시의 수
        int meshNum = Mathf.CeilToInt((float)vertexMax / vertNum);
        Debug.Log("meshNum " + meshNum);
        
        //배열 할당
        meshs = new Mesh[meshNum];
        materials = new Material[meshNum];
        
        //메시 Bounds계산
        //현재 위치 + 격자 크기만큼
        Bounds bounds = new Bounds(
         transform.position,
         new Vector3(segmentNum, segmentNum, segmentNum) * renderScale
         );

        int id = 0;
        for (int i = 0; i < meshNum; i++)
        {
            //정점 작성
            Vector3[] vertices = new Vector3[vertNum];
            //정점 번호 저장
            int[] indices = new int[vertNum];
            for (int j = 0; j < vertNum; j++)
            {
                vertices[j].x = id % segmentNum;
                vertices[j].y = (id / segmentNum) % segmentNum;
                vertices[j].z = (id / (segmentNum * segmentNum)) % segmentNum;
                indices[j] = j;
                id++;
            }
            //Mesh 작성
            meshs[i] = new Mesh();
            meshs[i].vertices = vertices;
            //GeometryShader로 폴리곤을 만드므로 point가 좋다
            meshs[i].SetIndices(indices, MeshTopology.Points, 0);
            meshs[i].bounds = bounds;

            materials[i] = new Material(mat);
        }
    }
    //렌더링 부분
    void RenderMesh()
    {
        Vector3 halfSize = new Vector3(segmentNum, segmentNum, segmentNum) * renderScale * 0.5f;
        //이동, 회전, 크기 행렬 정의
        Matrix4x4 trs = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);

        //computeBuffer들을 쉐이더에 넘김 넘기기
        for (int i = 0; i < meshs.Length; i++)
        {
            materials[i].SetPass(0);
            materials[i].SetInt("_SegmentNum", segmentNum);
            materials[i].SetFloat("_Scale", renderScale);
            materials[i].SetFloat("_Threashold", threashold);
            materials[i].SetFloat("_Metallic", metallic);
            materials[i].SetFloat("_Glossiness", glossiness);
            materials[i].SetFloat("_EmissionIntensity", EmissionIntensity);
            materials[i].SetVector("_HalfSize", halfSize);
            materials[i].SetColor("_DiffuseColor", DiffuseColor);
            materials[i].SetColor("_EmissionColor", EmissionColor);
            materials[i].SetMatrix("_Matrix", trs);
            Graphics.DrawMesh(meshs[i], Matrix4x4.identity, materials[i], 0);
        }
    }
}
