using UnityEngine;

public class TimeCompare : MonoBehaviour
{
    public float speed = 3f;//속도 값
    public float accel = 3f;//가속도 값

    public Vector3 velocity;//속도
    public Vector3 acceleration;//가속도
    private Vector3 prevVel;//이전 속도
    private Vector3 prevPos;//이전 위치


    void Start()
    {
        prevPos = transform.position;
        prevVel = Vector3.zero;

        acceleration = accel * Vector3.right;
    }

    void FixedUpdate()
    {
        //속도
        velocity +=(acceleration*Time.fixedDeltaTime);
        //이동
        transform.position = prevPos + 0.5f * (prevVel + velocity) * Time.fixedDeltaTime;

        //시각화
        Debug.DrawRay(transform.position, velocity, Color.green);
        Debug.DrawRay(transform.position, acceleration, Color.red);

        //갱신
        prevPos = transform.position;
        prevVel = velocity;
    }
}
