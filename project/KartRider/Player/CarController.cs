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
    public float steeringMaxAxis = 15fusing System.Collections;
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
  
    private void Awake()
    {
        carRigid = gameObject.GetComponent<Rigidbody>();
        SteerCarInit();
        // 차량 무게 중심 맞추기
        carRigid.centerOfMass = centerOfMass.transform.localPosition;
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
        for(int i = 0; i < 4; i+=3)
        {
            wheels[i].steerAngle = steeringMaxAxis * Input.GetAxis("Horizontal");
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
        if (Input.GetKey(KeyCode.LeftShift) || GameManager.Instance.IsBooster)
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
}

  
    private void Awake()
    {
        carRigid = gameObject.GetComponent<Rigidbody>();
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
    }
    /// <summary>
    /// 기능 : 바퀴 모터 돌리면서 자동차 이동
    /// 가속 조건 : 제한 속도 이하, 위 키 누름, 브레이크 상태가 아님
    /// </summary>
    void MoveCar()
    {
        GameManager.Instance.CarSpeed = carRigid.velocity.magnitude*3.6f;
        carRigid.centerOfMass = centerOfMass.transform.localPosition;
        
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
        for(int i = 0; i < 4; i+=3)
        {
            wheels[i].steerAngle = steeringMaxAxis * Input.GetAxis("Horizontal");
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
        if (Input.GetKey(KeyCode.LeftShift) || GameManager.Instance.IsBooster)
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
}
