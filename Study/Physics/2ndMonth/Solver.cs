using System.Collections.Generic;
using UnityEngine;

public static class Solver
{
    public static void Solve(float dt, List<MassPoint> masses, List<Spring> springs)
    {
        // 1) 힘 초기화
        Dictionary<MassPoint, Vector3> forceMap = new Dictionary<MassPoint, Vector3>();
        foreach (var m in masses) forceMap[m] = Vector3.zero;

        // 2) 스프링 힘 계산 (Hooke's Law)
        foreach (var s in springs)
        {
            Vector3 delta = s.b.position - s.a.position;
            float dist = delta.magnitude;
            float x = dist - s.restLength;

            if (dist > 0)
            {
                Vector3 F = s.k * x * (delta / dist);
                forceMap[s.a] += F;
                forceMap[s.b] -= F;
            }
        }

        // 3) 각 질점에 중력 적용
        foreach (var m in masses)
        {
            forceMap[m] += new Vector3(0, -9.81f * m.mass, 0);
        }

        // 4) Forward Euler 적분 (수요일 목표 수준)
        foreach (var m in masses)
        {
            Vector3 acceleration = forceMap[m] / m.mass;
            m.velocity += acceleration * dt;
            m.position += m.velocity * dt;

            // Transform 반영
            m.transform.position = m.position;
        }
    }

    /// <summary>
    /// Ax=b 의 선형 해
    /// </summary>
    /// <param name="A"></param>
    /// <param name="b"></param>
    /// <param name="iterations"></param>
    /// <returns></returns>
    public static float[] MatrixSolve(float[,] A, float[] b, int iterations = 15)
    {
        int n = b.Length;
        float[] x = new float[n];

        for (int it = 0; it < iterations; it++)
        {
            for (int i = 0; i < n; i++)
            {
                float sum = 0;

                for (int j = 0; j < n; j++)
                {
                    if (j != i)
                        sum += A[i, j] * x[j];
                }

                x[i] = (b[i] - sum) / A[i, i];
            }
        }

        return x;
    }
}
