public class PlayerPushPull : MonoBehaviour
{
    public float pushForce = 5f;

    void OnCollisionStay(Collision collision)
    {
        Rigidbody rb = collision.rigidbody;
        if (rb != null && rb.mass < 5f)
        {
            Vector3 dir = collision.contacts[0].point - transform.position;
            dir.y = 0;
            dir = -dir.normalized;
            rb.AddForce(dir * pushForce, ForceMode.Impulse);
        }
    }
}

using UnityEngine;

public class ObjectGrabber : MonoBehaviour
{
    public float grabDistance = 10f;
    public float moveSpeed = 10f;
    public Transform holdPoint;

    private Rigidbody heldObject;//잡은 오브젝트

    void Update()
    {
        //마우스 좌클릭
        if (Input.GetMouseButtonDown(0))
        {
            //잡은 오브젝트가 없으면
            if(heldObject == null)
            {
                //물체 검출
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray,out RaycastHit hit, grabDistance))
                {
                    //잡은 오브젝트 설정
                    if (hit.rigidbody)
                        heldObject = hit.rigidbody;
                }
            }
            else//잡은 오브젝트 해제
            {
                heldObject = null;
            }
        }

        //잡은 오브젝트 존재
        if (heldObject != null)
        {
            Vector3 dir = holdPoint.position - heldObject.position;
            heldObject.linearVelocity = dir * moveSpeed;
        }
    }
}
