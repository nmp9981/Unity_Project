using Desc_Web_Server.Data;
using Newtonsoft.Json;
using UnityEngine.Analytics;

// 1. 개별 모터의 상세 데이터
public class MotorState
{
    public int RPM { get; set; } // -1000 ~ 1000
    public float PowerKW { get; set; } // 0.0 ~ 5.0
    public float TorqueNM { get; set; } // 0.0 ~ 3.0
}

// 2. 서버에서 오는 전체 루트 객체 (motor1, motor2 포함)
public class MotorStateResponse
{
    [JsonProperty("motor1")]
    public MotorState Motor1 { get; set; }

    [JsonProperty("motor2")]
    public MotorState Motor2 { get; set; }
}

public class DriveState
{
    public DriveMode ModeSelect { get; set; }
    public int UserSpeed { get; set; }
    public float UserTorque { get; set; }
}
public enum DriveMode
{
    ConstantSpeed = 0,
    SpeedControl = 1,
    EcoMode = 2
}

public class BatteryState
{
    public float Soc { get; set; }
    public float Voltage { get; set; }
    public float Current { get; set; }
    public float RemainKwh { get; set; }
    public float TempC { get; set; }
    public ChargingStatus ChargingStatus { get; set; }
}

public enum ChargingStatus { 
    Charging = 0,
    Discharging = 1,
    Idle = 2,
}

public class AlarmState
{
    public bool BatLow { get; set; }
    public bool BatOverheat { get; set; }
    public bool MotorFault { get; set; }
    public bool EmergencyStop { get; set; }
}


public class EnergyFlowState
{
    public EnergyFlowDirection FlowDirection { get; set; }
    public float FlowRateKW { get; set; }
}

public enum EnergyFlowDirection
{
    BatteryToMotor = 1,
    MotorToBattery = 2,
    Stop = 3
}
