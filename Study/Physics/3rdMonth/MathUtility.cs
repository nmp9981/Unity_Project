public static class MathUtility
{
    /// <summary>
    /// 가장 가까운 값 찾기
    /// </summary>
    /// <param name="standardValue">기준 값</param>
    /// <param name="minValue"></param>
    /// <param name="maxValue"></param>
    /// <returns></returns>
    public static float ClampValue(float standardValue, float minValue, float maxValue)
    {
        if(standardValue < minValue) return minValue;
        if(standardValue > maxValue) return maxValue;

        return standardValue;
    }
}
