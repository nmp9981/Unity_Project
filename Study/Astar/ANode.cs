using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;

public class ANode
{
    //걷는게 가능한가?
    public bool isWalkAble;
    //노드 위치
    public Vector3 worldPos;
    //그리드상 위치
    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;
    public ANode parentNode;

    //생성자
    public ANode(bool nisWalkAble, Vector3 nWorldPos, int ngridX, int nGridY)
    {
        isWalkAble = nisWalkAble;
        worldPos = nWorldPos;
        gridX = ngridX;
        gridY = nGridY;
    }
    //FCost 구하기
    public int fCost
    {
        get { return gCost + hCost; }
    }
}
