using UnityEngine;

public class TelemetryUI : MonoBehaviour
{
    public DOF3RigidBody car;
    public TelemetryGraph speedGraph;
    public TelemetryGraph slipGraph;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            speedGraph.Draw(car.GetSpeedData(), 0f, 50f, Color.green);
            slipGraph.Draw(car.GetSlipRLData(), -1f, 1f, Color.red);
        }
    }
}
