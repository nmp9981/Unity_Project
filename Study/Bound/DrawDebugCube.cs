public static void DrawDebugCube(Vector3 center, Vector3 size, Color color)
{
    // 큐브의 8개의 꼭짓점 계산
    Vector3[] vertices = new Vector3[8];
    vertices[0] = center + new Vector3(-size.x, -size.y, -size.z) * 0.5f;
    vertices[1] = center + new Vector3(size.x, -size.y, -size.z) * 0.5f;
    vertices[2] = center + new Vector3(size.x, -size.y, size.z) * 0.5f;
    vertices[3] = center + new Vector3(-size.x, -size.y, size.z) * 0.5f;
    vertices[4] = center + new Vector3(-size.x, size.y, -size.z) * 0.5f;
    vertices[5] = center + new Vector3(size.x, size.y, -size.z) * 0.5f;
    vertices[6] = center + new Vector3(size.x, size.y, size.z) * 0.5f;
    vertices[7] = center + new Vector3(-size.x, size.y, size.z) * 0.5f;

    // 큐브의 12개의 간선을 그리기 위한 인덱스
    int[,] edges = new int[12, 2]
    {
    { 0, 1 }, { 1, 2 }, { 2, 3 }, { 3, 0 },
    { 4, 5 }, { 5, 6 }, { 6, 7 }, { 7, 4 },
    { 0, 4 }, { 1, 5 }, { 2, 6 }, { 3, 7 }
    };

    // Debug.DrawLine을 사용하여 간선을 그림
    for (int i = 0; i < edges.GetLength(0); i++)
    {
        Debug.DrawLine(vertices[edges[i, 0]], vertices[edges[i, 1]], color);
    }
}
