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
    /// 거듭 제곱 구하기
    /// a^n
    /// </summary>
    /// <param name="a">실수</param>
    /// <param name="n">정수</param>
    /// <returns></returns>
    public static float Pow(float a, int n)
    {
        if (n == 1) return a;

        //n이 짝수
        if(n%2==0) return Pow(a,n/2)*Pow(a,n/2);
        //n이 홀수
        return Pow(a,n/2)*Pow(a,n/2)*a;
    }

    /// <summary>
    /// 제곱근 구하기
    /// </summary>
    /// <param name="a">실수</param>
    /// <returns>루트a</returns>
    public static float Root(float a)
    {
        a = (a < 0) ? -a : a;//0이상의 실수로 변환

        float x = a;
        for (int i = 0; i < 11; i++)
        {
            x = (x + (a / x)) / 2;
        }
        return x;
    }

    /// <summary>
    /// 절댓값 구하기
    /// </summary>
    /// <param name="a">실수</param>
    /// <returns></returns>
    public static float Abs(float a)
    {
        return (a < 0) ? -a : a;
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
