using System.Collections;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEngine;

public class GJKAlgorithm : MonoBehaviour
{
    [SerializeField]
    MakeAABB objA;
    [SerializeField]
    MakeAABB objB;

    List<Vector3> meshPointSetList = new List<Vector3>();
    private void Update()
    {
        GJKCollideJudge();
    }
    /// <summary>
    /// 기능 : AABB충돌 판정
    /// </summary>
    bool AABBCollideJudge()
    {
        bool isXCollide =false;
        bool isYCollide = false;
        bool isZCollide = false;

        if (objA.maxPos.x > objB.minPos.x && objA.minPos.x < objB.maxPos.x)
        {
            isXCollide = true;
        }
        if (objA.maxPos.y > objB.minPos.y && objA.minPos.y < objB.maxPos.y)
        {
            isYCollide = true;
        }
        if (objA.maxPos.z > objB.minPos.z && objA.minPos.z < objB.maxPos.z)
        {
            isZCollide = true;
        }

        if(isXCollide && isYCollide && isZCollide)
        {
            return true;
        }
        else
        {
            objA.isCollide = false;
            objB.isCollide = false;
            return false;
        }
    }
    /// <summary>
    /// 기능 : 민코프스키 차 (A-B)
    /// </summary>
    void MincovskiDiff()
    {
       foreach(var vecA in objA.meshPoints)
        {
            foreach(var vecB in objB.meshPoints)
            {
                meshPointSetList.Add(vecA - vecB);
            }
        }
    }
    float Cross(Vector3 x, Vector3 y)
    {
        return x.x * y.y - x.y * y.x;
    }
    /// <summary>
    /// 원점 포함 여부
    /// </summary>
    bool IsIncludeOriginPoint(List<Vector3> meshPointSetList)
    {
        for (int i = 2; i < meshPointSetList.Count; i++)
        {
            //세 점
            Vector3 Point1 = meshPointSetList[0];
            Vector3 Point2 = meshPointSetList[1];
            Vector3 Point3 = meshPointSetList[i];

            //삼각형 내부 점 판정
            //외적의 부호가 모두 같으면 삼각형 내에 있다.
            if (Cross(Point1, Point2) >= 0 && Cross(Point2, Point3) >= 0 && Cross(Point3, Point1) >= 0)
            {
                return true;
            }
            if (Cross(Point1, Point2) <= 0 && Cross(Point2, Point3) <= 0 && Cross(Point3, Point1) <= 0)
            {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// GJK 충돌 판정
    /// 1) AABB충돌이 일어났는지 검사, 충돌이 일어난 경우에만 GJK추가 충돌 판정
    /// 2) 두 메시 좌표의 민코프스키 차
    /// 3) 민코프스키 차집합이 원점을 포함하는가?
    /// </summary>
    void GJKCollideJudge()
    {
        if (AABBCollideJudge())
        {
            MincovskiDiff();
            if (IsIncludeOriginPoint(meshPointSetList))
            {
                objA.isCollide = true;
                objB.isCollide = true;
            }
            else
            {
                objA.isCollide = false;
                objB.isCollide = false;
            }
        }
    }

}
