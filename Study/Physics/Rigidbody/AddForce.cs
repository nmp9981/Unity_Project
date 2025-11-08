using UnityEngine;

public class ForceTest : MonoBehaviour
{
    [SerializeField]
    public float forcePower = 10f;
    private Rigidbody rb;
    private Vector3 lastVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.W))
            rb.AddForce(Vector3.forward * forcePower);
        if (Input.GetKey(KeyCode.S))
            rb.AddForce(Vector3.back * forcePower);
        if (Input.GetKey(KeyCode.A))
            rb.AddForce(Vector3.left * forcePower);
        if (Input.GetKey(KeyCode.D))
            rb.AddForce(Vector3.right * forcePower);

        // 직접 Velocity 제어 테스트
        if (Input.GetKeyDown(KeyCode.Space))
            rb.linearVelocity = Vector3.up * 5f;
    }

    void FixedUpdate()
    {
        // 속도 벡터 시각화 (파란색)
        Debug.DrawLine(transform.position, transform.position + rb.linearVelocity, Color.blue);

        // 가속도 근사 (현재속도 - 이전속도)
        Vector3 accel = (rb.linearVelocity - lastVelocity) / Time.fixedDeltaTime;
        Debug.DrawLine(transform.position, transform.position + accel * 0.1f, Color.red);

        lastVelocity = rb.linearVelocity;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.1f);
    }
}
