using System;

public static class MathUtility
{
    const float PI = 3.141592653589793f;
    public const float EPSILON = 1e-6f;
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
/// 실수의 부호 추출
/// </summary>
/// <param name="a">실수</param>
/// <returns></returns>
public static float Sign(float a)
{
    if(a == 0) return 0;
    if (a < 0) return -1;
    return 1;
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

    #region 삼각함수
    /// <summary>
    /// 입력 각도 범위 제한
    /// 단위 : 호도법
    /// 제한 : -PI ~ PI
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public static float WrapPI(float x)
    {
        const float TWO_PI = 2f * PI;

        //제한 : -PI ~ PI
        x = x % TWO_PI;
        if (x > PI) x -= TWO_PI;
        if (x < -PI) x += TWO_PI;
        return x;
    }
    /// <summary>
    /// sin값
    /// </summary>
    /// <param name="x"></param>
    public static float Sin(float x)
    {
        x= WrapPI(x);

        float x2 = x*x;
        float d3 = (x2*x) / 6;
        float x4 = x2*x2;
        float d5 = (x4 * x)/120;

        return x - d3 + d5;
    }
    /// <summary>
    /// cos값
    /// </summary>
    /// <param name="x"></param>
    public static float Cos(float x)
    {
        x= WrapPI(x);

        return Sin(x+0.5f*PI);
    }
    /// <summary>
    /// Asin값
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public static float Asin(float x)
    {
        //5차항까지
        float x2 = x * x;
        float x4 = x2 * x2;
        float d3 = (x2 * x) / 6;
        float d5 = (3 * x4 * x) * 0.025f;

        return x + d3 + d5;
    }
    /// <summary>
    /// Acos값
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public static float Acos(float x)
    {
        float halfPI = PI / 2;
        //5차항까지
        float x2 = x * x;
        float x4 = x2 * x2;
        float d3 = (x2 * x) / 6;
        float d5 = (3 * x4 * x) * 0.025f;

        return halfPI - (x+d3+d5);
    }
    /// <summary>
    /// Tan값
    /// </summary>
    /// <param name="x"></param>
    public static float Tan(float x)
    {
        return Sin(x)/Cos(x);
    }
    /// <summary>
    /// Atan값(2변수)
    /// </summary>
    /// <param name="x">cos 값</param>
    /// <param name="y">sin 값</param>
    /// <returns></returns>
    public static float Atan2(float x, float y)
    {
        if (x == 0.0f)
            return y > 0 ? PI/2 : -PI/2;

        float absY = Abs(y);
        float absX = Abs(x);

        float angle;
        if (absY <= absX)
        {
            float z = y / x;
            float z2 = z * z;
            angle = z - z2 * z / 3 + z2 * z2 * z / 5;
        }
        else
        {
            float z = x / y;
            float z2 = z * z;
            angle = PI/2 - (z - z2 * z / 3 + z2 * z2 * z / 5);
        }

        // 사분면 보정
        //2,3사분면일때 각각 4,1사분면으로 보정
        if (x < 0)
            angle += (y >= 0) ? PI : -PI;

        return angle;
    }
    #endregion

    /// <summary>
    /// 2차 방정식 해 존재여부
    /// </summary>
    /// <returns></returns>
    public static bool SolveEquation2(float a, float b, float c,out float t0,out float t1)
    {
        t0 = t1 = 0;

        //판별식
        float D = b * b - 4 * a * c;
        //실수해가 없음
        if(D<0) return false;

        float invA = 1 / (2 * a);
        float rootD = Root(D);

        //해
        t0 = invA * (-b+rootD);
        t1 = invA * (-b - rootD);
        if (t0 > t1)
        {
            float temp = t1;
            t1 = t0;
            t0 = temp;
        }

        return true;
    }
}
