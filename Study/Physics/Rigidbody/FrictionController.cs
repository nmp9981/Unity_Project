using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FrictionController : MonoBehaviour
{
    public Transform ground; //땅
    public float muStatic = 0.6f;//정지 마찰 계수
    public float muKinetic = 0.5f;//운동 마찰 계수

    Vec3 gravity = new Vec3(0,-9.81f,0);//중력
    Rigidbody rb;

    void Start() => rb = GetComponent<Rigidbody>();

    private void FixedUpdate()
    {
        //ray를 쏴서 판별
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.5f))
        {
            Vec3 normal = new Vec3(hit.normal.x, hit.normal.y, hit.normal.z);//땅의 노말 벡터
            //땅의 기울기 방향으로 투영된 벡터(운동 방향)
            Vec3 g_parallel = VectorMathUtils.ProjectOnPlane(gravity, normal);
            //물체를 미끄러지게 하는 힘의 크기(쉽게 말해 알짜힘)
            float gParallelMag = g_parallel.Magnitude * rb.mass;

            //투영벡터 힘(mg의 투영벡터)=> 수직항력
            float normalForce = VectorMathUtils.Project(gravity * rb.mass, normal).Magnitude;

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
                Vec3 linearVelocity = new Vec3(rb.linearVelocity.x, rb.linearVelocity.y, rb.linearVelocity.z);
                Vec3 vPlane = VectorMathUtils.ProjectOnPlane(linearVelocity, normal);
                //운동 마찰 방향(운동 방향과 반대)
                Vec3 frictionDir = vPlane.Normalized*(-1);
                //운동 마찰 크기 : 운동 마찰 계수 * 수직항력
                float frictionMag = muKinetic * normalForce;
                //운동 마찰력 = 운동 마찰 방향 * 운동 마찰 크기
                Vec3 friction = frictionDir * frictionMag;

                //마찰 힘 적용
                rb.AddForce(new Vector3(friction.x, friction.y, friction.z));
            }
        }

    }
}
