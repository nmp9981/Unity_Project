using System;
using UnityEngine;

public static class MathUtility
{
    public const float PI = 3.141592653589793f;
    public const float EPSILON = 1e-6f;
    public const float Rad2Deg = 57.29578f;
    public const float Deg2Rad = 0.0174533f;

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
    /// 거듭 제곱 구하기
    /// a^n
    /// </summary>
    /// <param name="a">실수</param>
    /// <param name="b">실수</param>
    /// <returns></returns>
    public static float Pow(float a, float b)
    {
        float lnA = Ln(a);
        float BlnA = b * lnA;

        return Exp(BlnA);
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
    /// 선형 보간 값구하기
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public static float Lerp(float a, float b, float t)
    {
        return a + (b - a) * ClampValue(t,0,1);
    }
    /// <summary>
    /// 값이 a,b사이에서 어디쯤 있는지
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static float InverseLerp(float a, float b, float value)
    {
        // a와 b가 같을 경우 나누기 0 오류 방지
        if (a == b) return 0f;

        // (현재값 - 시작값) / (끝값 - 시작값)
        float result = (value - a) / (b - a);

        // 결과를 0과 1 사이로 제한 (Clamping)
        return ClampValue(result,0,1);
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
    /// Atan값(1변수)
    /// </summary>
    /// <param name="x">cos 값</param>
    /// <param name="y">sin 값</param>
    /// <returns></returns>
    public static float Atan(float x)
    {
        float x3 = (x * x * x)/3;
        float x5 = (x3 * x * x)/5;
        float x7 = (x5 * x * x)/7;
        float x9 = (x7 * x * x)/9;

        float atanValue = x - x3 + x5 - x7 + x9;
        return atanValue;
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
    /// <summary>
    /// Tanh(x)
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public static float Tanh(float x)
    {
        float ePlusX = Exp(x);
        float eMinusX = Exp(-x);

        float tanh = (ePlusX - eMinusX) / (ePlusX + eMinusX);
        return tanh;
    }
    #endregion

    /// <summary>
    /// e^x
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public static float Exp(float x)
    {
        float d2 = x * x * 0.5f;
        float d3 = x * x * x / 6;
        float d4 = Pow(x, 4) / 24;
        float d5 = Pow(x, 5) / 120;
        float ex = 1 + x + d2+d3+d4+d5;
        return Exp(x);
    }

    public static float Ln(float x)
    {
        if (x <= 0f) return float.NaN;

        int k = 0;

        // x를 1 근처로 맞춘다
        while (x > 2f)
        {
            x *= 0.5f;
            k++;
        }
        while (x < 0.5f)
        {
            x *= 2f;
            k--;
        }

        float y = (x - 1f) / (x + 1f);
        float y2 = y * y;

        float sum = 0f;
        float term = y;

        for (int i = 1; i < 20; i += 2)
        {
            sum += term / i;
            term *= y2;
        }

        return 2f * sum + k * 0.69314718f; // ln(2)
    }

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
