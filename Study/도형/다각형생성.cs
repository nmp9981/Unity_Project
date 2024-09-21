using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class PolygonGernerater : MonoBehaviour
{
    [SerializeField]
    [Range(3, 100)]
    private int polygonPoints = 3;//다각형 점 개수
    [SerializeField]
    [Min(0.1f)]
    private float outerRadius = 3;//다각형 외곽 반지름
    [SerializeField]
    [Min(0f)]
    private float innerRadius;//다각형 내부 반지름

    private Mesh mesh;
    private Vector3[] verties;//다각형 점 배열
    private int[] indices;//정점을 잇는 폴리곤 정보 배열

    EdgeCollider2D edgeCollider;//다각형 충돌 범위

    
    void Awake()
    {
        mesh = new Mesh();

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        InnerCheck();
    }

    //내부 반지름 체크
    void InnerCheck()
    {
        //안쪽 반지름은 바깥쪽 반지름보다 작아야한다.
        innerRadius = innerRadius > outerRadius ? outerRadius - 0.1f : innerRadius;

        //꽉찬 다각형
        if (innerRadius == 0)
        {
            DrawFilled(polygonPoints, outerRadius);
        }//내부에 도형있음
        else
        {
            DrawHollow(polygonPoints, outerRadius, innerRadius);
        }
    }
    void DrawFilled(int sides, float radius)
    {
        //정점 정보
        verties = GetCircumferencePoints(sides, radius);
        //정점을 잇는 폴리곤 정보
        indices = DrawFilledIndics(verties);
        //메시 생성
        GerneralPolygon(verties,indices);
        //정점 정보를 바탕으로 충돌범위 생성
        edgeCollider.points = GetEdgePoints(verties);
    }
    void DrawHollow(int polygonPoints, float outerRadius, float innerRadius)
    {
        //바깥쪽 둘레 점 정보
        Vector3[] outerPoints = GetCircumferencePoints(polygonPoints,outerRadius);
        //안쪽 둘레 점 정보
        Vector3[] innerPoints = GetCircumferencePoints(polygonPoints, innerRadius);

        //두개의 배열 정보 저장하는 리스트
        List<Vector3> points = new List<Vector3>();
        points.AddRange(outerPoints);
        points.AddRange(innerPoints);

        //정점 정보
        verties = points.ToArray();
        //정점 잇는 폴리곤 정보
        indices = DrawHollowIndices(polygonPoints);
        //메시 생성
        GerneralPolygon(verties, indices);

        //정점 정보를 바탕으로 충돌 범위 생성
        List<Vector2> edgePoints = new List<Vector2>();
        edgePoints.AddRange(GetEdgePoints(outerPoints));
        edgePoints.AddRange(GetEdgePoints(innerPoints));
        edgeCollider.points = edgePoints.ToArray();
    }
    int[] DrawFilledIndics(Vector3[] vertices)
    {
        int triangleCount = vertices.Length - 2;//다각형 내 삼각형의 개수
        List<int> indics = new List<int>();

        for(int idx = 0; idx < triangleCount; ++idx)
        {
            indics.Add(0);//원의 중심
            indics.Add(idx + 2);
            indics.Add(idx + 1);
        }
        return indics.ToArray();
    }
    int[] DrawHollowIndices(int polygonPoints)
    {
        List<int> indics = new List<int>();

        for (int idx = 0; idx < polygonPoints; ++idx)
        {
            int outterIndex = idx;
            int innerIndex = idx + polygonPoints;

            indics.Add(outterIndex);
            indics.Add(innerIndex);
            indics.Add((outterIndex+1)%polygonPoints);

            indics.Add(innerIndex);
            indics.Add(polygonPoints+((innerIndex) + 1) % polygonPoints);
            indics.Add((outterIndex + 1) % polygonPoints);
        }
        return indics.ToArray();
    }

    void GerneralPolygon(Vector3[] verties,int[] indices)
    {
        //점 반지름 정보에 따라 지속적으로 업데이트
        // 초기화 로직
        mesh.Clear();
        mesh.vertices = verties;
        mesh.triangles = indices;

        //Bound, normal 재연산
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    /// <summary>
    /// 기능 : 원의 둘레에 있는 정점들 리스트 반환 
    /// </summary>
    Vector3[] GetCircumferencePoints(int sides, float radius)
    {
        Vector3[] points = new Vector3[sides];
        float angleStep = 2 * Mathf.PI * ((float)1/sides);//n각형의 내각

        for(int idx = 0; idx < sides; ++idx)
        {
            Vector2 point = Vector2.zero;
            float angle = angleStep * idx;

            point.x = radius * Mathf.Cos(angle);
            point.y = radius * Mathf.Sin(angle);
            points[idx] = point;
        }
        return points;
    }
    /// <summary>
    /// 기능 : 충돌 포인트를 받아옴
    /// 배열 크기를 1 추가함으로서 닫힌 형태가 되게한다.
    /// </summary>
    Vector2[] GetEdgePoints(Vector3[] verties)
    {
        Vector2[] points = new Vector2[verties.Length + 1];
        for(int idx = 0; idx < verties.Length; ++idx)
        {
            points[idx] = verties[idx];
        }
        points[^1] = verties[0];
        return points;
    }
}
