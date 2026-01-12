public static Point DeCasteljau(List<Point> controlPoints, double t)
{
    if (controlPoints.Count == 1)
    {
        return controlPoints[0];
    }

    List<Point> nextPoints = new List<Point>();
    for (int i = 0; i < controlPoints.Count - 1; i++)
    {
        // 선형 보간 공식: P_new = (1-t)*P1 + t*P2
        double x = (1 - t) * controlPoints[i].X + t * controlPoints[i + 1].X;
        double y = (1 - t) * controlPoints[i].Y + t * controlPoints[i + 1].Y;
        nextPoints.Add(new Point(x, y));
    }

    return DeCasteljau(nextPoints, t); // 재귀 호출
}
