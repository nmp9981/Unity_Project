using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FrictionController : MonoBehaviour
{
    public Transform ground; //땅
    public float muStatic = 0.6f;//정지 마찰 계수
    public float muKinetic = 0.5f;//운동 마찰 계수
    Rigidbody rb;

    void Start() => rb = GetComponent<Rigidbody>();

    private void FixedUpdate()
    {
        //ray를 쏴서 판별
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.5f))
        {
            Vector3 normal = hit.normal;//땅의 노말 벡터
            Vector3 gravity = Physics.gravity;//중력
            //땅의 기울기 방향으로 투영된 벡터(운동 방향)
            Vector3 g_parallel = Vector3.ProjectOnPlane(gravity, normal);
            //물체를 미끄러지게 하는 힘의 크기(쉽게 말해 알짜힘)
            float gParallelMag = g_parallel.magnitude * rb.mass;

            //투영벡터 힘(mg의 투영벡터)=> 수직항력
            float normalForce = Vector3.Project(gravity * rb.mass, normal).magnitude;

            //정지마찰력 = 정지 마찰 계수 * 힘
            float maxStatic = muStatic * normalForce;

            //정지 마찰 상태
            if(gParallelMag < maxStatic && rb.linearVelocity.magnitude < 0.01f)
            {
                //정지 상태
            }
            else//운동 상태
            {
                //물체의 선형 속도(rb.linearVelocity)를 땅의 표면 방향으로 투영한 속도 벡터
                //물체의 운동 방향
                Vector3 vPlane = Vector3.ProjectOnPlane(rb.linearVelocity, normal);
                //운동 마찰 방향(운동 방향과 반대)
                Vector3 frictionDir = -vPlane.normalized;
                //운동 마찰 크기 : 운동 마찰 계수 * 수직항력
                float frictionMag = muKinetic * normalForce;
                //운동 마찰력 = 운동 마찰 방향 * 운동 마찰 크기
                Vector3 friction = frictionDir * frictionMag;

                //마찰 힘 적용
                rb.AddForce(friction);
            }

            // debug
            Debug.DrawRay(transform.position, g_parallel, Color.yellow);
            Debug.DrawRay(transform.position, Vector3.up * normalForce * 0.01f, Color.cyan);
        }

    }
}
