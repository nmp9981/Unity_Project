using static UnityEditor.PlayerSettings;

/// <summary>
/// 3*3행렬 구조체
/// </summary>
[System.Serializable]
public struct Mat3
{
    public float m00, m01, m02;
    public float m10, m11, m12;
    public float m20, m21, m22;

    //생성자
    public Mat3(float m00, float m01, float m02
        ,float m10, float m11,float m12,
        float m20, float m21, float m22)
    {
        this.m00 = m00;this.m01 = m01;this.m02 = m02;
        this.m10 = m10;this.m11 = m11;this.m12 = m12;
        this.m20 = m20;this.m21 = m21; this.m22 = m22;
    }

    //기본 연산자 오버로딩
    public static Mat3 operator +(Mat3 a, Mat3 b)
        => new Mat3(a.m00 + b.m00, a.m01 + b.m01, a.m02+b.m02,
            a.m10 + b.m10, a.m11 + b.m11, a.m12 + b.m12,
            a.m20 + b.m20, a.m21 + b.m21, a.m22 + b.m22);
    public static Mat3 operator -(Mat3 a, Mat3 b)
       => new Mat3(a.m00 - b.m00, a.m01 - b.m01, a.m02 - b.m02,
           a.m10 - b.m10, a.m11 - b.m11, a.m12 - b.m12,
           a.m20 - b.m20, a.m21 - b.m21, a.m22 - b.m22);
    public static Mat3 operator *(float a, Mat3 b)
       => new Mat3(a* b.m00, a* b.m01, a*b.m02,
           a*b.m10, a* b.m11, a*b.m12,
           a* b.m20, a* b.m21, a*b.m22);
    public static Mat3 operator *(Mat3 a, Mat3 b)
       => new Mat3(a.m00 * b.m00 + a.m01*b.m10+a.m02*b.m20, a.m00 * b.m01 + a.m01 * b.m11 + a.m02 * b.m21, a.m00 * b.m02 + a.m01 * b.m12 + a.m02 * b.m22,
           a.m10 * b.m00 + a.m11 * b.m10 + a.m12 * b.m20, a.m10 * b.m01 + a.m11 * b.m11 + a.m12 * b.m21, a.m10 * b.m02 + a.m11 * b.m12 + a.m12 * b.m22,
           a.m20 * b.m00 + a.m21 * b.m10 + a.m22 * b.m20, a.m20 * b.m01 + a.m21 * b.m11 + a.m22 * b.m21, a.m20 * b.m02 + a.m21 * b.m12 + a.m22 * b.m22);
    public static Mat3 operator /(Mat3 a, float b)
      => new Mat3(a.m00/b, a.m01 / b, a.m02 / b,
          a.m10 / b, a.m11 / b, a.m12 / b,
          a.m20 / b, a.m21 / b, a.m22 / b);

    //길이, 방향
    public float Det => x * x + y * y;
   

    //내적, 행렬식
    public static float Dot(Vec2 a, Vec2 b)
        => a.x * b.x + a.y * b.y;
}

public class Transform2D
{
    public Vec2 position;
    public float rotation;
    public Vec2 scale;

    public Mat3 LocalToWorld;
    public Mat3 WorldToLocal;
}

public static class MatrixUtility
{
    /// <summary>
    /// 위치 변환
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static Mat3 Translate(Vec2 t)
    {
        Mat3 Matrix = new Mat3();
        Matrix.m00 = 1; Matrix.m01 = 0; Matrix.m02 = t.x;
        Matrix.m10 = 0; Matrix.m11 = 1; Matrix.m12 = t.y;
        Matrix.m20 = 0; Matrix.m21 = 0; Matrix.m22 = 1;
        return Matrix;
    }
    /// <summary>
    /// 회전 행렬
    /// </summary>
    /// <param name="rad"></param>
    /// <returns></returns>
    public static Mat3 Rotate(float rad)
    {
        float cos = MathUtility.Cos(rad);
        float sin = MathUtility.Sin(rad);

        Mat3 Matrix = new Mat3();
        Matrix.m00 = cos; Matrix.m01 = -sin; Matrix.m02 = 0;
        Matrix.m10 = sin; Matrix.m11 = cos; Matrix.m12 = 0;
        Matrix.m20 = 0; Matrix.m21 = 0; Matrix.m22 = 0;
        return Matrix;
    }
    /// <summary>
    /// 크기 변환
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static Mat3 Scale(Vec2 s)
    {
        Mat3 Matrix = new Mat3(s.x,0,0,
            0,s.y,0,
            0,0,1);
        return Matrix;
    }

    public static Mat3 TRS(Transform2D trans)
    {
        return Translate(trans.position)+Rotate(trans.rotation)+Scale(trans.scale);
    }
}
