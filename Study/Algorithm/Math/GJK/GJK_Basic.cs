using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

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
        public void GJK_AllFlow(List<Point> shape1, List<Point> shape2)
        {
            HashSet<Point> newShape = MincovskiDiff(shape1, shape2);
            IsOriginPoint(newShape);
        }

        /// <summary>
        /// 민코브스키 차
        /// </summary>
        /// <param name="shape1"></param>
        /// <param name="shape2"></param>
        /// <returns></returns>
        HashSet<Point> MincovskiDiff(List<Point> shape1, List<Point> shape2)
        {
            HashSet<Point> diffSet = new HashSet<Point>();

            int shape1Count = shape1.Count;
            int shape2Count = shape2.Count;
            for (int i = 0; i < shape1Count; i++)
            {
                for(int j=0;j<shape2Count; j++)
                {
                    Point p1 = shape1[i];
                    Point p2 = shape2[j];

                    //p2-p1
                    Point diffPoint = new Point(p2.x-p1.x,p2.y-p1.y);
                    diffSet.Add(diffPoint);
                }
            }
            return diffSet;
        }

        /// <summary>
        /// 새로운 도형이 원점을 포함하는가?
        /// </summary>
        /// <returns></returns>
        bool IsOriginPoint(HashSet<Point> points)
        {
            //세 점을 골라야함
            int pointCount = points.Count;
            Point[] pointArray = points.ToArray();
            for(int i = 0; i < pointCount; i++)
            {
                for(int j = i + 1; j < pointCount; j++)
                {
                    for(int k = j + 1; k < pointCount; k++)
                    {
                        ///원점 포함
                        if (IsContainTriangle(pointArray[i], pointArray[j], pointArray[k]))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
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
    }
}
