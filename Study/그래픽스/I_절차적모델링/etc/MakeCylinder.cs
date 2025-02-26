#region 메시로 실린더 그리기
//실린더 그리기
void MakeCylinder(Vector3 startPos, Vector3 endPos, float radius)
{
    List<Vector3> vertices = new List<Vector3>();
    List<Vector2> uvs = new List<Vector2>();
    List<Vector3> normals = new List<Vector3>();
    List<int> triangles = new List<int>();
    int segments = 8;

    //빈 게임 오브젝트 생성
    GameObject obj = new GameObject("Pipe");
    MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
    MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
    if (meshFilter == null)
    {
        obj.AddComponent<MeshFilter>();
        meshFilter = obj.GetComponent<MeshFilter>();
    }
    if (meshRenderer == null)
    {
        obj.AddComponent<MeshRenderer>();
        meshRenderer = obj.GetComponent<MeshRenderer>();
        meshRenderer.material = material;
    }

    //측면
    //측면 정점 설정
    vertices = GenerateCap(segments + 1, startPos, endPos, radius, uvs, normals);
    //측면 만들기
    triangles = MakeSideTriangleIndex(segments);

    Mesh mesh = new Mesh();
    //메시 인스턴스 설정
    mesh.vertices = vertices.ToArray();
    //mesh.uv = uvs.ToArray();
    //mesh.normals = normals.ToArray();
    mesh.triangles = triangles.ToArray();

    //만들어진 메시의 경계영역 계산(culling에 필요)
    mesh.RecalculateBounds();
    //삼각면과 정점으로부터 메쉬 노멀 재계산
    mesh.RecalculateNormals();

    //메시필터로 만든 메시 그리기
    meshFilter.mesh = mesh;
}
/// <summary>
/// 측면 삼각형 인덱스 리스트 
/// </summary>
/// <param name="startPos"></param>
/// <param name="endPos"></param>
/// <param name="radius"></param>
/// <param name="segments"></param>
/// <returns></returns>
List<int> MakeSideTriangleIndex(int segments)
{
    //삼각형 정점 배열
    List<int> triangles = new List<int>();

    //위와 아래를 이어서 측면 구성
    int len = 2 * (segments + 1);
    for (int i = 0; i < segments + 1; i++)
    {
        //측면은 직사각형의 집합
        //각 정점 인덱스 설정
        int index = 2 * i;
        int a = index;
        int b = index + 1;
        int c = (index + 2) % len;
        int d = (index + 3) % len;

        //삼각형 1
        triangles.Add(b);
        triangles.Add(a);
        triangles.Add(c);
        //삼각형2
        triangles.Add(b);
        triangles.Add(c);
        triangles.Add(d);
    }
    return triangles;
}
/// <summary>
/// 측면 정점 설정
/// </summary>
/// <param name="segments"></param>
/// <param name="top"></param>
/// <param name="bottom"></param>
/// <param name="radius"></param>
/// <param name="vertices"></param>
/// <param name="uvs"></param>
/// <param name="normals"></param>
/// <param name="side"></param>
List<Vector3> GenerateCap(int segments, Vector3 startPos, Vector3 endPos, float radius,
         List<Vector2> uvs, List<Vector3> normals)
{
    List<Vector3> vertices = new List<Vector3>();

    //n각형 구성
    for (int i = 0; i < segments; i++)
    {
        //각도 비율(0~2PI)
        float ratio = (float)i / (segments - 1);
        float rad = 2 * Mathf.PI * ratio;//2PI

        //정점 위치
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        float x = radius * cos;
        float y = radius * sin;
        Vector3 tp = Vector3.zero;
        Vector3 bp = Vector3.zero;
        //방향에 따라 정점 충이 달라짐
        Vector3 diff = endPos - startPos;
        if (diff.x == 0)//z가 움직임
        {
            tp = new Vector3(startPos.x, y, x);
            bp = new Vector3(endPos.x, y, x);
        }
        else if (diff.z == 0)
        {
            tp = new Vector3(x, y, startPos.z);
            bp = new Vector3(x, y, endPos.z);
        }

        //정점 등록
        vertices.Add(tp);
        uvs.Add(new Vector2(ratio, 1));

        vertices.Add(bp);
        uvs.Add(new Vector2(ratio, 0));

        //측면
        //층면의 바깥쪽을 향하는 법선
        var normal = new Vector3(cos, 0f, sin);
        normals.Add(normal);
        normals.Add(normal);
    }
    return vertices;
}
#endregion 메시로 실린더 그리기
