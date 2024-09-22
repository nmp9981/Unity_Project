using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFuncion : MonoBehaviour
{
    //공 촬영용 카메라
    Camera ballCamera;
    //메인 카메라
    Camera mainCamera;
    //공 오브젝트
    [SerializeField]
    GameObject ball;

    // Start is called before the first frame update
    void Awake()
    {
        CameraSetting();
    }

    void Update()
    {
        FocusCamera();
    }
    /// <summary>
    /// 기능 : ball 촬영 카메라 세팅
    /// 1) 메인카메라 끄기
    /// 2) ball 촬영 카메라 켜기
    /// </summary>
    void CameraSetting()
    {
        mainCamera = Camera.main;
        ballCamera = GameObject.Find("BallCamera").GetComponent<Camera>();

        mainCamera.enabled = false;
        ballCamera.enabled = true;

        ball = GameObject.FindWithTag("Player");
    }
    /// <summary>
    /// 카메라 위치 세팅
    /// </summary>
    void FocusCamera()
    {
        ballCamera.transform.position = ball.transform.position + new Vector3(0,0,-10);
    }
}
