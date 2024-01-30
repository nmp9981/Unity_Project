using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float rotateSpeed = 10.0f;
    public float zoomSpeed = 10.0f;
    public float moveAmount = 5.0f;

    private Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        CameraZoom();
        CameraRotate();
        CameraMoving();
    }
    void CameraZoom()
    {
        float distance = Input.GetAxis("Mouse ScrollWheel") * -1 * zoomSpeed;//줌 거리
        if (distance != 0)
        {
            mainCamera.fieldOfView += distance;//화각 변경
        }
    }
    void CameraRotate()
    {
        if (Input.GetMouseButton(1))//마우스 우클릭
        {
            Vector3 rot = transform.rotation.eulerAngles; // 현재 카메라의 각도를 Vector3로 반환
            rot.y += Input.GetAxis("Mouse X") * rotateSpeed; // 마우스 X 위치 * 회전 스피드
            rot.x += -1 * Input.GetAxis("Mouse Y") * rotateSpeed; // 마우스 Y 위치 * 회전 스피드
            Quaternion q = Quaternion.Euler(rot); // Quaternion으로 변환
            q.z = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, q, 2f); // 자연스럽게 회전
        }
    }
    void CameraMoving()
    {
        if (Input.GetKey(KeyCode.W)) transform.position += new Vector3(0,0,Time.deltaTime*moveAmount);
        else if (Input.GetKey(KeyCode.S)) transform.position += new Vector3(0, 0,-1f* Time.deltaTime * moveAmount);
        else if (Input.GetKey(KeyCode.A)) transform.position += new Vector3(-1f * Time.deltaTime * moveAmount,0,0);
        else if (Input.GetKey(KeyCode.D)) transform.position += new Vector3(Time.deltaTime * moveAmount,0,0);
        else if (Input.GetKey(KeyCode.Q)) transform.position += new Vector3(0,-1f * Time.deltaTime * moveAmount,0);
        else if (Input.GetKey(KeyCode.E)) transform.position += new Vector3(0,1f * Time.deltaTime * moveAmount,0);
    }
}
