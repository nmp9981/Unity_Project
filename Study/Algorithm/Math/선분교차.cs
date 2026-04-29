using System;

/// <summary>
/// 점
/// </summary>
public struct Point
{
    public float x, y;
}

/// <summary>
/// 선
/// </summary>
public struct line
{
    public Point a, b;
}

public class Geometry
{
    
    /// <summary>
    /// CCW : 선분 방향 판정
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    /// <returns></returns>
    public int CCW(Point p1, Point p2, Point p3)
    {
        float abX = p2.x-p1.x;
        float abY = p2.y - p1.y;
        float acX = p3.x - p1.x;
        float acY = p3.y - p1.y;

        float crossResult = abX * acY - abY * acX;

        if (crossResult > 0) return 1;//반시계
        if(crossResult < 0) return -1;//시계

        return 0;//일직선
    }
    
    /// <summary>
    /// 선분 교차 판정
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    /// <returns></returns>
    public bool IsCrossLine(line A, line B)
    {
        int baseCondition = CCW(A.a, A.b, B.a) * CCW(A.a,A.b,B.b);
        int addCondition = CCW(B.a, B.b, A.a) * CCW(B.a, B.b, A.b);

        //일직선
        if(baseCondition == 0 && addCondition == 0)
        {
            //각 선분의 최대, 최소
            float minAX = MathF.Min(A.a.x,A.b.x);
            float maxAX = MathF.Max(A.a.x, A.b.x);
            float minAY = MathF.Min(A.a.y, A.b.y);
            float maxAY = MathF.Max(A.a.y, A.b.y);

            float minBX = MathF.Min(B.a.x, B.b.x);
            float maxBX = MathF.Max(B.a.x, B.b.x);
            float minBY = MathF.Min(B.a.y, B.b.y);
            float maxBY = MathF.Max(B.a.y, B.b.y);

            return (maxAX >= minBX && maxBX >= minAX)
                && (maxAY >= minBY && maxBY >= minAY);
        }

        return (baseCondition<=0 && addCondition<=0);
    }
}
