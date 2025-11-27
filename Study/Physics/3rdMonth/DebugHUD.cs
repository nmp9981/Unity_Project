using UnityEngine;

public class DebugHUD : MonoBehaviour
{
    float physTimer, physCount;
    float renderTimer, renderCount;
    float lastPhysicsDt;

    void OnEnable()
    {
        Application.targetFrameRate = -1;
    }

    public void OnPhysicsStep(float dt)
    {
        lastPhysicsDt = dt;
        physTimer += Time.unscaledDeltaTime;
        physCount += 1;
    }
    void Update()
    {
        renderTimer += Time.unscaledDeltaTime;
        renderCount += 1;
        if (renderTimer >= 1f)
        {
            float physFPS = physCount / physTimer;
            float renderFPS = renderCount / renderTimer;
            // draw on GUI
            physTimer = renderTimer = physCount = renderCount = 0f;
        }
    }
    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 300, 20), $"Physics FPS: {physCount:0} (last dt: {lastPhysicsDt:F4})");
        GUI.Label(new Rect(10, 30, 300, 20), $"Render FPS: {1f / Time.deltaTime:0}");
        GUI.Label(new Rect(10, 50, 300, 20), $"Interpolation alpha: {(Time.time - Time.fixedTime) / Time.fixedDeltaTime:F2}");
    }
}
