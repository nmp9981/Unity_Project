using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMove : MonoBehaviour
{
    public float moveForce = 20f;
    public float maxSpeed = 5f;
    private Rigidbody rb;

    void Start() => rb = GetComponent<Rigidbody>();

    void Update()
    {
        //방향 설정
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 dir = new Vector3(x, 0, z).normalized;

        // 가속도 기반 이동
        if (rb.linearVelocity.magnitude < maxSpeed)
            rb.AddForce(dir * moveForce);
    }
}
