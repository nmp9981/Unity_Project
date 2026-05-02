using System.Collections.Generic;
using UnityEngine;

namespace ConVexHULL
{
    public struct Point
    {
        public float x, y;

    }

    public class ConvecHull
    {
        //도형을 이루는 최종 점들의 집합
        Stack<Point> shapePointStack = new Stack<Point>();
        //기준점
        Point pivotPoint;

        /// <summary>
        /// 전체 과정
        /// </summary>
        public void AllFlow(List<Point> pointList)
        {
            if (pointList.Count < 3) return;//도형을 이룰 수 없음

            //기준점 잡기
            pivotPoint = pointList[0];
            foreach (var pos in pointList)
            {
                if (pivotPoint.y > pos.y || (pivotPoint.y == pos.y && pivotPoint.x > pos.x))
                {
                    pivotPoint = pos;
                }
            }
            //각도 정렬
            pointList.Sort((a, b) => {
                if (a.Equals(pivotPoint)) return -1;
                if (b.Equals(pivotPoint)) return 1;

                int dir = CCW(pivotPoint, a, b);
                if (dir != 0) return -dir; // 반시계 방향이 우선순위가 높도록

                // 일직선상에 있다면 거리가 가까운 쪽을 우선
                float distA = Mathf.Pow(a.x - pivotPoint.x, 2) + Mathf.Pow(a.y - pivotPoint.y, 2);
                float distB = Mathf.Pow(b.x - pivotPoint.x, 2) + Mathf.Pow(b.y - pivotPoint.y, 2);
                return distA.CompareTo(distB);
            });

            RemovePointInLine(pointList);//일직선 위 점 제거
        }

        /// <summary>
        /// CCW : 선분 방향 판정
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <returns></returns>
        public int CCW(Point p1, Point p2, Point p3)
        {
            float abX = p2.x - p1.x;
            float abY = p2.y - p1.y;
            float acX = p3.x - p1.x;
            float acY = p3.y - p1.y;

            float crossResult = abX * acY - abY * acX;

            if (crossResult > 0) return 1;//반시계
            if (crossResult < 0) return -1;//시계

            return 0;//일직선
        }

        /// <summary>
        /// 일직선 위 점 제거
        /// </summary>
        Stack<Point> RemovePointInLine(List<Point> pointList)
        {
            shapePointStack.Clear();
            //첫 2개의점을 넣는다.
            shapePointStack.Push(pointList[0]);
            shapePointStack.Push(pointList[1]);

            //점 순회하면서 시계방향일 경우 제거
            for (int i = 2; i < pointList.Count; i++)
            {
                while(shapePointStack.Count >= 2)
                {
                    //위에서 2개의 점을 꺼낸다.
                    Point p1 = shapePointStack.Pop();//맨위
                    Point p2 = shapePointStack.Peek();//맨위에서 2번째

                    //현재점까지 3개의 점으로 CCW를 사용해서 방향을 검사한다.
                    Point curP = pointList[i];
                    int dir = CCW(p2, p1, curP);

                    //반시계 방향인가?
                    if (dir == 1)
                    {
                        shapePointStack.Push(p1);//다시 넣는다.
                        break;
                    }
                }
                shapePointStack.Push(pointList[i]);
            }
            return shapePointStack;
        }
    }
}
