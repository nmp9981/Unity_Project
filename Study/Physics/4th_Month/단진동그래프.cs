using UnityEngine;

public class TestScripts : MonoBehaviour
{
    public float x = 1f;     // 초기 위치
    public float v = 0f;     // 초기 속도
    public float dt = 0.02f; // 실험용 Δt

    float t;
    const float k = 1f;      // 스프링 강성(편의상 1)

    // 그래프 용
    private LineRenderer line;
    int index = 0;

    void Start()
    {
        // 그래프 라인 설정
        line = gameObject.AddComponent<LineRenderer>();
        line.positionCount = 1;
        line.widthMultiplier = 0.05f;
    }

    void FixedUpdate()
    {
        // ⚠️ 물리 계산(Euler)
        float a = -k * x;

        x = x + v * dt;
        v = v + a * dt;

        // 그래프 점 추가
        line.positionCount++;
        line.SetPosition(index, new Vector3(t, x, 0));
        index++;

        t += dt;

        Debug.Log($"t={t:F2} / x={x:F3} / v={v:F3}");
    }
}
