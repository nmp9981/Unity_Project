public static class MathUtility
{
    /// <summary>
    /// 두 수중 작은 값 반환
    /// </summary>
    /// <param name="a">실수</param>
    /// <param name="b">실수</param>
    /// <returns>Min값</returns>
    public static float Min(float a, float b)
    {
        return a < b ? a : b;
    }
    /// <summary>
    /// 두 수중 큰 값 반환
    /// </summary>
    /// <param name="a">실수</param>
    /// <param name="b">실수</param>
    /// <returns>Min값</returns>
    public static float Max(float a, float b)
    {
        return a > b ? a : b;
    }

    /// <summary>
    /// 가장 가까운 값 찾기
    /// </summary>
    /// <param name="standardValue">기준 값</param>
    /// <param name="minValue">범위 최솟값</param>
    /// <param name="maxValue">범위 최댓값</param>
    /// <returns></returns>
    public static float ClampValue(float standardValue, float minValue, float maxValue)
    {
        if(standardValue < minValue) return minValue;
        if(standardValue > maxValue) return maxValue;

        return standardValue;
    }
}
