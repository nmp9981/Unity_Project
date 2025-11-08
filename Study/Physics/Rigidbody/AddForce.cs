using UnityEngine;

public class ForceTest : MonoBehaviour
{
    [SerializeField]
    public float forcePower = 10f;
    private Rigidbody rb;

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
}
