using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkCameraMode : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private bool _isWalkCameraMode;
    [SerializeField] private float _playerSpeed = 5f;
    [SerializeField] private float _playerRotateSpeed = 15f;
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
    public float PlayerRotateSpeed { get { return _playerRotateSpeed; } }
    public bool IsCollide { get { return _isCollide; } set { _isCollide = value; } }
    public Quaternion LastWalkCameraRotation { get { return _lastWalkCameraRotation; } }

    void Awake()
    {
        gameObject.GetComponent<WalkCameraMode>().enabled = true;//스크립트 활성화
        
        Zoom = new Vector3(0, 0.5f*gameObject.GetComponent<BoxCollider>().size.y, -1f * gameObject.GetComponent<BoxCollider>().size.z);
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
    private void LateUpdate()
    {
        WalkCameraRotation();
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
            //if (CheckHitWall(playerMoveAmount)) playerMoveAmount = Vector3.zero;

            gameObject.transform.position += playerMoveAmount;

            if (Input.GetMouseButton(1))//마우스 우클릭
            {
                Vector3 rot = this.gameObject.transform.rotation.eulerAngles; // 현재 카메라의 각도를 Vector3로 반환
                rot.y += Input.GetAxis("Mouse X") * PlayerRotateSpeed; // 마우스 X 위치 * 회전 속도
                rot.x += -1 * Input.GetAxis("Mouse Y") * PlayerRotateSpeed; // 마우스 Y 위치 * 회전 속도
                Quaternion q = Quaternion.Euler(rot); // Quaternion으로 변환
                q.z = 0;//z축 고정
                this.gameObject.transform.rotation = Quaternion.Slerp(transform.rotation, q, 2f); // 자연스럽게 회전
            }
        }
    }
    private bool CheckHitWall(Vector3 movement)
    {
        // 움직임에 대한 로컬 벡터를 월드 벡터로 변환해준다.
        movement = transform.TransformDirection(movement);
        // scope로 ray 충돌을 확인할 범위를 지정할 수 있다.
        float scope = 5f;

        List<Vector3> rayPositions = new List<Vector3>();
        rayPositions.Add(transform.position + Vector3.forward);
        /*
        rayPositions.Add(transform.position + Vector3.up);
        rayPositions.Add(transform.position + Vector3.left);
        rayPositions.Add(transform.position + Vector3.right);
        */
        // 디버깅을 위해 ray를 화면에 그린다.
        foreach (Vector3 pos in rayPositions)
        {
            Debug.DrawRay(pos, movement * scope, Color.red);
        }

        // ray와 벽의 충돌을 확인한다.
        foreach (Vector3 pos in rayPositions)
        {
            if (Physics.Raycast(pos, movement, out RaycastHit hit, scope))
            {
                if (hit.collider.CompareTag("Part"))
                    return true;
            }
        }
        return false;
    }
    void WalkCameraRotation()
    {
        if (!CameraWalkMode) return;

        camera.transform.rotation = this.gameObject.transform.rotation;
        camera.transform.position = this.gameObject.transform.position + Zoom;
        lastWalkCameraPosition = camera.transform.position;
        
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
