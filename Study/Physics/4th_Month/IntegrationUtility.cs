public static class IntegrationUtility
{
    /// <summary>
    /// Verlet 위치 수치적분
    /// </summary>
    /// <param name="dt">시간 간격</param>
    /// <param name="a">가속도</param>
    /// <param name="curX">현재 위치</param>
    /// <param name="preX">이전 위치</param>
    /// <returns>다음 위치</returns>
    public static float PositionVerlet(float dt, float a,float curX, float preX)
    {
        float nextX = 2 * curX - preX + a * dt * dt;
        return nextX;
    }
}
