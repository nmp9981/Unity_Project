using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkCameraMode : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private bool _isWalkCameraMode;
    [SerializeField] private float _playerSpeed = 5.0f;
    [SerializeField] private bool _isCollide;
   
    private Vector3 _zoom;
    private Vector3 playerMoveAmount;
    private Vector3 lastGeneralCameraPosition;
    private Vector3 lastWalkCameraPosition;
    private Quaternion lastGeneralCameraRotation;
    private Quaternion _lastWalkCameraRotation = Quaternion.Euler(15, 0, 0);

    public bool CameraWalkMode { get { return _isWalkCameraMode; } set { _isWalkCameraMode = value; } }
    public Vector3 Zoom { get { return _zoom; } set { _zoom = value; } }
    public float PlayerSpeed { get { return _playerSpeed; } }
    public bool IsCollide { get { return _isCollide; } set { _isCollide = value; } }
    public Quaternion LastWalkCameraRotation { get { return _lastWalkCameraRotation; } }

    void Awake()
    {
        gameObject.GetComponent<WalkCameraMode>().enabled = true;//스크립트 활성화
        
        Zoom = new Vector3(gameObject.GetComponent<BoxCollider>().size.x, gameObject.GetComponent<BoxCollider>().size.y, -2.5f * gameObject.GetComponent<BoxCollider>().size.z);
        CameraWalkMode = false;
        IsCollide = false;
        lastWalkCameraPosition = gameObject.transform.position + Zoom;
        lastGeneralCameraPosition = camera.transform.position + Zoom;
        lastGeneralCameraRotation = camera.transform.rotation;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) CameraModeChange();
        WalkCamera();
    }
    void CameraModeChange()
    {
        CameraWalkMode = !CameraWalkMode;
        if (CameraWalkMode)
        {
            lastGeneralCameraPosition = camera.transform.position;
            lastGeneralCameraRotation = camera.transform.rotation;
            camera.transform.position = lastWalkCameraPosition;
            camera.transform.rotation = LastWalkCameraRotation;
        }
        else
        {
            camera.transform.position = lastGeneralCameraPosition;
            camera.transform.rotation = lastGeneralCameraRotation;
        }
    }
    //플레이어와 카메라 이동
    void WalkCamera()
    {
        if (!IsCollide) gameObject.transform.position += new Vector3(0, -2 * Time.deltaTime, 0);//중력
        //여기서 카메라 위치 갱신
        if (CameraWalkMode)
        {
            if (Input.GetKey(KeyCode.Q))
            {
                gameObject.transform.position += new Vector3(0, PlayerSpeed*Time.deltaTime, 0);
            }

            playerMoveAmount = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * PlayerSpeed * Time.deltaTime;
            gameObject.transform.position += playerMoveAmount;
            camera.transform.position = gameObject.transform.position + Zoom;
            lastWalkCameraPosition = camera.transform.position;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Part")
        {
            gameObject.transform.position -= playerMoveAmount/2;
            IsCollide = true;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Part")
        {   
            IsCollide = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Part")
        {
            IsCollide = false;

        }
    }
}
