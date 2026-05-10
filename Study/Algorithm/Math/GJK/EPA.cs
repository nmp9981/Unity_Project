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
    public struct Edge
    {
        public Vector2 normal;   // 바깥 방향 (단위 벡터)
        public float distance;   // 원점에서 이 edge(직선)까지의 수직 거리
        public int index;        // polytope 리스트에서의 위치 (분할할 때 필요)
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

            const int MAX_ITER = 128;
            for (int i=0;i<MAX_ITER;i++)
            {
                //dir 방뱡으로 새점을 뽑는다.
                Point p = Support(shapeA, shapeB, dir);

                //종료 조건(새로 봅은 점 P기준)
                float dot = p.x * (dir.x) + p.y * (dir.y);
                if (dot <= 0) return false;//충돌 X

                //원점 포함하면 충돌 -> EPA로
                if (IsContainTriangle(p1,p2,p))
                {
                    //EPA
                    simplexList = new List<Point> { p1, p2, p };
                    var (depth, normal) = EPA_Algorithm(shapeA, shapeB, simplexList);
                    Debug.Log($"충돌! 깊이: {depth}, 방향: {normal}");
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
            return false;
        }

        /// <summary>
        /// EPA 알고리즘
        /// </summary>
        /// <returns></returns>
        (float depth, Vector3 direction) EPA_Algorithm(List<Point> shapeA, List<Point> shapeB, List<Point> polytope)
        {
            //CCW 정렬
            EnsureCCW(polytope);

            const int MAX_ITER = 32;
            const float EPSILON = 0.0001f;

            //반복 루프
            for (int iter = 0; iter < MAX_ITER; iter++)
            {
                //원점에서 가장 가까운 모서리
                Edge edge = FindClosestEdge(polytope);

                // edge.normal 방향으로 새 support point
                Vector3 dir3 = new Vector3(edge.normal.x, edge.normal.y, 0);
                Point p = Support(shapeA, shapeB, dir3);

                // 새 점을 normal 방향으로 투영한 길이
                float dotPN = p.x * edge.normal.x + p.y * edge.normal.y;

                // 수렴: 새 점이 edge보다 더 못 나갔다 = edge가 진짜 경계
                if (dotPN - edge.distance < EPSILON)//찐 경계를 찾음
                {
                    return (edge.distance, edge.normal);
                }

                // polytope 확장, 아직 경계를 못찾을 때 해당 모서리도 추가해서 확장
                polytope.Insert(edge.index + 1, p);
            }

            // 최대 반복 도달 — 마지막 결과 반환
            Edge final = FindClosestEdge(polytope);
            return (final.distance, final.normal);
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
        /// CCW 정렬
        /// </summary>
        /// <param name="polytope"></param>
        void EnsureCCW(List<Point> polytope)
        {
            // 삼각형 가정 (3점)
            Vector3 ab = new Vector3(polytope[1].x - polytope[0].x, polytope[1].y - polytope[0].y, 0);
            Vector3 ac = new Vector3(polytope[2].x - polytope[0].x, polytope[2].y - polytope[0].y, 0);
            float signedArea = Vector3.Cross(ab, ac).z;

            if (signedArea < 0)//시계 방향이면 두 점을 바꾼다.
            {
                // CW → 마지막 두 점 swap
                (polytope[1], polytope[2]) = (polytope[2], polytope[1]);
            }
        }

        /// <summary>
        /// 원점에서 가장 가까운 점 찾기
        /// </summary>
        /// <param name="polyrype"></param>
        /// <returns></returns>
        Edge FindClosestEdge(List<Point> polytope)
        {
            Edge closest = new Edge();
            closest.distance = float.MaxValue;

            int count = polytope.Count;
            for (int i= 0;i < count;i++)
            {
                // 모서리 구하기: polytope[i] → polytope[(i+1) % count]
                int j = (i + 1) % count;
                Point a = polytope[i];
                Point b = polytope[j];

                // edge 벡터
                Vector2 edgeVec = new Vector2(b.x - a.x, b.y - a.y);

                // 바깥 normal (CCW 가정 → 시계방향 90도 회전)
                //90도 회전한 벡터를 반환
                //변 방향을 오르쪽으로 90도 회전하면 노말벡터 나옴
                Vector2 normal = new Vector2(edgeVec.y, -edgeVec.x).normalized;

                // 원점에서 edge까지의 거리
                float distance = Vector2.Dot(normal, new Vector2(a.x, a.y));

                // 최소 거리 갱신
                if (distance < closest.distance)
                {
                    closest.distance = distance;
                    closest.normal = normal;
                    closest.index = i;
                }
            }
            return closest;
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
