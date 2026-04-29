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
            //정렬
            float px1, px2, px3, px4;
            if (A.a.x > A.b.x)
            {
                px1 = A.b.x;
                px2 = A.a.x;
            }
            else
            {
                px1 = A.a.x;
                px2 = A.b.x;
            }
            if (B.a.x > B.b.x)
            {
                px3 = B.b.x;
                px4 = B.a.x;
            }
            else
            {
                px3 = B.a.x;
                px4 = B.b.x;
            }

            return (px2 >= px3 && px4 >= px1);
        }

        return (baseCondition<=0 && addCondition<=0);
    }
}
