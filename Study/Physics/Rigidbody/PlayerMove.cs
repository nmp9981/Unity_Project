using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMove : MonoBehaviour
{
    public float moveForce = 20f;
    public float maxSpeed = 5f;
    private Rigidbody rb;

    public float jumpPower = 7f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    private bool isGrounded;

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

        // 지면 감지 (Raycast)
        isGrounded = Physics.Raycast(groundCheck.position, Vector3.down, 0.2f, groundLayer);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
    }

   
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * 0.2f);
    }
}
