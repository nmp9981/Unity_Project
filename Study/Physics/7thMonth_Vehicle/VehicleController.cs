using UnityEngine;

public class VehicleController : MonoBehaviour
{
    private float throttle;//앞뒤
    private float steerInput;//좌우
    private float breakInput;

    public float maxSteerAngle = 30;//최대 바퀴 회전각

    public float Throttle { get  {return throttle; } set { throttle = value; } }
    public float SteerInput { get { return steerInput; } set { steerInput = value; } }
    public float BrakeInput {  get { return breakInput; } set { breakInput = value; } }

    VehicleDynamicsCore core;

    private void Start()
    {
        core = new VehicleDynamicsCore();
        core.state.Ux = 5f;
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    throttle = 1;
        //}else throttle = 0;

        //if (Input.GetKeyDown(KeyCode.A)) steerInput = -1;
        //else if(Input.GetKeyDown(KeyCode.D)) steerInput = 1;
        //else steerInput = 0;
    }

    private void FixedUpdate()
    {
        VehicleInput input = new VehicleInput
        {
            throttle = Input.GetAxis("Vertical") > 0 ? Input.GetAxis("Vertical") : 0,
            brake = Input.GetAxis("Vertical") < 0 ? -Input.GetAxis("Vertical") : 0,
            steering = Input.GetAxis("Horizontal") * 0.4f
        };

        core.Step(input, Time.fixedDeltaTime);

        // Transform 업데이트
        transform.Translate(
            new Vector3(core.state.Ux * Time.fixedDeltaTime, 0, 0));
        transform.Rotate(
            Vector3.up,
            core.state.r * Mathf.Rad2Deg * Time.fixedDeltaTime);
    }
    /// <summary>
    /// 바퀴 회전각 가져오기
    /// </summary>
    /// <returns></returns>
    public float GetSteerAngle() => steerInput * maxSteerAngle;
}
