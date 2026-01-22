using System;
using UnityEngine;

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

    //행렬식
    public static float Det(Mat3 a) {
        float first = a.m00 * (a.m11*a.m22-a.m12*a.m21);
        float second =a.m01* (a.m10 * a.m22 - a.m12 * a.m20);
        float third = a.m02 * (a.m10 * a.m21 - a.m11 * a.m20);
        return first-second+third;
    }

    //단위 행렬, 0행렬
    public static Mat3 I()
        => new Mat3
        {
            m00 = 1,m01 = 0,m02 = 0,
            m10 = 0,m11 = 1,m12 = 0,
            m20 = 0,m21 = 0,m22 = 1
        };
    public static Mat3 O()
        => new Mat3
        {
            m00 = 0,m01 = 0,m02 = 0,
            m10 = 0,m11 = 0,m12 = 0,
            m20 = 0,m21 = 0,m22 = 0
        };
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
    /// 3*3행렬을 3*3배열로 변환
    /// </summary>
    /// <param name="a"></param>
    /// <returns></returns>
    public static float[,] MatrixToArray3(Mat3 a)
    {
        float[,] array = new float[3, 3];
        array[0,0] = a.m00; array[0, 1] = a.m00; array[0, 2] = a.m02;
        array[1, 0] = a.m10; array[1, 1] = a.m10; array[1, 2] = a.m12;
        array[2, 0] = a.m20; array[2, 1] = a.m20; array[2, 2] = a.m22;
        return array;
    }
    /// <summary>
    /// 3*3배열을 3*3행렬로 변환
    /// </summary>
    /// <param name="a"></param>
    /// <returns></returns>
    public static Mat3 ArrayToMatrix3(float[,] array)
    {
        Mat3 m = new Mat3();
        m.m00 = array[0, 0]; m.m01 = array[0, 1]; m.m02 = array[0, 2];
        m.m10 = array[1, 0]; m.m11 = array[1, 1]; m.m12 = array[1, 2];
        m.m20 = array[2, 0]; m.m21 = array[2, 1]; m.m22 = array[2, 2];
        return m;
    }
    /// <summary>
    /// 행 swap연산
    /// </summary>
    /// <param name="array">배열</param>
    /// <param name="row1">이동 전</param>
    /// <param name="row2">이동 후</param>
    /// <param name="restRow">나머지 행번호</param>
    /// <returns></returns>
    public static float[,] SwapRow(float[,] array, int row1, int row2, int restRow)
    {
        float[,] newArray = new float[3,3];
        newArray[row2, 0] = array[row1, 0]; newArray[row2, 1] = array[row1, 1]; newArray[row2, 2] = array[row1, 2];
        newArray[row1, 0] = array[row2, 0]; newArray[row1, 1] = array[row2, 1]; newArray[row1, 2] = array[row2, 2];
        newArray[restRow,0] = array[restRow, 0]; newArray[restRow, 1] = newArray[restRow, 1]; newArray[restRow, 2] = array[restRow, 2];
        return newArray;
    }


    /// <summary>
    /// 역행렬
    /// </summary>
    /// <param name="a"></param>
    /// <returns></returns>
    public static Mat3 Inv(Mat3 a)
    {
        float[,] origin = MatrixToArray3(a);
        float[,] invArray = MatrixToArray3(Mat3.I());

        //역행렬이 없음
        if (Mat3.Det(a) == 0)
        {
            return float.MaxValue*Mat3.I();
        }

        //가우스 죠르단 소거법
        for(int i = 0; i < 3; i++)
        {
            //행 Swap
            if (origin[i, i] == 0)//기준
            {
                if (i == 0)
                {
                    if (origin[i + 1, i] == 0)
                    {
                        origin = SwapRow(origin, i, i + 2, 1);
                        invArray = SwapRow(invArray, i, i + 2, 1);
                    }
                    else
                    {
                        origin = SwapRow(origin, i, i + 1, 2);
                        invArray = SwapRow(invArray, i, i + 1, 2);
                    }
                }else if (i == 1)
                {
                    origin = SwapRow(origin, i, i + 1, 0);
                    invArray = SwapRow(invArray, i, i + 1, 0);
                }
            }
            //선도행 계산
            for(int j = 0; j < 3; j++)
            {
                origin[i, j] = origin[i, j] / origin[i, i];
                invArray[i, j] = invArray[i, j] / origin[i, i];

                for(int k = 0; k < 3; k++)
                {
                    if (k == i) continue;
                    
                    origin[k, j] = -origin[k,i]*origin[i, j]+origin[k,j];
                    invArray[k, j] = -origin[k, i] * invArray[i, j] + invArray[k, j];
                }
            }
        }

        Mat3 invMat3 = ArrayToMatrix3(invArray);
        return invMat3;
    }

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
        return Translate(trans.position)*Rotate(trans.rotation)*Scale(trans.scale);
    }
}
