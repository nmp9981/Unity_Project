using UnityEngine;

public class InputSystem : MonoBehaviour
{
    ConnectOutline connectOutline;
    ConnectCrossPipeToSprinkler connectCrossPipeToSprinkler;

    bool isInstallVertical = false;
    bool isInstallCrossPipe = false;

    [SerializeField]
    GameObject mainPipe;

    [SerializeField]
    CameraSystem cam;

    private void Awake()
    {
        connectOutline = GetComponent<ConnectOutline>();
        connectCrossPipeToSprinkler = GetComponent<ConnectCrossPipeToSprinkler>();
    }

    private void Update()
    {
        InputFunction();
        UpdateInputMouseAboutCamera();
    }
    /// <summary>
    /// 키 입력을 받아서 기능을 
    /// </summary>
    void InputFunction()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            connectOutline.StartConnectOutline();
            isInstallVertical = true;
            mainPipe.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (!isInstallVertical)
            {
                return;
            }
            connectOutline.InstallCrossPipe();
            isInstallCrossPipe = true;
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (!isInstallCrossPipe)
            {
                return;
            }
            connectCrossPipeToSprinkler.InstallBranchPipe();
        }

    }
    /// <summary>
    /// 키메라 관련 키 입력
    /// </summary>
    void UpdateInputMouseAboutCamera()
    {
        //마우스 우클릭
        if (Input.GetMouseButton(1))
        {
            //회전 입력
            float xRotateAmount = Input.GetAxis("Mouse X");
            float yRotateAmount = Input.GetAxis("Mouse Y");
            if (xRotateAmount != 0 || yRotateAmount != 0)
            {
                cam.CameraRotate(xRotateAmount, yRotateAmount);
            }

            //이동 입력
            float verticalAmount = Input.GetAxis("Vertical");
            float horizontalAmount = Input.GetAxis("Horizontal");
            bool isUpKey = Input.GetKey(KeyCode.Q);
            bool isDownKey = Input.GetKey(KeyCode.E);

            //이동관련 입력이 없음
            if (verticalAmount == 0 && horizontalAmount == 0 && !isUpKey && !isDownKey)
            {

            }
            else
            {
                //카메라 이동
                cam.CameraMoving(verticalAmount, horizontalAmount, isUpKey, isDownKey);
            }
        }
       
        //휠 이동
        float wheelInputAmount = Input.GetAxis("Mouse ScrollWheel");
        if (wheelInputAmount != 0)
        {
            //카메라 줌 이동
            cam.MoveMouseWheel(wheelInputAmount);
        }
    }
}
