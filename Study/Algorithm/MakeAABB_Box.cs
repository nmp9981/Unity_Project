using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeAABB : MonoBehaviour
{
    private MeshRenderer meshRendeer;
    public Vector3[] meshPoints;

    private Vector3 AABBMinPos;
    private Vector3 AABBMaxPos;

    public Vector3 minPos;
    public Vector3 maxPos;

    public bool isCollide = false;
    private void Awake()
    {
        meshRendeer = GetComponent<MeshRenderer>();
        meshPoints = gameObject.GetComponent<MeshFilter>().mesh.vertices;
        MakeAABB_Box();
    }
    /// <summary>
    /// 기능 : AABB박스 제작
    /// 1) 원점을 기준으로 AABB박스 생성
    /// 2) 원점 기준으로 AABB 박스의 꼭짓점을 구한다
    /// </summary>
    void MakeAABB_Box()
    { 
        AABBMinPos = meshPoints[0];
        AABBMaxPos = meshPoints[0];

        foreach (Vector3 vec in meshPoints)
        {
            if (vec.x <= AABBMinPos.x && vec.y <= AABBMinPos.y && vec.z <= AABBMinPos.z)
            {
                AABBMinPos = vec;
            }
            if (vec.x >= AABBMaxPos.x && vec.y >= AABBMaxPos.y && vec.z >= AABBMaxPos.z)
            {
                AABBMaxPos = vec;
            }
        }
    }
    /// <summary>
    /// 기능 : 실제 AABB 꼭짓점 좌표로 변환
    /// </summary>
    void ChangeAABBPosition()
    {
        minPos = AABBMinPos+ gameObject.transform.position;
        maxPos = AABBMaxPos+ gameObject.transform.position;
    }
    private void Update()
    {
        ChangeAABBPosition();
        if (isCollide)
        {
            meshRendeer.material.color = Color.red;
        }
        else
        {
            meshRendeer.material.color = Color.blue;
        }
    }
}
