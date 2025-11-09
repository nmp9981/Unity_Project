using UnityEngine;

public class SlopeTest : MonoBehaviour
{
    public float slideThreshold = 0.6f;//미끄러지기 시작하는 임계각도
    public float slideSpeed = 5f;
    private Rigidbody rb;

    void Start() => rb = GetComponent<Rigidbody>();

    void FixedUpdate()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 1.2f))//땅을 감지해야함
        {
            Vector3 normal = hit.normal;
            float slopeAngle = Vector3.Angle(normal, Vector3.up);

            //경사각이 임계값보다 크면 미끄러지기 시작
            //경사면을 따라 아래로 미끄러지는 방향 벡터
            if (slopeAngle > slideThreshold * 45f)//0.6일경우 27도가 임계값
                rb.AddForce(Vector3.ProjectOnPlane(Vector3.down, normal) * slideSpeed);
        }
    }
}
