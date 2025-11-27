using UnityEngine;

public class DebugHUD : MonoBehaviour
{
    float physTimer, physCount;//물리 효과
    float renderTimer, renderCount;//렌더링 효과
    float lastPhysicsDt;

    /// <summary>
    /// 프레임레이트를 제한하지 않음(무제한)
    /// 엔진이 가능한 최대 속도로 렌더링
    /// </summary>
    void OnEnable()
    {
        Application.targetFrameRate = -1;
    }
    private void FixedUpdate()
    {
        OnPhysicsStep(Time.fixedDeltaTime);
    }
    /// <summary>
    /// 1초 동안 Physics Step이 몇 번 실행됐는지(FPS)
    /// </summary>
    /// <param name="dt"></param>
    public void OnPhysicsStep(float dt)
    {
        lastPhysicsDt = dt;
        physTimer += Time.unscaledDeltaTime;
        physCount += 1;
    }
    /// <summary>
    /// 프레임 마다 실행
    /// physFPS = physics step이 초당 몇 번 실행됐는지
    ///→ FixedUpdate가 설정한 dt대로 움직이는지 검사
    /// renderFPS = Update가 초당 몇 번 실행됐는지
    ///→ 프레임레이트 측정
    /// </summary>
    void Update()
    {
        renderTimer += Time.unscaledDeltaTime;
        renderCount += 1;
        if (renderTimer >= 1f)//1초
        {
            float physFPS = physCount / physTimer;
            float renderFPS = renderCount / renderTimer;
            //초기화
            physTimer = renderTimer = physCount = renderCount = 0f;
        }
    }
    /// <summary>
    /// UI표시
    /// </summary>
    void OnGUI()
    {
        float physicsFPS = 1 / Time.fixedDeltaTime;

        GUI.Label(new Rect(10, 10, 300, 20), $"Physics FPS: {physicsFPS:0} (last dt: {lastPhysicsDt:F4})");
        GUI.Label(new Rect(10, 30, 300, 20), $"Render FPS: {1f / Time.deltaTime:0}");
        GUI.Label(new Rect(10, 50, 300, 20), $"Interpolation alpha: {(Time.time - Time.fixedTime) / Time.fixedDeltaTime:F2}");
    }
}
