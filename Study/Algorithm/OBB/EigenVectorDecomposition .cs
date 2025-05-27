using System.Numerics;
using System;

/// <summary>
/// 3x3행렬 클래스
/// </summary>
public class Matrix3x3
{
    public double[,] Mat { get; private set; } = new double[3, 3];

    public Matrix3x3(double[,] data)
    {
        if (data.GetLength(0) != 3 || data.GetLength(1) != 3)
            throw new ArgumentException("Matrix must be 3x3.");
        Array.Copy(data, Mat, 9);//원본 : data
    }

    //스칼라 곱셈, 단위 행렬 뺄셈 (A-lamda*I)
    public Matrix3x3 SubtractScalarIdentity(Complex scalar)
    {
        double[,] resultData = new double[3, 3]; // 결과 행렬은 double로 유지하되, 내부적으로 Complex 계산
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                resultData[i, j] = Mat[i, j];
            }
            // 대각선 요소에서 복소수 스칼라를 뺌 (실수 부분만)
            resultData[i, i] -= scalar.Real;
        }
        return new Matrix3x3(resultData);
    }

    // 행렬식 계산
    public double DeterminantBy3x3Matrix()
    {
        double det = 0;
        det += Mat[0, 0] * (Mat[1, 1] * Mat[2, 2] - Mat[1, 2] * Mat[2, 1]);
        det -= Mat[0, 1] * (Mat[1, 0] * Mat[2, 2] - Mat[1, 2] * Mat[2, 0]);
        det += Mat[0, 2] * (Mat[1, 0] * Mat[2, 1] - Mat[1, 1] * Mat[2, 0]);
        return det;
    }

    //행렬 복사
    public Matrix3x3 Clone()
    {
        return new Matrix3x3(Mat);
    }
}

public class EigenVectorDecomposition
{
    /// <summary>
    /// 고윳값, 고유벡터 구하는 Flow
    /// </summary>
    /// <param name="meshCovMatrix"></param>
    public (Complex[] eigenvalues, double[,] eigenvectors) SolveEigenVectorFlow(UnityEngine.Matrix4x4 meshCovMatrix)
    {
        double[,] changed3x3Matrix = Change3x3Matrix(meshCovMatrix);
        Matrix3x3 mat = new Matrix3x3(changed3x3Matrix);

        var eigenvalues = GetEigenValue(mat);
        var eigenvectors = GetEigenVector(eigenvalues, mat);

        return (eigenvalues, eigenvectors);
    }
    /// <summary>
    /// 3x3행렬로 변환
    /// </summary>
    /// <param name="meshCovMatrix"></param>
    /// <returns></returns>
    double[,] Change3x3Matrix(UnityEngine.Matrix4x4 meshCovMatrix)
    {
        double[,] data = new double[,]
        {
             {meshCovMatrix.m00, meshCovMatrix.m01, meshCovMatrix.m02},
             {meshCovMatrix.m10, meshCovMatrix.m11, meshCovMatrix.m12},
             {meshCovMatrix.m20, meshCovMatrix.m21, meshCovMatrix.m22}
        };
        return data;
    }
    /// <summary>
    /// 고윳값 구하기
    /// 1) 3차 방정식을 세운다.
    /// 2) 3차 방정식의 근을 구한다.
    /// </summary>
    /// <param name="m"></param>
    /// <returns></returns>
    public Complex[] GetEigenValue(Matrix3x3 m)
    {
        //특성 방정식 구하기(3차 방정식) => x^3+bx^2+cx+d=0
        //고윳값 성질은 방정식의 근과 계수와의 관계와 연관
        double aCoeff = 1.0;
        double bCoeff = -(m.Mat[0, 0] + m.Mat[1, 1] + m.Mat[2, 2]);//trace(A), 세근의 합

        double M00 = m.Mat[1, 1] * m.Mat[2, 2] - m.Mat[1, 2] * m.Mat[2, 1];
        double M11 = m.Mat[0, 0] * m.Mat[2, 2] - m.Mat[0, 2] * m.Mat[2, 0];
        double M22 = m.Mat[0, 0] * m.Mat[1, 1] - m.Mat[0, 1] * m.Mat[1, 0];

        double cCoeff = M00 + M11 + M22;
        double dCoeff = -m.DeterminantBy3x3Matrix();//det(A), 세근의 곱

        //3차방정식의 해 구하기
        Complex[] eigenvalues = SolveCubic(aCoeff, bCoeff, cCoeff, dCoeff);
        return eigenvalues;
    }
    /// <summary>
    /// 고유벡터 구하기
    /// </summary>
    /// <param name="eigenvalues"></param>
    /// <returns></returns>
    public double[,] GetEigenVector(Complex[] eigenvalues, Matrix3x3 mat, double epcilon = 1e-9)
    {
        double[,] eigenvectors = new double[3, 3];

        //각 고윳값에 대한 고유벡터를 구한다.
        for (int i = 0; i < eigenvalues.Length; i++)
        {
            Complex lambda = eigenvalues[i];//고윳값

            // (A - lambda * I) 행렬 생성 (실수 부분만 사용)
            // 주의: 행렬 A는 double이지만, lambda는 Complex이므로 (A-lambda*I)는 Complex 행렬이 되어야 함.
            // 하지만 이 가우스 소거법은 Complex 숫자를 직접 다루지 않으므로,
            // 실수 고유값에 대해서만 작동하는 단순화된 방식입니다.
            // Complex 고유값을 처리하려면 행렬 요소도 Complex 타입으로 다루어야 합니다.
            // 여기서는 실수 고유값에 대한 고유 벡터만 유효합니다.
            Matrix3x3 M = mat.SubtractScalarIdentity(lambda);

            //가우스 죠르단 소거법 사용해서 연립방정식의 해 구하기 => 이 해가 고유벡터
            // 목표: 행렬을 행 사다리꼴(Row Echelon Form)로 만듦
            Matrix3x3 gauseMat = RowEchelonFormMatrix(M);

            // 영공간(Null Space)에서 고유 벡터 찾기
            // (A - lambda*I)v = 0 에서 v = [x, y, z]
            // 행렬의 랭크가 2라고 가정 (하나의 자유 변수: z)
            // 만약 랭크가 1이라면, 더 많은 자유 변수와 다른 접근 방식이 필요합니다.
            Complex[] v = new Complex[3];

            int rank = RankMatrix(gauseMat);
            //랭크가 3일경우
            if (rank==3)
            {
                v[2] = gauseMat.Mat[2, 2];
            }
            else if(rank==2)//랭크 2
            {
                // z를 자유 변수로 설정 (예: z = 1)
                // (v[2]는 z, v[1]은 y, v[0]은 x)
                v[2] = new Complex(1.0, 0.0);
            }
            else//랭크 1
            {
                // y, z를 자유 변수로 설정 (예: z = 1)
                // (v[2]는 z, v[1]은 y, v[0]은 x)
                v[2] = new Complex(1.0, 0.0);
                v[1] = new Complex(1.0, 0.0);
            }

            // 두 번째 행으로부터 y 계산: m_current[1,1]y + m_current[1,2]z = 0
            if (rank >= 2)
            {
                if (Math.Abs(gauseMat.Mat[1, 1]) > epcilon)
                {
                    v[1] = -new Complex(gauseMat.Mat[1, 2], 0.0) * v[2] / new Complex(gauseMat.Mat[1, 1], 0.0);
                }
                else // m_current[1,1]이 0에 가깝다면, v[1]도 자유 변수일 가능성
                {
                    v[1] = new Complex(1.0, 0.0); // 이 경우, 다른 자유 변수처럼 1로 설정 (단순화)
                }
            }

            // 첫 번째 행으로부터 x 계산: m_current[0,0]x + m_current[0,1]y + m_current[0,2]z = 0
            if (Math.Abs(gauseMat.Mat[0, 0]) > epcilon)
            {
                v[0] = (-new Complex(gauseMat.Mat[0, 1], 0.0) * v[1]
                    - new Complex(gauseMat.Mat[0, 2], 0.0) * v[2]) / new Complex(gauseMat.Mat[0, 0], 0.0);
            }
            else // m_current[0,0]이 0에 가깝다면, v[0]도 자유 변수일 가능성
            {
                v[0] = new Complex(1.0, 0.0); // 이 경우, 다른 자유 변수처럼 1로 설정 (단순화)
            }

            // 고유 벡터 정규화
            eigenvectors[0, i] = NormalizeVector(v)[0].Real;
            eigenvectors[1, i] = NormalizeVector(v)[1].Real;
            eigenvectors[2, i] = NormalizeVector(v)[2].Real;
        }
        return eigenvectors;
    }

    /// <summary>
    /// Rank 구하기
    /// </summary>
    /// <param name="gauseMat"></param>
    /// <returns></returns>
    int RankMatrix(Matrix3x3 gauseMat)
    {
        int rank = 0;
        //랭크가 3일경우
        if (gauseMat.Mat[2, 2] != 0)
        {
            rank = 3;
        }
        else
        {
            //랭크 1
            if (gauseMat.Mat[1, 2] == 0 && gauseMat.Mat[1, 1] == 0)
                rank = 1;
            else//랭크 2
                rank = 2;
        }
        return rank;
    }

    // --- 3차 방정식의 근을 찾는 카르다노의 공식 구현 ---
    // x^3 + px + q = 0 형태의 방정식에 대한 근 찾기
    private Complex[] SolveDepressedCubic(double p, double q)
    {
        Complex[] roots = new Complex[3];

        double discriminant = (q / 2.0) * (q / 2.0) + (p / 3.0) * (p / 3.0) * (p / 3.0);

        if (discriminant >= 0)
        {
            // 한 개의 실수 근과 두 개의 켤레 복소수 근, 또는 모든 실수 근 (중근 포함)
            double u = Math.Cbrt(-q / 2.0 + Math.Sqrt(discriminant));
            double v = Math.Cbrt(-q / 2.0 - Math.Sqrt(discriminant));

            roots[0] = new Complex(u + v, 0); // 실수 근

            // 나머지 두 근은 복소수 또는 중근
            double realPart = -(u + v) / 2.0;
            double imagPart = Math.Sqrt(3.0) / 2.0 * (u - v);

            roots[1] = new Complex(realPart, imagPart);
            roots[2] = new Complex(realPart, -imagPart);
        }
        else
        {
            // 세 개의 서로 다른 실수 근 (삼각함수 형태)
            double u = 2.0 * Math.Sqrt(-p / 3.0);
            double theta = Math.Acos((-q / 2.0) / Math.Pow(-p / 3.0, 1.5));
            // Ensure theta is in valid range for Acos. Due to floating point issues, it might slightly exceed [-1,1].
            if (double.IsNaN(theta))
            { // Handle cases where argument to Acos is slightly out of range
                double arg = (-q / 2.0) / Math.Pow(-p / 3.0, 1.5);
                if (arg > 1.0) arg = 1.0;
                if (arg < -1.0) arg = -1.0;
                theta = Math.Acos(arg);
            }

            roots[0] = new Complex(u * Math.Cos(theta / 3.0), 0);
            roots[1] = new Complex(u * Math.Cos((theta + 2.0 * Math.PI) / 3.0), 0);
            roots[2] = new Complex(u * Math.Cos((theta + 4.0 * Math.PI) / 3.0), 0);
        }
        return roots;
    }

    // 일반 3차 방정식 ax^3 + bx^2 + cx + d = 0 의 근 찾기
    private Complex[] SolveCubic(double a, double b, double c, double d)
    {
        // 3차 방정식이 아님
        if (a == 0) throw new ArgumentException("3차 방정식이 아닙니다.");

        // 방정식을 x^3 + p*x + q = 0 형태로 변환 (Depressed Cubic)
        double p = (3 * a * c - b * b) / (3 * a * a);
        double q = (2 * b * b * b - 9 * a * b * c + 27 * a * a * d) / (27 * a * a * a);

        //카르다노의 공식
        Complex[] depressedRoots = SolveDepressedCubic(p, q);

        // 변환된 근을 원래 방정식의 근으로 되돌림: x = y - b/(3a)
        Complex b_3a = new Complex(b / (3.0 * a), 0);
        for (int i = 0; i < 3; i++)
        {
            depressedRoots[i] -= b_3a;
        }
        return depressedRoots;
    }

    /// <summary>
    /// 행 사다리꼴 행렬로 변환
    /// </summary>
    /// <param name="m"></param>
    /// <returns></returns>
    Matrix3x3 RowEchelonFormMatrix(Matrix3x3 m, double epcilon = 1e-9)
    {
        Matrix3x3 curMat = m.Clone();

        for (int k = 0; k < 2; k++)
        {
            //피벗행 찾기(최대 절댓값 비교)
            int pivotRow = k;
            for (int row = k + 1; row < 3; row++)
            {
                if (Math.Abs(m.Mat[row, k]) > Math.Abs(m.Mat[pivotRow, k]))
                {
                    pivotRow = row;
                }
            }
            //피벗 행, 현재 행 교환
            if (pivotRow != k)
            {
                //각 열별로 교환
                for (int j = k; j < 3; j++)
                {
                    double temp = curMat.Mat[k, j];
                    curMat.Mat[k, j] = curMat.Mat[pivotRow, j];
                    curMat.Mat[pivotRow, j] = temp;
                }
            }

            // 피벗 요소가 0에 가까우면 다음 열로 넘어감 (수치적 불안정성 회피)
            if (Math.Abs(curMat.Mat[k, k]) < epcilon)
            {
                continue;
            }

            // 피벗 아래의 요소들을 0으로 만듦
            for (int row = k + 1; row < 3; row++)
            {
                double factor = curMat.Mat[row, k] / curMat.Mat[k, k];
                for (int col = k; col < 3; col++)
                {
                    curMat.Mat[row, col] -= factor * curMat.Mat[k, col];
                }
            }
        }
        return curMat;
    }
    /// <summary>
    /// 벡터 정규화 (복소수 벡터용)
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    private static Complex[] NormalizeVector(Complex[] vector)
    {
        double magnitudeSquared = 0;
        foreach (Complex val in vector)
        {
            magnitudeSquared += (val * Complex.Conjugate(val)).Real; // |z|^2 = z * z
        }
        double magnitude = Math.Sqrt(magnitudeSquared);

        // 영 벡터는 정규화 불가
        if (magnitude == 0) return vector;

        Complex[] normalized = new Complex[vector.Length];
        for (int i = 0; i < vector.Length; i++)
        {
            normalized[i] = vector[i] / magnitude;
        }
        return normalized;
    }
}
