using Desc_Web_Server.Data;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class MotorAllApiConnecter : MonoBehaviour
{
    [SerializeField]
    public ShipMotorControl myMotor; // 기존에 만드신 Motor 클래스 인스턴스
    [SerializeField]
    public ShipMotorControl myMotor2; // 기존에 만드신 Motor 클래스 인스턴스

    private string baseUrl = "http://localhost:6789/api";

    void Start()
    {
        // 통합 업데이트 루프 시작
        StartCoroutine(DataUpdateLoop());
    }

    IEnumerator DataUpdateLoop()
    {
        while (true)
        {
            // 1. Motor 데이터 가져오기 (api/motor)
            yield return StartCoroutine(GetRequest<MotorStateResponse>("/motor/up", (data) => {
                HandleMotorUpdate(data);
            }));
            // 2. Battery 데이터 가져오기 (api/analysis)
            yield return StartCoroutine(GetRequest<BatteryState>("/battery/status", (data) => {
                HandleBatteryUpdate(data);
            }));
            // 3. Energy Flow 데이터 가져오기 (api/analysis)
            yield return StartCoroutine(GetRequest<EnergyFlowState>("/energy-flow/status", (data) => {
                HandleEnergyFlowUpdate(data);
            }));
            // 4. Drive 데이터 가져오기 (api/analysis)
            //yield return StartCoroutine(GetRequest<DriveState>("/analysis", (data) => {
            //    HandleDriveUpdate(data);
            //}));
            // 5. Alarm 데이터 가져오기 (api/analysis)
            yield return StartCoroutine(GetRequest<AlarmState>("/alarm/noti", (data) => {
                HandleAlarmUpdate(data);
            }));
            

            // 2초마다 모든 데이터 갱신
            yield return new WaitForSeconds(2.0f);
        }
    }

    // [공통 통신 메서드] 어떤 데이터 타입(T)이든 처리 가능
    IEnumerator GetRequest<T>(string endpoint, Action<T> callback)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(baseUrl + endpoint))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                // 1. 서버에서 받은 JSON 텍스트 파싱
                string json = webRequest.downloadHandler.text;
                try
                {
                    T data = JsonConvert.DeserializeObject<T>(json);
                    callback?.Invoke(data);
                }
                catch (Exception e)
                {
                    Debug.LogError($"{endpoint} 데이터 파싱 에러: {e.Message}");
                }
            }
        }
    }

    // --- 데이터별 처리 로직 분리 ---
    void HandleMotorUpdate(MotorStateResponse data)
    {
        if (data == null || data.Motor1 == null) return;

        // motor1의 데이터를 사용 예시
        myMotor.currentRpm = data.Motor1.RPM;
        myMotor.motor_power_h = data.Motor1.PowerKW;
        myMotor.currentTorque = data.Motor1.TorqueNM;

        myMotor2.currentRpm = data.Motor2.RPM;
        myMotor2.motor_power_h = data.Motor2.PowerKW;
        myMotor2.currentTorque = data.Motor2.TorqueNM;
    }

    void HandleBatteryUpdate(BatteryState data)
    {
        MotorManager.instance.AmountBattery = data.Soc;
        MotorManager.instance.MotorVoltage = data.Voltage;
        MotorManager.instance.OutputAMP = data.Current;
        MotorManager.instance.RemainBattery = data.RemainKwh;

        //data.BatTempC;
        switch (data.ChargingStatus)
        {
            case ChargingStatus.Charging:
                MotorManager.instance.isBatChg = true;
                break;
            case ChargingStatus.Discharging:
                MotorManager.instance.isBatChg = false;
                break;
            case ChargingStatus.Idle:
                MotorManager.instance.isBatChg = false;
                break;
        }
    }

    void HandleDriveUpdate(DriveState data)
    {
        myMotor.currentSpeed = data.UserSpeed;
        myMotor.currtentControl = data.UserTorque;

        MotorManager.instance.isAcMode = false;
        MotorManager.instance.isGenMode = false;
        MotorManager.instance.isBatMode = false;

        switch (data.ModeSelect)
        {
            case DriveMode.ConstantSpeed:
                MotorManager.instance.isAcMode = true;
                break;
            case DriveMode.SpeedControl:
                MotorManager.instance.isGenMode = true;
                break;
                case DriveMode.EcoMode:
                MotorManager.instance.isBatMode = true;
                break;
        }
    }
    void HandleAlarmUpdate(AlarmState data)
    {
        MotorManager.instance.b_LV_AlramState = data.BatLow;
        MotorManager.instance.b_TP_AlarmState = data.BatOverheat;
        MotorManager.instance.engine1AlarmState = data.MotorFault;
        MotorManager.instance.engine2AlarmState = data.MotorFault;
        MotorManager.instance.eStopAlarmState = data.EmergencyStop;
    }
    void HandleEnergyFlowUpdate(EnergyFlowState data)
    {
        //myMotor.currentSpeed = data.FlowRateKW;
        MotorManager.instance.isAcMode = true;
        MotorManager.instance.isGenMode = false;
        MotorManager.instance.isBatMode = false;

        switch (data.FlowDirection)
        {
            case EnergyFlowDirection.BatteryToMotor:
                break;
            case EnergyFlowDirection.MotorToBattery:
   
                break;
            case EnergyFlowDirection.Stop:
            
                break;
        }
    }
}
