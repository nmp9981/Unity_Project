using Autodesk.Fbx;
using ProceduralModeling;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Tubular : MonoBehaviour
{
    public Mesh mesh;

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;

    [SerializeField] protected CatmullRomCurve curve;

    [SerializeField, Range(2, 50)] protected int tubularSegments = 20, radialSegments = 8;
    [SerializeField, Range(0.1f, 5f)] protected float radius = 0.5f;
    [SerializeField] protected bool closed = false;

    List<Vector3> vertices = new List<Vector3>();
    List<Vector3> normals = new List<Vector3>();
    List<Vector4> tangents = new List<Vector4>();
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
        //곡선으로부터 Frenet frame을 계산
        var frames = curve.ComputeFrenetFrames(tubularSegments, closed);

        //Tubular의 정점 데이터를 생성
        for (int i = 0; i < tubularSegments; i++)
        {
            GenerateSegment(curve,frames,vertices,normals,tangents,i);
        }

        //폐곡선이면 시자과 끝이 이어지도록 마지막 정점을 곡선 시작부분에 배치
        GenerateSegment(curve, frames, vertices, normals, tangents, (!closed) ? tubularSegments : 0);

        //곡선의 시작점을 향하는 uv좌표 작성
        for(int i = 0; i <= tubularSegments; i++)
        {
            for(int j = 0; j <= radialSegments; j++)
            {
                float u = (float) j/ radialSegments;
                float v = (float) i/ tubularSegments;
                uvs.Add(new Vector2(u, v));
            }
        }

        //측면 구성
        for (int j = 1; j <= tubularSegments; j++)
        {
            for (int i = 1; i <= radialSegments; i++)
            {
                int a = (radialSegments + 1) * (j - 1) + (i - 1);
                int b = (radialSegments + 1) * j + (i - 1);
                int c = (radialSegments + 1) * j + i;
                int d = (radialSegments + 1) * (j - 1) + i;
                triangles.Add(a); triangles.Add(d); triangles.Add(b);
                triangles.Add(b); triangles.Add(d); triangles.Add(c);
            }
        }

        //메시 인스턴스 설정
        mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.normals = normals.ToArray();
        mesh.tangents = tangents.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = triangles.ToArray();

        //만들어진 메시의 경계영역 계산(culling에 필요)
        mesh.RecalculateBounds();
        //삼각면과 정점으로부터 메쉬 노멀 재계산
        mesh.RecalculateNormals();

        //메시필터로 만든 메시 그리기
        meshFilter.mesh = mesh;
    }

    void GenerateSegment(CurveBase curve,List<FrenetFrame> frames,List<Vector3> vertices,
        List<Vector3> normals,List<Vector4> tangents,int index)
    {
        //uv생성
        float u = (float)index / tubularSegments;

        var p = curve.GetPointAt(u);
        var fr = frames[index];
        var N = fr.Normal;
        var B = fr.Binormal;

        for (int j = 0; j <= radialSegments; j++)
        {
            //0~2PI
            float PI2 = 2 * Mathf.PI;
            float rad = (float)j / radialSegments * PI2;

            //원주를 따라 균등하게 정점을 배치한다
            float cos = Mathf.Cos(rad), sin = Mathf.Sin(rad);
            var v = (cos * N + sin * B).normalized;//노말, 법선 벡터
            vertices.Add(p + radius * v);
            normals.Add(v);
            var tangent = fr.Tangent;
            tangents.Add(new Vector4(tangent.x, tangent.y, tangent.z, 0f));
        }
    }
        
}
