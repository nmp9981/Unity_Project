using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GJKAlgorithm
{
    public struct Point
    {
        public float x, y;
        public Point(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public class GJK
    {
        List<Point> simplexList = new();

        /// <summary>
        /// GJK 알고리즘
        /// </summary>
        public bool GJK_Algorithm(List<Point> shapeA, List<Point> shapeB)
        {
            //첫 두점 등록
            Vector3 dir = Vector3.right;
            Point p1 = Support(shapeA, shapeB, dir);
            Point p2 = Support(shapeA, shapeB, -dir);
            simplexList.Add(p1);
            simplexList.Add(p2);

            //다음 방향
            dir = VerticalOToLineAB(p1, p2);

            while (true)
            {
                //dir 방뱡으로 새점을 뽑는다.
                Point p = Support(shapeA, shapeB, dir);

                // 같은 점이 또 뽑혔으면 더 진행 못 함 → 충돌 없음
                if (Mathf.Approximately(p.x, p1.x) && Mathf.Approximately(p.y, p1.y)) return false;
                if (Mathf.Approximately(p.x, p2.x) && Mathf.Approximately(p.y, p2.y)) return false;

                //종료 조건(새로 봅은 점 P기준)
                float dot = p.x * (dir.x) + p.y * (dir.y);
                if (dot <= 0) return false;//충돌 X

                //원점 포함하면 충돌 -> EPA로
                if (IsContainTriangle(p1,p2,p))
                {
                    //EPA

                    return true;
                }
                else
                {
                    // 가정: simplex = [p1, p2, p], p가 새로 추가된 점
                    Vector3 ap = new Vector3(p1.x - p.x, p1.y - p.y, 0);  // P→A
                    Vector3 bp = new Vector3(p2.x - p.x, p2.y - p.y, 0);  // P→B
                    Vector3 po = new Vector3(-p.x, -p.y, 0);              // P→O

                    // AP의 바깥쪽 normal 방향(원점 쪽)
                    Vector3 apPerp = TripleCross(bp, ap, ap); // AB와 AP에 수직, B 반대 쪽

                    if (Vector3.Dot(apPerp, po) > 0)
                    {
                        // 원점이 AP 바깥쪽 → B 제거
                        p2 = p;
                        dir = VerticalOToLineAB(p1, p);
                    }
                    else
                    {
                        // 원점이 BP 바깥쪽 → A 제거
                        p1 = p;
                        dir = VerticalOToLineAB(p2, p);
                    }
                }
            }
        }

        /// <summary>
        /// Support 함수
        /// </summary>
        /// <param name="shape1"></param>
        /// <param name="shape2"></param>
        /// <returns></returns>
        Point Support(List<Point> shapeA, List<Point> shapeB, Vector3 dir)
        {
            Point a = FarthestInDirection(shapeA, dir);   // A에서 d 방향 최댓값
            Point b = FarthestInDirection(shapeB, -dir);  // B에서 -d 방향 최댓값
            return new Point(a.x - b.x, a.y - b.y);//민코브스키 차
        }

        /// <summary>
        /// 주어진 방향으로부터 가장 멀리 있는 점
        /// </summary>
        /// <param name="shape1"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        Point FarthestInDirection(List<Point> shapeList, Vector3 dir)
        {
            Point farPoint = new Point();
            float maxDot = -float.MaxValue;

            foreach (Point p in shapeList)
            {
                float dot = dir.x*p.x+ dir.y*p.y;
                //더 큰 내적값
                if(dot > maxDot)
                {
                    maxDot = dot;
                    farPoint = p;
                }
            }
            return farPoint;
        }

        /// <summary>
        /// 삼각형내에 원점이 포함하는가?
        /// </summary>
        /// <returns></returns>
        bool IsContainTriangle(Point p1, Point p2, Point p3)
        {
            //외적의 부호가 모두 같아야함
            if (CrossVector(p1,p3)>=0 && CrossVector(p3,p2) >= 0 && CrossVector(p2, p1) >= 0)
            {
                return true;
            }
            if(CrossVector(p1, p3) <= 0 && CrossVector(p3, p2) <= 0 && CrossVector(p2, p1) <= 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 두 벡터의 외적값
        /// </summary>
        /// <returns></returns>
        float CrossVector(Point p, Point q)
        {
            return p.x * q.y - p.y * q.x;
        }

        /// <summary>
        /// 두 벡터의 내적값
        /// </summary>
        /// <param name="p"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        float DotVector(Point p, Point q)
        {
            return p.x*q.x+p.y*q.y;
        }

        /// <summary>
        /// 선분 AB와 원점의 방향 구하기
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        Vector3 VerticalOToLineAB(Point A, Point B)
        {
            Vector3 AB = new Vector3(B.x-A.x,B.y-A.y,0);
            Vector3 AO = new Vector3(-A.x,-A.y,0);

            return Vector3.Cross(Vector3.Cross(AB, AO), AB);
        }

        /// <summary>
        /// 삼중 외적
        /// </summary>
        /// <returns></returns>
        Vector3 TripleCross(Vector3 a, Vector3 b, Vector3 c)
        {
            return Vector3.Cross(Vector3.Cross(a, b), c);
        }
    }
}
