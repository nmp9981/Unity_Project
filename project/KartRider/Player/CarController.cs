using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class CarController : MonoBehaviour
{
    Rigidbody carRigid;
    public WheelCollider[] wheels = new WheelCollider[4];
    public GameObject[] wheelMeshs = new GameObject[4];
    //질량 중심
    public GameObject centerOfMass;

    //최대 차량 회전 각도
    private float steeringMaxAxis;
    private float prevSteerAngle;

    //방향키 이동량
    float horizontalKey;
    float verticalKey;

    // 후륜 타이어 마찰력
    WheelFrictionCurve fFrictionBackLeft;
    WheelFrictionCurve sFrictionBackLeft;
    WheelFrictionCurve fFrictionBackRight;
    WheelFrictionCurve sFrictionBackRight;

    // 마찰계수
    float slipRate = 1.7f;
    float handBreakSlipRate = 0.5f;

    //부스터 클래스
    BoosterManager boosterManager;

    //효과음 시간
    float currentMotorSFXTime = 0.3f;
    float currentDriftSFXTime = 0.3f;
    float motorSFXTime = 0.4f;

    private void Awake()
    {
        carRigid = gameObject.GetComponent<Rigidbody>();
        boosterManager = GetComponent<BoosterManager>();
        SteerCarInit();
        // 차량 무게 중심 맞추기
        carRigid.centerOfMass = centerOfMass.transform.localPosition;

        //타이어 마찰력 세팅
        FrontFrictionSetting();
        BackFrictionSetting();
    }
    // 전륜 타이어 마찰력 세팅
    void FrontFrictionSetting()
    {
        WheelFrictionCurve fFrictionFrontLeft = wheels[0].GetComponent<WheelCollider>().forwardFriction;
        WheelFrictionCurve sFrictionFrontLeft = wheels[0].GetComponent<WheelCollider>().sidewaysFriction;
        WheelFrictionCurve fFrictionFrontRight = wheels[3].GetComponent<WheelCollider>().forwardFriction;
        WheelFrictionCurve sFrictionFrontRight = wheels[3].GetComponent<WheelCollider>().sidewaysFriction;

        fFrictionFrontLeft.stiffness = 1.0f;
        wheels[0].GetComponent<WheelCollider>().forwardFriction = fFrictionFrontLeft;
        sFrictionFrontLeft.stiffness = 1.0f;
        wheels[0].GetComponent<WheelCollider>().sidewaysFriction = sFrictionFrontLeft;
        fFrictionFrontRight.stiffness = 1.0f;
        wheels[3].GetComponent<WheelCollider>().forwardFriction = fFrictionFrontRight;
        sFrictionFrontRight.stiffness = 1.0f;
        wheels[3].GetComponent<WheelCollider>().sidewaysFriction = sFrictionFrontRight;
    }
    //후륜 타이어 마찰력 세팅
    void BackFrictionSetting()
    {
        fFrictionBackLeft = wheels[1].GetComponent<WheelCollider>().forwardFriction;
        sFrictionBackLeft = wheels[1].GetComponent<WheelCollider>().sidewaysFriction;
        fFrictionBackRight = wheels[2].GetComponent<WheelCollider>().forwardFriction;
        sFrictionBackRight = wheels[2].GetComponent<WheelCollider>().sidewaysFriction;
    }
    private void Update()
    {
        ResponKart();
        FlowTime();
        //FrontTireMoveDirection();
    }
    //자동차 움직임
    private void FixedUpdate()
    {
        InputDirectionKey();
        MoveCar();
        SteerCar();
        AnimateWheelMeshs();
        Breaking();
        KartDrift();
    }
    /// <summary>
    /// 기능 : 방향키 입력
    /// </summary>
    void InputDirectionKey()
    {
        horizontalKey = Input.GetAxis("Horizontal");
        verticalKey = Input.GetAxis("Vertical");
    }
    /// <summary>
    /// 기능 : 바퀴 모터 돌리면서 자동차 이동
    /// 가속 조건 : 제한 속도 이하, 위 키 누름, 브레이크 상태가 아님
    /// 주행 가능상태일때만 이동
    /// </summary>
    void MoveCar()
    {
        // 현재 속도 체크
        GameManager.Instance.CarSpeed = carRigid.velocity.magnitude*3.6f;

        // 주행 가능 상태가 아님
        if (GameManager.Instance.IsDriving == false) return;

        float verticalAmount = verticalKey;
        if (GameManager.Instance.CarSpeed <= GameManager.Instance.SpeedLimit && verticalAmount!=0 && !GameManager.Instance.IsBreaking)
        {
            for (int i = 0; i < 4; i++)
            {
                wheels[i].motorTorque = GameManager.Instance.Touque * verticalAmount;
            }
        }
        //제한 속도 초과
        else
        {
            for (int i = 0; i < 4; i++)
            {
                wheels[i].motorTorque = 0;
            }
        }

        // 전후진 키를 누르지 않으면 제동이 걸리도록 한다	
        float decSpeed = 1800;
        if (verticalAmount == 0) {
            for (int i = 0; i < 4; i++)
            {
                wheels[i].brakeTorque = decSpeed;
            }
        } 
        else {
            for (int i = 0; i < 4; i++)
            {
                wheels[i].brakeTorque = 0;
            }
        }
        // sound 재생
        if(verticalAmount!=0 && GameManager.Instance.IsDriving)
        {
            PlayKartMotorSound();
        }
        else
        {
            currentMotorSFXTime = 0.3f;
        }
    }
    /// <summary>
    /// 기능 : 시간 흐름
    /// </summary>
    void FlowTime()
    {
        currentMotorSFXTime += Time.deltaTime;
        currentDriftSFXTime += Time.deltaTime;
    }
    /// <summary>
    /// 기능 : 모터 소리 재생
    /// </summary>
    /// <param name="number"></param>
    void PlayKartMotorSound()
    {
        if(currentMotorSFXTime >= motorSFXTime)
        {
            currentMotorSFXTime = 0;
            SoundManger._sound.PlaySfx((int)SFXSound.Motor);
        }
    }
    /// <summary>
    /// 기능 : 드리프트 소리 재생
    /// </summary>
    /// <param name="number"></param>
    void PlayKartDriftSound()
    {
        if (currentDriftSFXTime >= motorSFXTime)
        {
            currentDriftSFXTime = 0;
            SoundManger._sound.PlaySfx((int)SFXSound.Drift);
        }
    }
    /// <summary>
    /// 기능 : 앞바퀴 2개를 이동방향으로 향하기	
    /// 추가 로직
    /// </summary>
    void FrontTireMoveDirection()
    {	
        wheelMeshs[0].transform.Rotate(Vector3.up, wheels[0].steerAngle - prevSteerAngle, Space.World);
        wheelMeshs[3].transform.Rotate(Vector3.up, wheels[3].steerAngle - prevSteerAngle, Space.World);
        prevSteerAngle = wheels[3].steerAngle;
    }
    /// <summary>
    /// 기능 : 자동차 회전
    /// 1) 사용자가 직접 회전 제어한다.
    /// </summary>
    void SteerCar()
    {
        //회전 중
        if (horizontalKey != 0)
        {
            wheels[0].steerAngle = steeringMaxAxis * horizontalKey;
            wheels[3].steerAngle = steeringMaxAxis * horizontalKey;

            ////바퀴 회전 효과(추가 로직)
            //for(int idx = 0; idx < 4; idx++)
            //{
            //    wheelMeshs[idx].transform.Rotate(0,wheels[idx].rpm / 60 * 360 * Time.fixedDeltaTime, 0);
            //}
        }
        else
        {
            wheels[0].steerAngle = 0;
            wheels[3].steerAngle = 0;
        }
        
    }
    /// <summary>
    /// 기능 : 자동차 회전값 초기화
    /// 1) 자동차 바퀴는 처음에 똑바로 향하게한다.
    /// </summary>
    void SteerCarInit()
    {
        for (int i = 0; i < 4; i ++)
        {
            wheels[i].steerAngle = 0;
        }
    }
    /// <summary>
    /// 기능 : 자동차 회전 
    /// 1) 바퀴가 직접 움직인다.
    /// </summary>
    void AnimateWheelMeshs()
    {
        Vector3 pos = Vector3.zero;
        Quaternion rot = Quaternion.identity;
        for(int i = 0; i < 4; i++)
        {
            wheels[i].GetWorldPose(out pos, out rot);
            wheelMeshs[i].transform.position = pos;
            wheelMeshs[i].transform.rotation = rot;
        }
    }
    /// <summary>
    /// 기능 : 브레이크
    /// 조건1 : 부스터 직후 or 왼쪽 shift키를 누름
    /// 조건2 : 주행 가능상태가 아님
    /// </summary>
    void Breaking()
    {
        if (Input.GetKey(KeyCode.RightShift) || GameManager.Instance.IsBooster)
        {
            GameManager.Instance.IsBreaking = true;
        }else if(GameManager.Instance.IsDriving == false)
        {
            GameManager.Instance.IsBreaking = true;
        }
        else
        {
            GameManager.Instance.IsBreaking = false;
        }

        if (GameManager.Instance.IsBreaking)
        {
            for(int i=0;i<4; i++)
            {
                wheels[i].brakeTorque = GameManager.Instance.BreakPower;
            }
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                wheels[i].brakeTorque = 0;
            }
        }
    }
    void ResponKart()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            gameObject.transform.position = MapManager.SearchNearResponposition(gameObject.transform.position);
        }
    }
    /// <summary>
    /// 기능 : 드리프트
    /// 원리 : 후륜 타이어의 마찰계수 조절
    /// 1) 왼쪽 shift 누르고 좌우 방향키를 누른 상태에서만 작동
    /// </summary>
    void KartDrift()
    {
        
        // 드리프트 상태
        if (Input.GetKey(KeyCode.LeftShift) && horizontalKey != 0)
        {
            steeringMaxAxis = 75;
            PlayKartDriftSound();
            wheels[1].brakeTorque = GameManager.Instance.BreakPower;
            wheels[2].brakeTorque = GameManager.Instance.BreakPower;

            fFrictionBackLeft.stiffness = handBreakSlipRate;
            wheels[1].GetComponent<WheelCollider>().forwardFriction = fFrictionBackLeft;

            sFrictionBackLeft.stiffness = slipRate;
            wheels[1].GetComponent<WheelCollider>().sidewaysFriction = sFrictionBackLeft;

            fFrictionBackLeft.stiffness = handBreakSlipRate;
            wheels[2].GetComponent<WheelCollider>().forwardFriction = fFrictionBackLeft;

            sFrictionBackLeft.stiffness = slipRate;
            wheels[2].GetComponent<WheelCollider>().sidewaysFriction = sFrictionBackLeft;

            BoosterGageAmountUP();
        }
        else // 드리프트 상태 아님
        {
            steeringMaxAxis = 25;
            currentDriftSFXTime = 0.3f;
            
            wheels[1].brakeTorque = 0;
            wheels[2].brakeTorque = 0;

            fFrictionBackLeft.stiffness = slipRate;
            wheels[1].GetComponent<WheelCollider>().forwardFriction = fFrictionBackLeft;

            sFrictionBackLeft.stiffness = slipRate;
            wheels[1].GetComponent<WheelCollider>().sidewaysFriction = sFrictionBackLeft;

            fFrictionBackLeft.stiffness = slipRate;
            wheels[2].GetComponent<WheelCollider>().forwardFriction = fFrictionBackLeft;

            sFrictionBackLeft.stiffness = slipRate;
            wheels[2].GetComponent<WheelCollider>().sidewaysFriction = sFrictionBackLeft;
        }
    }
    /// <summary>
    /// 기능 : 부스터 게이지 증가
    /// 1) 게이지가 1 이상인 경우 부스터 획득
    /// 2) 속도에 따라 게이지 상승량이 다름
    /// </summary>
    void BoosterGageAmountUP()
    {
        float gageChargeAmount = GameManager.Instance.CarSpeed / 300f;
        GameManager.Instance.CurrentBoosterGage += (gageChargeAmount * Time.deltaTime);
        if (GameManager.Instance.CurrentBoosterGage >= 1) boosterManager.BoosterGet();
    }
}
