using UnityEngine;

public class VehicleController : MonoBehaviour
{
    private float throttle;//앞뒤
    private float steerInput;//좌우
    private float breakInput;

    public float Throttle { get  {return throttle; } set { throttle = value; } }
    public float SteerInput { get { return steerInput; } set { steerInput = value; } }
    public float BrakeInput {  get { return breakInput; } set { breakInput = value; } }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            throttle = 1;
        }else throttle = 0;

        if (Input.GetKeyDown(KeyCode.A)) steerInput = -1;
        else if(Input.GetKeyDown(KeyCode.D)) steerInput = 1;
        else steerInput = 0;
    }
}
