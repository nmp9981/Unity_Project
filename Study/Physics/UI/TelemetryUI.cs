using System.Collections.Generic;
using UnityEngine;

public class TelemetryUI : MonoBehaviour
{
    public DOF3RigidBody car;
    public TelemetryGraph speedGraph;
    public TelemetryGraph slipGraph;
    public List<TelemetryFrame> telemetryLog = new List<TelemetryFrame>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            speedGraph.Draw(car.GetSpeedData(), 0f, 50f, Color.green);
            slipGraph.Draw(car.GetSlipRLData(), -1f, 1f, Color.red);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            AnalyzeAll();
        }
    }
    /// <summary>
    /// 전체 로그 분석
    /// </summary>
    public void AnalyzeAll()
    {
        foreach (var f in telemetryLog)
        {
            AnalyzeFrame(f);
        }
    }
    /// <summary>
    /// 구간 분석
    /// </summary>
    /// <param name="startTime"></param>
    /// <param name="endTime"></param>
    public void AnalyzeCorner(float startTime, float endTime)
    {
        foreach (var f in telemetryLog)
        {
            if (f.time < startTime || f.time > endTime)
                continue;

            AnalyzeFrame(f);
        }
    }
    /// <summary>
    /// 각 프레임 분석
    /// </summary>
    /// <param name="f"></param>
    public void AnalyzeFrame(TelemetryFrame f)
    {
        if (DetectOversteer(f))
        {
            Debug.Log($"[{f.time:F2}] Oversteer 발생 " +
                      $"rearSlip:{Mathf.Max(Mathf.Abs(f.slipRL), Mathf.Abs(f.slipRR)):F2} " +
                      $"yaw:{f.yawRate:F2}");
        }

        if (DetectUndersteer(f))
        {
            Debug.Log($"[{f.time:F2}] Understeer 발생 " +
                      $"frontSlip:{Mathf.Max(Mathf.Abs(f.slipFL), Mathf.Abs(f.slipFR)):F2}");
        }

        if (DetectGripLoss(f))
        {
            Debug.Log($"[{f.time:F2}] Grip Loss " +
                      $"slip:{Mathf.Max(Mathf.Abs(f.slipRL), Mathf.Abs(f.slipRR)):F2}");
        }
    }
    /// <summary>
    /// 오버스티어 감지
    /// </summary>
    /// <param name="f"></param>
    /// <returns></returns>
    public bool DetectOversteer(TelemetryFrame f)
    {
        float rearSlip = MathUtility.Max(MathUtility.Abs(f.slipRL), MathUtility.Abs(f.slipRR));
        float frontSlip = MathUtility.Max(MathUtility.Abs(f.slipFL), MathUtility.Abs(f.slipFR));

        bool slipCondition = rearSlip > frontSlip + 0.05f;
        bool yawCondition = MathUtility.Abs(f.yawRate) > 0.3f;

        return slipCondition && yawCondition;
    }
    /// <summary>
    /// 언더스티어 감지
    /// </summary>
    /// <param name="f"></param>
    /// <returns></returns>
    public bool DetectUndersteer(TelemetryFrame f)
    {
        float rearSlip = MathUtility.Max(MathUtility.Abs(f.slipRL), MathUtility.Abs(f.slipRR));
        float frontSlip = MathUtility.Max(MathUtility.Abs(f.slipFL), MathUtility.Abs(f.slipFR));

        return frontSlip > rearSlip + 0.05f;
    }

    public bool DetectGripLoss(TelemetryFrame f)
    {
        float maxSlip = MathUtility.Max(
           MathUtility.Max(MathUtility.Abs(f.slipFL),
            MathUtility.Abs(f.slipFR)),
            MathUtility.Max(MathUtility.Abs(f.slipRL),
            MathUtility.Abs(f.slipRR))
        );

        return maxSlip > 0.3f;
    }
}
