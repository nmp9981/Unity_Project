using UnityEngine;
using UnityEngine.InputSystem;

public class CameraSystem : MonoBehaviour
{
    #region 변수 모음
    /// <summary>
    /// 조작할 카메라
    /// </summary>
    public static Camera mainCamera { get; set; } = null;

    #region 카메라 이동 관련 변수
    /// <summary>
    /// 카메라 이동량
    /// </summary>
    [SerializeField]
    private float moveAmount = 20f;
    [SerializeField]
    private float dragSpeed = 30f;
    /// <summary>
    /// 카메라 휠 이동량
    /// </summary>
    [SerializeField]
    private float wheelMoveAmount = 500f;

    /// <summary>
    /// 회전량
    /// </summary>
    [SerializeField]
    private float rotateSpeed = 10f;

    /// <summary>
    /// x,y 축 마우스 이동량
    /// </summary>
    float mouseX;
    float mouseY;
    #endregion 카메라 이동 관련 변수
    #endregion 변수 모음

    #region UNITY 생명주기 함수
    void Awake()
    {
        //메인 카메라 가져오기
        GetMainCam();
    }
    #endregion UNITY 생명주기 함수

    #region Awake 함수
    /// <summary>
    /// 메인 카메라 가져오기
    /// </summary>
    void GetMainCam()
    {
        mainCamera = GetComponent<Camera>();
    }
    #endregion Awake 함수

    #region 카메라 기능 함수
    /// </summary>
    /// <param name="xAmount">수직 회전량</param>
    /// <param name="yAmount">수평 회전량</param>
    public void CameraRotate(float xAmount, float yAmount)
    {
        mouseY = xAmount * rotateSpeed * Time.deltaTime;//y축 회전량(수직)
        mouseX = -yAmount * rotateSpeed * Time.deltaTime;//x축 회전량(수평)

        //현재 회전을 쿼터니언으로 가져오기
        Quaternion currentRatation = transform.rotation;

        //y축 수직 회전 : Yaw
        Quaternion yawRataion = Quaternion.Euler(0, mouseY, 0);

        //x축 수직 회전 : Pitch
        Quaternion pitchRataion = Quaternion.Euler(mouseX, 0, 0);

        //회전 (yaw->pitch)
        transform.rotation = yawRataion * currentRatation * pitchRataion;
    }
    /// <summary>
    /// 카메라 드래그 기능
    /// </summary>
    /// <param name="camPos">첫 클릭시 카메라 위치</param>
    /// <param name="clickPoint">첫 클릭시 마우스 입력 위치</param>
    public void CameraViewMoving(Vector3 camPos, Vector2 clickPoint)
    {
        // 원래 위치와 변경된 위치의 차이
        Vector3 difference = mainCamera.ScreenToViewportPoint((Vector2)Input.mousePosition - clickPoint);

        //카메라 이동 방향 설정
        difference.z = difference.y;
        difference.y = 0;
        float y = transform.position.y;

        // 카메라 차이값 만큼 이동
        mainCamera.transform.position = camPos + difference * dragSpeed;
        mainCamera.transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }

    /// <summary>
    /// 카메라 이동
    /// </summary>
    /// <param name="verticalAmount">수직 이동량</param>
    /// <param name="horizontalAmount">수평 이동량</param>
    /// <param name="isUp">위키 입력 여부</param>
    /// <param name="isDown">아래키 입력 여부</param>
    public void CameraMoving(float verticalAmount, float horizontalAmount, bool isUp, bool isDown)
    {
        //이동량 측정
        Vector3 moveAmountVector = transform.forward * verticalAmount + transform.right * horizontalAmount;
        moveAmountVector += transform.up * (isUp ? 1 : 0);
        moveAmountVector += transform.up * (isDown ? -1 : 0);

        // 이동량을 좌표에 반영
        if (moveAmountVector != Vector3.zero)
            transform.position += moveAmountVector * moveAmount * Time.deltaTime;
    }
    /// <summary>
    /// 마우스 휠 이동
    /// </summary>
    /// <param name="wheelAmount">휠 이동량</param>
    public void MoveMouseWheel(float wheelAmount)
    {
        //이동량 측정
        Vector3 moveAmountVector = transform.forward * wheelAmount;

        // 이동량을 좌표에 반영
        if (moveAmountVector != Vector3.zero)
            transform.position += moveAmountVector * wheelMoveAmount * Time.deltaTime;
    }
    #endregion 카메라 기능 함수
}
