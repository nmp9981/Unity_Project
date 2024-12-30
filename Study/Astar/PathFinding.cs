using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    AGrid grid;

    //시작, 목표 지점
    public Transform StartObject;
    public Transform TargetObject;

    private void Awake()
    {
        grid = GetComponent<AGrid>();
    }
   
    void Update()
    {
        FindPath(StartObject.position, TargetObject.position);
    }
    /// <summary>
    /// 기능 : 시작 노드~타겟 노드까지의 길찾기
    /// </summary>
    /// <param name="startPos">시작 지점</param>
    /// <param name="targetPos">목표 지점</param>
    void FindPath(Vector2 startPos, Vector3 targetPos)
    {
        ANode startNode = grid.GetNodeFromWorldPoint(startPos);
        ANode targetNode = grid.GetNodeFromWorldPoint(targetPos);

        List<ANode> openList = new List<ANode>();
        HashSet<ANode> closeList = new HashSet<ANode>();
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            ANode currentNode = openList[0];//현재 노드

            //열린 목록에 FCost가 가장 작은 노드를 찾는다. 같을 시 HCost가 작은것 선택
            for(int i = 1; i < openList.Count; i++)
            {
                if (openList[i].fCost < currentNode.fCost || 
                    (openList[i].fCost == currentNode.fCost && openList[i].hCost < currentNode.hCost))
                {
                    currentNode = openList[i];
                }
            }
            //탐색된 노드는 열린목록에서 제거하고 닫힌목록으로 이동
            openList.Remove(currentNode);
            closeList.Add(currentNode);

            //탐색노드가 목표노드라면 탐색종료
            if(currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }

            //이웃노드 계속 탐색
            List<ANode> gridNeighbourList = grid.GetNeighbours(currentNode);
            foreach (ANode n in gridNeighbourList)
            {
                //이동불가거나 끝난 목록에 잇는 노드는 스킵
                if(!n.isWalkAble || closeList.Contains(n))
                {
                    continue;
                }

                //이웃 노드들의 GCost, HCost를 계산하여 열린목록에 추가한다. (현재와 이웃노드)
                int newCurrentToNeighbourCost = currentNode.gCost + GetDistanceCost(currentNode,n);
                //이웃노드의 Gcost가 더 작거나 열린 노드에 없으면
                if(newCurrentToNeighbourCost < n.gCost || !openList.Contains(n))
                {
                    n.gCost = newCurrentToNeighbourCost;
                    n.hCost = GetDistanceCost(n, targetNode);
                    n.parentNode = currentNode;

                    //한번도 탐색을 안함
                    if (!openList.Contains(n))
                    {
                        openList.Add(n);
                    }
                }
            }
        }
    }
    /// <summary>
    /// 탐색 종료 후 최종 노드의 ParentNode추적하여 리스트에 담는다.
    /// 도착 지점으로부터 역추적
    /// 시작 지점 ~ 끝 지점
    /// </summary>
    /// <param name="startNode">시작 노드</param>
    /// <param name="targetNode">목표 노드</param>
    void RetracePath(ANode startNode, ANode targetNode)
    {
        List<ANode> path = new List<ANode>();
        ANode currentNode = targetNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parentNode;
        }
        path.Reverse();
        //순차 정렬된 경로 리스트
        grid.path = path;
    }
    /// <summary>
    /// 두 노드간 거리로 Cost계산
    /// 대각선 이동 : 14, x,y,축 1칸 이동 : 10
    /// </summary>
    /// <param name="ANode">A노드</param>
    /// <param name="BNode">B노드</param>
    /// <returns>Cost</returns>
    int GetDistanceCost(ANode ANode, ANode BNode)
    {
        int distX = Mathf.Abs(ANode.gridX - BNode.gridX);
        int distY = Mathf.Abs(ANode.gridY - BNode.gridY);

        if (distX > distY)
        {
            return 14 * distY + 10 * (distX - distY);
        }
        return 14 * distX + 10 * (distY - distX);
    }
}
