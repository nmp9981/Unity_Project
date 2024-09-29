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

    public float Touque = 1200f;
    //최대 차량 회전 각도
    public float steeringMaxAxis = 18f;
    
    //브레이크 여부
    public bool isBreaking = false;
    public float breakPower = 5000f;

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
    /// </summary>
    void MoveCar()
    {
        GameManager.Instance.CarSpeed = carRigid.velocity.magnitude*3.6f;
        carRigid.centerOfMass = centerOfMass.transform.localPosition;
        
        if (GameManager.Instance.CarSpeed <= GameManager.Instance.SpeedLimit)
        {
            for (int i = 0; i < 4; i++)
            {
                wheels[i].motorTorque = Touque * Input.GetAxis("Vertical");
            }
        }
    }
    /// <summary>
    /// 기능 : 자동차 회전
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
    /// </summary>
    void Breaking()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            isBreaking = true;
        }
        else
        {
            isBreaking = false;
        }

        if (isBreaking)
        {
            for(int i=0;i<4; i++)
            {
                wheels[i].brakeTorque = breakPower;
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
