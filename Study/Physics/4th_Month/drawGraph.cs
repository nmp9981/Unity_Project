using UnityEngine;

public class TestScripts : MonoBehaviour
{
    public float dt = 0.02f;
    public int steps = 1000;
    public float scale = 1f;

    LineRenderer graphX;   // time-x 그래프
    LineRenderer graphPhase; // phase-space 그래프

    void Start()
    {
        // LineRenderer 준비
        graphX = CreateLine("Graph_Time_X");
        graphPhase = CreateLine("Graph_Phase");

        // 시뮬레이션 데이터 저장
        float[] xs = new float[steps];
        float[] vs = new float[steps];
        float[] ts = new float[steps];

        float x = 1f;
        float v = 0f;
        float t = 0f;

        for (int i = 0; i < steps; i++)
        {
            // Euler Integration
            x = x + v * dt;
            v = v - x * dt;

            xs[i] = x;
            vs[i] = v;
            ts[i] = t;

            t += dt;
        }

        // 그래프 그리기
        DrawGraphTimeX(graphX, ts, xs);
        DrawGraphPhase(graphPhase, xs, vs);
    }

    LineRenderer CreateLine(string name)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(this.transform);

        var lr = obj.AddComponent<LineRenderer>();
        lr.widthMultiplier = 0.05f;
        lr.positionCount = 0;
        lr.material = new Material(Shader.Find("Sprites/Default"));

        return lr;
    }

    void DrawGraphTimeX(LineRenderer lr, float[] ts, float[] xs)
    {
        lr.positionCount = ts.Length;
        for (int i = 0; i < ts.Length; i++)
        {
            lr.SetPosition(i, new Vector3(ts[i] * 2f, xs[i] * scale, 0));
        }
    }

    void DrawGraphPhase(LineRenderer lr, float[] xs, float[] vs)
    {
        lr.positionCount = xs.Length;
        for (int i = 0; i < xs.Length; i++)
        {
            lr.SetPosition(i, new Vector3(xs[i] * scale * 2f, vs[i] * scale * 2f, 0));
        }
    }
}
