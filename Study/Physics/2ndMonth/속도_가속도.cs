using UnityEngine;

public class TimeCompare : MonoBehaviour
{
    public float speed = 3f;//속도 값
    public float accel = 1f;//가속도 값

    public Vector3 velocity;//속도
    public Vector3 acceleration;//가속도
    private Vector3 prevVel;//이전 속도
    private Vector3 prevPos;//이전 위치


    void Start()
    {
        prevPos = transform.position;
        prevVel = Vector3.zero;
    }

    void FixedUpdate()
    {
        float t = Time.time;
        float force = Mathf.Sin(t) * 2f; // -2..2 N (임의)
        //이동
        transform.position = prevPos + Vector3.right * force*Time.fixedDeltaTime;

        //속도
        velocity = (transform.position - prevPos) / Time.fixedDeltaTime;
        //가속도
        acceleration = (velocity - prevVel) / Time.fixedDeltaTime;

        //시각화
        Debug.DrawRay(transform.position, velocity, Color.green);
        Debug.DrawRay(transform.position, acceleration, Color.red);

        //갱신
        prevPos = transform.position;
        prevVel = velocity;
    }
}
