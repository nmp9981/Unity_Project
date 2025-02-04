using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent (typeof(MeshRenderer))]
public class DrawPolygonClass : MonoBehaviour
{
    [SerializeField]
    float size;

    public Mesh mesh;

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
    }
    void Start()
    {
        MakeMesh();
    }
   
    void MakeMesh()
    {
        //메쉬 인스턴스
        mesh = new Mesh();

        float hsize = size*0.5f;

        //사각형 정점 데이터
        Vector3[] vertices = new Vector3[]
        {
            new Vector3 (hsize, hsize, 0),//0번
            new Vector3 (hsize, -hsize, 0),//1번
            new Vector3 (-hsize, -hsize, 0),//2번
            new Vector3 (-hsize, hsize, 0)//3번
        };

        //사각형 UV 데이터
        Vector2[] UVs = new Vector2[]
        {
            new Vector2 (0, 0),//0번
            new Vector2 (1, 0),//1번
            new Vector2 (1, 1),//2번
            new Vector2 (0,1)//3번
        };

        //사각형 법선 벡터
        Vector3[] normals = new Vector3[]
        {
            new Vector3 (0,0,-1),//0번
            new Vector3 (0,0,-1),//1번
            new Vector3 (0,0,-1),//2번
            new Vector3 (0,0,-1)//3번
        };

        //삼각형 데이터 생성(사각형이므로 삼각형은 2개)
        //index를 삼각형 순서대로
        //반드시 시계방향으로 설정(미충족시 그려지지 않음)
        int[] triangles = new int[]
        {
            0,1,2,//1번 삼각형
            2,3,0//2번 삼각형
        };

        //메시 인스턴스 설정
        mesh.vertices = vertices;
        //mesh.uv = UVs;
        mesh.normals = normals;
        mesh.triangles = triangles;

        //만들어진 메시의 경계영역 계산(culling에 필요)
        mesh.RecalculateBounds();
        //삼각면과 정점으로부터 메쉬 노멀 재계산
        mesh.RecalculateNormals();

        //메시필터로 만든 메시 그리기
        meshFilter.mesh = mesh;
    }
}
