using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class CubesMarching : MonoBehaviour
{
    [SerializeField] private int width = 30;
    [SerializeField] private int height = 10;

    //디버그용, 각 지점 표시용
    [SerializeField] float resolution = 1;
    //난수 범위
    [SerializeField] float noiseScale = 1;

    //역치 값
    [SerializeField] private float heightTresshold = 0.5f;

    [SerializeField] bool visualizeNoise;
    [SerializeField] bool use3DNoise;

    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();

    //각 좌표에서 표면까지의 거리
    private float[,,] heights;

    private MeshFilter meshFilter;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        StartCoroutine(TestAll());
    }

    private IEnumerator TestAll()
    {
        while (true)
        {
            SetHeights();
            MarchCubes();
            SetMesh();
            yield return new WaitForSeconds(1f);
        }
    }

    //메시 세팅
    private void SetMesh()
    {
        Mesh mesh = new Mesh();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }
    /// <summary>
    /// 높이 세팅
    /// </summary>
    private void SetHeights()
    {
        heights = new float[width + 1, height + 1, width + 1];

        for (int x = 0; x < width + 1; x++)
        {
            for (int y = 0; y < height + 1; y++)
            {
                for (int z = 0; z < width + 1; z++)
                {
                    if (use3DNoise)
                    {
                        float currentHeight = PerlinNoise3D((float)x / width * noiseScale, (float)y / height * noiseScale, (float)z / width * noiseScale);

                        heights[x, y, z] = currentHeight;
                    }
                    else
                    {
                        //현재 높이, (x,z)에서의 노이즈 생성, (x,z)에서 +-1
                        float currentHeight = height * Mathf.PerlinNoise(x * noiseScale, z * noiseScale);
                        //표면까지의 거리
                        float distToSufrace;

                        //거리 함수
                        if (y <= currentHeight - 0.5f)
                            distToSufrace = 0f;
                        else if (y > currentHeight + 0.5f)
                            distToSufrace = 1f;
                        else if (y > currentHeight)
                            distToSufrace = y - currentHeight;
                        else
                            distToSufrace = currentHeight - y;

                        //표면까지의 거리 저장
                        heights[x, y, z] = distToSufrace;
                    }
                }
            }
        }
    }

    private float PerlinNoise3D(float x, float y, float z)
    {
        float xy = Mathf.PerlinNoise(x, y);
        float xz = Mathf.PerlinNoise(x, z);
        float yz = Mathf.PerlinNoise(y, z);

        float yx = Mathf.PerlinNoise(y, x);
        float zx = Mathf.PerlinNoise(z, x);
        float zy = Mathf.PerlinNoise(z, y);

        return (xy + xz + yz + yx + zx + zy) / 6;
    }

    //역치 계산 -> 삼각형 폴리곤 인덱스를 가져옴(0~255)
    private int GetConfigIndex(float[] cubeCorners)
    {
        int configIndex = 0;

        for (int i = 0; i < 8; i++)
        {
            if (cubeCorners[i] > heightTresshold)
            {
                configIndex |= 1 << i;
            }
        }

        return configIndex;
    }

    /// <summary>
    /// 마칭 큐브 생성
    /// </summary>
    private void MarchCubes()
    {
        vertices.Clear();
        triangles.Clear();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < width; z++)
                {
                    float[] cubeCorners = new float[8];//각 큐브 상태

                    for (int i = 0; i < 8; i++)
                    {
                        //꼭짓점 위치 찾기
                        Vector3Int corner = new Vector3Int(x, y, z) + MarchingTable.Corners[i];
                        //거리 등록
                        cubeCorners[i] = heights[corner.x, corner.y, corner.z];
                    }

                    MarchCube(new Vector3(x, y, z), cubeCorners);
                }
            }
        }
    }

    /// <summary>
    /// 마칭 큐브 그리기
    /// </summary>
    /// <param name="position"></param>
    /// <param name="cubeCorners"></param>
    private void MarchCube(Vector3 position, float[] cubeCorners)
    {
        int configIndex = GetConfigIndex(cubeCorners);

        if (configIndex == 0 || configIndex == 255)
        {
            return;
        }

        int edgeIndex = 0;
        for (int t = 0; t < 5; t++)//최대 삼각형 5개
        {
            for (int v = 0; v < 3; v++)//삼각형 정점
            {
                //삼각형 정점 : 삼각형 폴리곤 번호, 정점 번호
                int triTableValue = MarchingTable.Triangles[configIndex, edgeIndex];

                //그리기 불가
                if (triTableValue == -1)
                {
                    return;
                }

                //모서리 잇기
                Vector3 edgeStart = position + MarchingTable.Edges[triTableValue, 0];
                Vector3 edgeEnd = position + MarchingTable.Edges[triTableValue, 1];

                Vector3 vertex = (edgeStart + edgeEnd) / 2;

                vertices.Add(vertex);
                triangles.Add(vertices.Count - 1);

                edgeIndex++;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!visualizeNoise || !Application.isPlaying)
        {
            return;
        }

        for (int x = 0; x < width + 1; x++)
        {
            for (int y = 0; y < height + 1; y++)
            {
                for (int z = 0; z < width + 1; z++)
                {
                    Gizmos.color = new Color(heights[x, y, z], heights[x, y, z], heights[x, y, z], 1);
                    Gizmos.DrawSphere(new Vector3(x * resolution, y * resolution, z * resolution), 0.2f * resolution);
                }
            }
        }
    }
}
