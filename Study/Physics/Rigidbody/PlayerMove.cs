using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMove : MonoBehaviour
{
    public float moveForce = 20f;
    public float maxSpeed = 5f;
    public float airControlFactor = 0.5f;//공중 제어 비율
    //캐릭터가 이동을 멈췄을때 서서히 멈추게 하는 계수
    //값이 클수록 더 빠르게 멈춤, 작을수록 더 제동거리가 더 길다
    public float friction = 2f;
    private Vector3 dir;
    private Rigidbody rb;

    public float jumpPower = 7f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    private bool isGrounded;
    private bool wantToJump;

    void Start() => rb = GetComponent<Rigidbody>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            wantToJump = true;
    }

    private void FixedUpdate()
    {
        // 지면 감지 (Raycast)
        isGrounded = Physics.Raycast(groundCheck.position, Vector3.down, 0.3f, groundLayer);

        //방향 설정
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        dir = new Vector3(x, 0, z).normalized;

        // 가속도 기반 이동
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if(flatVel.magnitude < maxSpeed)
        {
            float control = isGrounded ? 1f : airControlFactor;
            //땅에 없으면 이동력이 감소
            rb.AddForce(dir * moveForce*control,ForceMode.Acceleration);
        }
        
        
        //실제 점프
        if (wantToJump)
        {
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            wantToJump = false;
        }

        // 감속 처리
        if (isGrounded && dir.magnitude < 0.1f)
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, Time.fixedDeltaTime * friction);
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * 0.2f);
    }
}
