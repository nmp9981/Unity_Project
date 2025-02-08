void MakePlane()
{
    mesh = new Mesh();
    List<Vector3> vertices = new List<Vector3>();
    List<Vector2> uvs = new List<Vector2>();
    List<Vector3> normals = new List<Vector3>();
    //플레인 크기
    int width = 10;
    int height = 8;
    //정점 위치
    for (int z = 0; z < height; z++)
    {
        //세로출 각 정점 위치 비율 (높이가 3이면 정점 인덱스는 0~2)
        float rz = (float) z / (height-1);
        for (int x = 0; x < width; x++)
        {
            //가로줄 각 정점 위치 비율 (너비가 10이면 정점 인덱스는 0~9)
            float rx = (float)z / (width - 1);
            iny y = x;//여기서 높이 설정
            vertices.Add(new Vector3(x, y, z));
            uvs.Add(new Vector2(rx, rz));
            normals.Add(new Vector3(0, 1, 0));
        }
    }
    //삼각형 인덱스
    List<int> triangles = new List<int>();
    for (int z = 0; z < height-1; z++)
    {
        for (int x = 0; x < width-1; x++)
        {
            //기준 정점
            int index = x + z * width;
            //4개 정점 구하기
            int a = index;
            int b = index + 1;
            int c = index + width+1;
            int d = index + width;
            //역시 시계방향으로 인덱스를 넣어준다.
            triangles.Add(a);
            triangles.Add(d);
            triangles.Add(c);
            triangles.Add(a);
            triangles.Add(c);
            triangles.Add(b);
        }
    }
    //메시 인스턴스 설정
    mesh.vertices = vertices.ToArray();
    mesh.uv = uvs.ToArray();
    mesh.normals = normals.ToArray();
    mesh.triangles = triangles.ToArray();
    //만들어진 메시의 경계영역 계산(culling에 필요)
    mesh.RecalculateBounds();
    //삼각면과 정점으로부터 메쉬 노멀 재계산
    mesh.RecalculateNormals();
    //메시필터로 만든 메시 그리기
    meshFilter.mesh = mesh;
}
