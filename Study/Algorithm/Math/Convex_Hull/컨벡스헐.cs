using System.Collections.Generic;
using System.Linq;
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
        int pivotIndex = 0;

        /// <summary>
        /// 전체 과정
        /// </summary>
        public void AllFlow(List<Point> pointList)
        {
            //다각형을 이룰 수 없음
            if (pointList.Count < 3) return;

            //피벗 점 찾기
            for (int i = 1; i < pointList.Count; i++)
            {
                //y좌표가 더 작은 점이면 갱신 같은 경우 x좌표가 더 작은 값으로
                if (pointList[pivotIndex].y > pointList[i].y ||
                    (pointList[pivotIndex].y == pointList[i].y && pointList[pivotIndex].x > pointList[i].x))
                {
                    pivotIndex = i;
                }
            }

            //각도 정렬
            pointList = AngleSort(pointList);

            //일직선 위 점 제거하면서 컨벡스헐 구성
            RemovePointInLine(pointList);
        }

        /// <summary>
        /// 각도 정렬
        /// </summary>
        public List<Point> AngleSort(List<Point> pointList)
        {
            //배열로 바꿈
            Point[] pointArray = pointList.ToArray();

            //피벗포인트를 0번으로
            Point temp = pointArray[0];
            pointArray[0] = pointArray[pivotIndex];
            pointArray[pivotIndex] = temp;

            //각도 정렬
            int pointLength = pointArray.Length;
            bool isSwap = false;
            for (int i = 1; i < pointLength; i++)
            {
                for(int j = i + 1; j < pointLength; j++)
                {
                    //두 점
                    Point p0 = pointArray[i];
                    Point p1 = pointArray[j];

                    //방향
                    int dir = CCW(pointArray[0],p0,p1);

                    if (dir == 0)//일직선
                    {
                        //피벗점간의 거리 비교
                        float dist0 = (pointArray[0].x - p0.x) * (pointArray[0].x - p0.x) + (pointArray[0].y - p0.y) * (pointArray[0].y - p0.y);
                        float dist1 = (pointArray[0].x - p1.x) * (pointArray[0].x - p1.x) + (pointArray[0].y - p1.y) * (pointArray[0].y - p1.y);

                        //스왑
                        if (dist0 > dist1)
                        {
                            isSwap = true;
                        }else isSwap = false;
                    }
                    else if (dir == -1)//시계
                    {
                        isSwap = true;
                    }
                    else {//반시계면 정상
                        isSwap = false;
                    }

                    //Swap
                    if (isSwap)
                    {
                        Point tempPoint = pointArray[i];
                        pointArray[i] = pointArray[j];
                        pointArray[j] = tempPoint;
                    }
                }
            }
            return pointArray.ToList();
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

            //첫 두개의 점 넣기
            shapePointStack.Push(pointList[0]);
            shapePointStack.Push(pointList[1]);

            for (int i = 2; i < pointList.Count; i++)
            {
                Point curPoint = pointList[i];
                while(shapePointStack.Count >= 2)
                {
                    //맨위 2개의 점
                    Point p1 = shapePointStack.Pop();
                    Point p2 = shapePointStack.Peek();

                    //방향
                    int dir = CCW(p2,p1,curPoint);

                    //반시계
                    if (dir == 1)
                    {
                        shapePointStack.Push(p1);
                        break;
                    }
                }
                shapePointStack.Push(curPoint);
            }
            return shapePointStack;
        }
    }
}
