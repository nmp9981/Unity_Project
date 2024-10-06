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
    public float steeringMaxAxis = 15f;

    // 후륜 타이어 마찰력
    WheelFrictionCurve fFrictionBackLeft;
    WheelFrictionCurve sFrictionBackLeft;
    WheelFrictionCurve fFrictionBackRight;
    WheelFrictionCurve sFrictionBackRight;

    // 마찰계수
    float slipRate = 1.0f;
    float handBreakSlipRate = 0.5f;

    //부스터 클래스
    BoosterManager boosterManager;

    private void Awake()
    {
        carRigid = gameObject.GetComponent<Rigidbody>();
        boosterManager = GetComponent<BoosterManager>();
        SteerCarInit();
        // 차량 무게 중심 맞추기
        carRigid.centerOfMass = centerOfMass.transform.localPosition;

        BackFrictionSetting();
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
    }
    //자동차 움직임
    private void FixedUpdate()
    {
        MoveCar();
        SteerCar();
        AnimateWheelMeshs();
        Breaking();
        KartDrift();
    }
    /// <summary>
    /// 기능 : 바퀴 모터 돌리면서 자동차 이동
    /// 가속 조건 : 제한 속도 이하, 위 키 누름, 브레이크 상태가 아님
    /// </summary>
    void MoveCar()
    {
        GameManager.Instance.CarSpeed = carRigid.velocity.magnitude*3.6f;
        
        float verticalAmount = Input.GetAxis("Vertical");
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
    }
    /// <summary>
    /// 기능 : 자동차 회전
    /// 1) 사용자가 직접 회전 제어한다.
    /// </summary>
    void SteerCar()
    {
        //회전 중
        if (Input.GetAxis("Horizontal") != 0)
        {
            wheels[0].steerAngle = steeringMaxAxis * Input.GetAxis("Horizontal");
            wheels[3].steerAngle = steeringMaxAxis * Input.GetAxis("Horizontal");
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
    /// 조건 : 부스터 직후 or 왼쪽 shift키를 누름
    /// </summary>
    void Breaking()
    {
        if (Input.GetKey(KeyCode.RightShift) || GameManager.Instance.IsBooster)
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
            gameObject.transform.position = GameObject.Find("StartPos").transform.position;
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
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetAxis("Horizontal") != 0)
        {
            fFrictionBackLeft.stiffness = handBreakSlipRate;
            wheels[1].GetComponent<WheelCollider>().forwardFriction = fFrictionBackLeft;

            sFrictionBackLeft.stiffness = handBreakSlipRate;
            wheels[1].GetComponent<WheelCollider>().sidewaysFriction = sFrictionBackLeft;

            fFrictionBackLeft.stiffness = handBreakSlipRate;
            wheels[2].GetComponent<WheelCollider>().forwardFriction = fFrictionBackLeft;

            sFrictionBackLeft.stiffness = handBreakSlipRate;
            wheels[2].GetComponent<WheelCollider>().sidewaysFriction = sFrictionBackLeft;

            BoosterGageAmountUP();
        }
        else // 드리프트 상태 아님
        {
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
    /// 2) 속도에 따라 게이지 상승량이 다름 : TODO
    /// </summary>
    void BoosterGageAmountUP()
    {
        GameManager.Instance.CurrentBoosterGage += (0.5f * Time.deltaTime);
        if (GameManager.Instance.CurrentBoosterGage >= 1) boosterManager.BoosterGet();
    }
}
