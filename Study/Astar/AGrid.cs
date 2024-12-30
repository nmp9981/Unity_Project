using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AGrid : MonoBehaviour
{
    //장애물 레이어
    public LayerMask unWalkableMask;
    //그리드 영역 전체 사이즈
    public Vector2 gridWorldSize;
    //노드 반지름
    public float nodeRadius;
    //그리드 배열
    ANode[,] grid;
    //경로
    public List<ANode> path;

    //노드 지름
    float nodeDiameter;
    //그리드 전체 x,y 개수(길이)
    int gridSizeX;
    int gridSizeY;

    void Start()
    {
        nodeDiameter = 2 * nodeRadius;//노드 지름
        //그리드 x,y개수(길이) 구하기
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }
    /// <summary>
    /// 기능 : 그리드 제작
    /// </summary>
    void CreateGrid()
    {
        //그리드 배열 할당
        grid = new ANode[gridSizeX, gridSizeY];
        
        //그리드 원점(가장 자리 꼭짓점)
        Vector3 worldBottomLeft = transform.position 
            - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        //그리드 생성
        Vector3 worldPoint;
        for (int x = 0; x < gridSizeX; x++)
        {
            for(int y = 0; y < gridSizeY; y++)
            {
                //그리드 생성 위치
                worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) 
                    + Vector3.forward * (y * nodeDiameter + nodeRadius);
                //벽과 충돌 체크, 이동 가능 노드인지 확인
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unWalkableMask));
                grid[x, y] = new ANode(walkable, worldPoint,x,y);
            }
        }
    }
    /// <summary>
    /// 기능 : 노드의 8방향 주변 노드를 찾는 함수
    /// </summary>
    /// <param name="node">기준 노드</param>
    /// <returns>근처 8방향 노드</returns>
    public List<ANode> GetNeighbours(ANode node)
    {
        List<ANode> neighbours = new List<ANode>();
        for(int x = -1; x <= 1; x++)
        {
            for(int y = -1; y <= 1; y++)
            {
                //자기 자신일 경우 스킵
                if (x == 0 && y == 0) continue;

                //그리드상의 위치를 기준으로 체크
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                //x,y값 그리드 범위 내에 있는 경우만 포함
                if(checkX>=0 && checkX<gridSizeX && checkY>=0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }
    /// <summary>
    /// unity의 월드좌표로부터 그리드상의 노드를 찾는 함수
    /// </summary>
    /// <param name="worldPositon">월드 좌표</param>
    /// <returns>변환 후 그리드상 노드</returns>
    public ANode GetNodeFromWorldPoint(Vector3 worldPositon)
    {
        float percentX = (worldPositon.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPositon.z + gridWorldSize.y / 2) / gridWorldSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    /// <summary>
    /// 기능 : 그리드 그리기
    /// </summary>
    private void OnDrawGizmos()
    {
        //큐브 생성
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (grid != null)
        {
            foreach(ANode n in grid)
            {
                Gizmos.color = (n.isWalkAble) ? Color.blue : Color.red;
                if (path != null)
                {
                    //경로
                    if (path.Contains(n))
                    {
                        Gizmos.color = Color.black;
                    }
                    Gizmos.DrawCube(n.worldPos, Vector3.one*(nodeDiameter-0.1f));
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
