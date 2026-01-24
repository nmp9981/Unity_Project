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
    public static Vec3 operator *(Mat3 a, Vec3 b)
       => new Vec3(a.m00 * b.x+ a.m01 * b.y+ a.m02 * b.z,
           a.m10 * b.x+ a.m11 * b.y+ a.m12 * b.z,
           a.m20 * b.x+ a.m21 * b.y+ a.m22 * b.z);
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

    //전치행렬
    public static Mat3 Transpose(Mat3 a)
    {
        Mat3 transMat = new Mat3(a.m00,a.m10,a.m20,a.m01,a.m11,a.m21,a.m02,a.m12,a.m22);
        return transMat;
    }
}

/// <summary>
/// 4*4행렬 구조체
/// </summary>
[System.Serializable]
public struct Mat4
{
    public float m00, m01, m02,m03;
    public float m10, m11, m12,m13;
    public float m20, m21, m22,m23;
    public float m30, m31, m32, m33;

    //생성자
    public Mat4(float m00, float m01, float m02, float m03
        , float m10, float m11, float m12, float m13,
        float m20, float m21, float m22, float m23,
        float m30, float m31, float m32, float m33)
    {
        this.m00 = m00; this.m01 = m01; this.m02 = m02; this.m03 = m03;
        this.m10 = m10; this.m11 = m11; this.m12 = m12; this.m13 = m13;
        this.m20 = m20; this.m21 = m21; this.m22 = m22; this.m23 = m23;
        this.m30 = m30; this.m31 = m31; this.m32 = m32; this.m33 = m33;
    }

    //기본 연산자 오버로딩
    public static Mat4 operator +(Mat4 a, Mat4 b)
        => new Mat4(a.m00 + b.m00, a.m01 + b.m01, a.m02 + b.m02, a.m03 + b.m03,
            a.m10 + b.m10, a.m11 + b.m11, a.m12 + b.m12, a.m13 + b.m13,
            a.m20 + b.m20, a.m21 + b.m21, a.m22 + b.m22, a.m23+b.m23,
            a.m30 + b.m30, a.m31 + b.m31, a.m32 + b.m32, a.m33 + b.m33);
    public static Mat4 operator -(Mat4 a, Mat4 b)
       => new Mat4(a.m00 - b.m00, a.m01 - b.m01, a.m02 - b.m02,a.m03 - b.m03,
           a.m10 - b.m10, a.m11 - b.m11, a.m12 - b.m12, a.m13 - b.m13,
           a.m20 - b.m20, a.m21 - b.m21, a.m22 - b.m22, a.m23-b.m23,
           a.m30 - b.m30, a.m31 - b.m31, a.m32 - b.m32, a.m33 - b.m33);
    public static Mat4 operator *(float a, Mat4 b)
       => new Mat4(a * b.m00, a * b.m01, a * b.m02, a* b.m03,
           a * b.m10, a * b.m11, a * b.m12,a* b.m13,
           a * b.m20, a * b.m21, a * b.m22, a*b.m23,
           a * b.m30, a * b.m31, a * b.m32, a * b.m33);
    public static Mat4 operator *(Mat4 a, Mat4 b)
       => new Mat4(a.m00 * b.m00 + a.m01 * b.m10 + a.m02 * b.m20+a.m03*b.m30, 
           a.m00 * b.m01 + a.m01 * b.m11 + a.m02 * b.m21 + a.m03 * b.m31, 
           a.m00 * b.m02 + a.m01 * b.m12 + a.m02 * b.m22 + a.m03 * b.m32,
           a.m00 * b.m03 + a.m01 * b.m13 + a.m02 * b.m23 + a.m03 * b.m33,
           a.m10 * b.m00 + a.m11 * b.m10 + a.m12 * b.m20 + a.m13 * b.m30, 
           a.m10 * b.m01 + a.m11 * b.m11 + a.m12 * b.m21 + a.m13 * b.m31, 
           a.m10 * b.m02 + a.m11 * b.m12 + a.m12 * b.m22 + a.m13 * b.m32,
           a.m10 * b.m03 + a.m11 * b.m13 + a.m12 * b.m23 + a.m13 * b.m33,
           a.m20 * b.m00 + a.m21 * b.m10 + a.m22 * b.m20 + a.m23 * b.m30, 
           a.m20 * b.m01 + a.m21 * b.m11 + a.m22 * b.m21 + a.m23 * b.m31, 
           a.m20 * b.m02 + a.m21 * b.m12 + a.m22 * b.m22 + a.m23 * b.m32,
           a.m20 * b.m03 + a.m21 * b.m13 + a.m22 * b.m23 + a.m23 * b.m33,
           a.m30 * b.m00 + a.m31 * b.m10 + a.m32 * b.m20 + a.m33 * b.m30,
           a.m30 * b.m01 + a.m31 * b.m11 + a.m32 * b.m21 + a.m33 * b.m31,
           a.m30 * b.m02 + a.m31 * b.m12 + a.m32 * b.m22 + a.m33 * b.m32,
           a.m30 * b.m03 + a.m31 * b.m13 + a.m32 * b.m23 + a.m33 * b.m33);
    public static Mat4 operator /(Mat4 a, float b)
      => new Mat4(a.m00 / b, a.m01 / b, a.m02 / b, a.m03/b,
          a.m10 / b, a.m11 / b, a.m12 / b, a.m13/b,
          a.m20 / b, a.m21 / b, a.m22 / b, a.m23/b,
          a.m30 / b, a.m31 / b, a.m32 / b, a.m33 / b);

    //단위 행렬, 0행렬
    public static Mat4 I()
        => new Mat4
        {
            m00 = 1,
            m01 = 0,
            m02 = 0,
            m03 = 0,
            m10 = 0,
            m11 = 1,
            m12 = 0,
            m13 = 0,
            m20 = 0,
            m21 = 0,
            m22 = 1,
            m23 = 0,
            m30 = 0,
            m31 = 0,
            m32 = 0,
            m33 = 1,
        };
    public static Mat4 O()
        => new Mat4
        {
            m00 = 0,
            m01 = 0,
            m02 = 0,
            m03 = 0,
            m10 = 0,
            m11 = 0,
            m12 = 0,
            m13 = 0,
            m20 = 0,
            m21 = 0,
            m22 = 0,
            m23 = 0,
            m30 = 0,
            m31 = 0,
            m32 = 0,
            m33 = 0
        };
    //전치행렬
    public static Mat4 Transpose(Mat4 a)
    {
        Mat4 transMat = new Mat4(a.m00, a.m10, a.m20, a.m30,a.m01, a.m11, a.m21,a.m31, 
            a.m02, a.m12, a.m22,a.m32, a.m03, a.m13, a.m23, a.m33);
        return transMat;
    }
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
        array[0,0] = a.m00; array[0, 1] = a.m01; array[0, 2] = a.m02;
        array[1, 0] = a.m10; array[1, 1] = a.m11; array[1, 2] = a.m12;
        array[2, 0] = a.m20; array[2, 1] = a.m21; array[2, 2] = a.m22;
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
        newArray[restRow,0] = array[restRow, 0]; newArray[restRow, 1] = array[restRow, 1]; newArray[restRow, 2] = array[restRow, 2];
        return newArray;
    }

    /// <summary>
    /// 역행렬, 회전행렬 전용
    /// R^-1 = R^t
    /// </summary>
    /// <param name="a"></param>
    /// <returns></returns>
    public static Mat3 Transpose(Mat3 a)
    {
        Mat3 invMat3 = new Mat3();
        invMat3.m00 = a.m00;
        invMat3.m01 = a.m10;
        invMat3.m02 = a.m20;
        invMat3.m10 = a.m01;
        invMat3.m11 = a.m11;
        invMat3.m12 = a.m21;
        invMat3.m20 = a.m02;
        invMat3.m21 = a.m12;
        invMat3.m22 = a.m22;
        return invMat3;
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
        if (Mat3.Det(a) < MathUtility.EPSILON)
        {
            return Mat3.O();
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
    /// 점 변환 -> 이동
    /// 동차좌표에서 (x, y, 1) 을 곱함
    /// </summary>
    /// <param name="p"></param>
    /// <param name="m"></param>
    /// <returns></returns>
    public static Vec2 MulPoint(Vec2 p, Mat3 m)
    {
        return new Vec2(
            m.m00 * p.x + m.m01 * p.y + m.m02,
            m.m10 * p.x + m.m11 * p.y + m.m12
        );
    }
    /// <summary>
    /// 방향 변환 -> 방향과 스케일
    /// 동차좌표에서 (x, y, 0) 을 곱함
    /// </summary>
    /// <param name="v"></param>
    /// <param name="m"></param>
    /// <returns></returns>
    public static Vec2 MulVector(Vec2 v, Mat3 m)
    {
        return new Vec2(
            m.m00 * v.x + m.m01 * v.y,
            m.m10 * v.x + m.m11 * v.y
        );
    }
    /// <summary>
    /// 점 변환 -> 이동
    /// 동차좌표에서 (x, y, z, 1) 을 곱함
    /// </summary>
    /// <param name="p"></param>
    /// <param name="m"></param>
    /// <returns></returns>
    public static Vec3 MulPoint(Vec3 p, Mat4 m)
    {
        return new Vec3(
            m.m00 * p.x + m.m01 * p.y + m.m02*p.z+m.m03,
            m.m10 * p.x + m.m11 * p.y + m.m12*p.z+m.m13,
            m.m20 * p.x + m.m21 * p.y + m.m22 * p.z + m.m23
        );
    }
    /// <summary>
    /// 방향 변환 -> 방향과 스케일
    /// 동차좌표에서 (x, y, z, 0) 을 곱함
    /// </summary>
    /// <param name="v"></param>
    /// <param name="m"></param>
    /// <returns></returns>
    public static Vec3 MulVector(Vec3 v, Mat4 m)
    {
        return new Vec3(
            m.m00 * v.x + m.m01 * v.y + m.m02*v.z,
            m.m10 * v.x + m.m11 * v.y + m.m12 * v.z,
            m.m20 * v.x + m.m21 * v.y + m.m22 * v.z
        );
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
        Matrix.m20 = 0; Matrix.m21 = 0; Matrix.m22 = 1;
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
    /// <summary>
    /// TR Matrix
    /// </summary>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public static Mat4 TR(Vec3 position, Mat3 rotation)
    {
        Mat4 m = Mat4.I();

        m.m00 = rotation.m00; m.m01 = rotation.m01; m.m02 = rotation.m02;
        m.m10 = rotation.m10; m.m11 = rotation.m11; m.m12 = rotation.m12;
        m.m20 = rotation.m20; m.m21 = rotation.m21; m.m22 = rotation.m22;

        m.m03 = position.x;
        m.m13 = position.y;
        m.m23 = position.z;

        return m;
    }
    /// <summary>
    /// TR역행렬
    /// </summary>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public static Mat4 InverseTR(Vec3 position, Mat3 rotation)
    {
        // R^-1 = R^T
        Mat3 rT = Mat3.Transpose(rotation);
        Vec3 t = (-1f)*(rT * position);

        Mat4 m = Mat4.I();

        m.m00 = rT.m00; m.m01 = rT.m01; m.m02 = rT.m02;
        m.m10 = rT.m10; m.m11 = rT.m11; m.m12 = rT.m12;
        m.m20 = rT.m20; m.m21 = rT.m21; m.m22 = rT.m22;

        m.m03 = t.x;
        m.m13 = t.y;
        m.m23 = t.z;

        return m;
    }

    /// <summary>
    /// 위치 변환
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static Mat4 Translate(Vec3 t)
    {
        Mat4 Matrix = new Mat4();
        Matrix.m00 = 1; Matrix.m01 = 0; Matrix.m02 = 0; Matrix.m03 = t.x;
        Matrix.m10 = 0; Matrix.m11 = 1; Matrix.m12 = 0; Matrix.m13 = t.y;
        Matrix.m20 = 0; Matrix.m21 = 0; Matrix.m22 = 1; Matrix.m23 = t.z;
        Matrix.m30 = 0; Matrix.m31 = 0; Matrix.m32 = 0; Matrix.m33 = 1;
        return Matrix;
    }
   
    /// <summary>
    /// 크기 변환
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static Mat4 Scale(Vec3 s)
    {
        Mat4 Matrix = new Mat4(s.x, 0, 0,0,
            0, s.y, 0,0,
            0,0, s.z, 0,
            0, 0, 0,1);
        return Matrix;
    }

    public static Mat3 TRS(Transform2D trans)
    {
        return Translate(trans.position)*Rotate(trans.rotation)*Scale(trans.scale);
    }
  
    public static Mat3 InverseTRS(Vec2 pos, float rot, Vec2 scale)
    {
        Mat3 invScale = Scale(new Vec2(1 / scale.x, 1 / scale.y));
        Mat3 invRot = Rotate(-rot);
        Mat3 invTrans = Translate(pos*(-1));

        return invScale * invRot * invTrans;
    }
    public static Mat3 FromQuaternion(CustomQuaternion q)
    {
        float xx = q.vec.x * q.vec.x;
        float yy = q.vec.y * q.vec.y;
        float zz = q.vec.z * q.vec.z;
        float xy = q.vec.x * q.vec.y;
        float xz = q.vec.x * q.vec.z;
        float yz = q.vec.y * q.vec.z;
        float wx = q.scala * q.vec.x;
        float wy = q.scala * q.vec.y;
        float wz = q.scala * q.vec.z;

        return new Mat3(
            1f - 2f * (yy + zz), 2f * (xy - wz), 2f * (xz + wy),
            2f * (xy + wz), 1f - 2f * (xx + zz), 2f * (yz - wx),
            2f * (xz - wy), 2f * (yz + wx), 1f - 2f * (xx + yy)
        );
    }
}
