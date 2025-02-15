using ProceduralModeling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MyTubular : MonoBehaviour
{
    //내가 설정한 정점
    public List<Vector3> curvePoints = new List<Vector3>();

    public Mesh mesh;

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;

    //실린더 개수, 측면 n각형
    [SerializeField, Range(2, 50)] protected int tubularSegments, radialSegments = 8;
    //반지름
    [SerializeField, Range(0.1f, 5f)] protected float radius = 0.5f;
    //폐곡선 여부
    [SerializeField] protected bool closed = false;

    List<Vector3> vertices = new List<Vector3>();//정점
    List<Vector2> uvs = new List<Vector2>();
    List<int> triangles = new List<int>();

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Start()
    {
        MakeTubular();
    }
    void MakeTubular()
    {
        tubularSegments = curvePoints.Count;
        //Tubular의 정점 데이터를 생성
        for (int i = 0; i < tubularSegments; i++)
        {
            GenerateSegment(i);
        }

        //폐곡선이면 시자과 끝이 이어지도록 마지막 정점을 곡선 시작부분에 배치
        if (closed)
        {
            GenerateSegment(0);
        }

        //곡선의 시작점을 향하는 uv좌표 작성
        //for (int i = 0; i < tubularSegments; i++)
        //{
        //    for (int j = 0; j < radialSegments; j++)
        //    {
        //        float u = (float)j / radialSegments;
        //        float v = (float)i / tubularSegments;
        //        uvs.Add(new Vector2(u, v));
        //    }
        //}

        //측면 구성 -> 삼각형 인덱스
        for (int j = 0; j < tubularSegments-1; j++)//몇번 실린더
        {
            for (int i = 0; i < radialSegments; i++)//각 실린더의 n각형
            {
                int a = j * (radialSegments + 1)+i;
                int b = j * (radialSegments + 1)+i+1;
                int c = (j+1) * (radialSegments + 1)+i;
                int d = (j+1) * (radialSegments + 1)+i+1;

                triangles.Add(a); triangles.Add(b); triangles.Add(c);
                triangles.Add(c); triangles.Add(b); triangles.Add(d);
            }
        }

        //메시 인스턴스 설정
        mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        //mesh.uv = uvs.ToArray();
        mesh.triangles = triangles.ToArray();

        //만들어진 메시의 경계영역 계산(culling에 필요)
        mesh.RecalculateBounds();
        //삼각면과 정점으로부터 메쉬 노멀 재계산
        mesh.RecalculateNormals();

        //메시필터로 만든 메시 그리기
        meshFilter.mesh = mesh;
    }

    /// <summary>
    /// 정점 생성, n번 실린더
    /// </summary>
    /// <param name="n">실린더 번호, 중심점</param>
    void GenerateSegment(int n)
    {
        for(int idx = 0; idx <= radialSegments; idx++)
        {
            //0~2PI
            float PI2 = 2 * Mathf.PI;
            float rad = (float)idx / radialSegments * PI2;

            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);
            var newPos = curvePoints[n] + new Vector3(radius * cos, 0, radius * sin);
            vertices.Add(newPos);
        }
    }
}
