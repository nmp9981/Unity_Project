using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.UIElements.UxmlAttributeDescription;

#region 구조체 및 클래스
public enum DriveType
{
    FWD,
    RWD,
    AWD
}
/// <summary>
/// 노면 타입
/// </summary>
public enum SurfaceType
{
    Dry,
    Wet,
    Snow
}
/// <summary>
/// 타이어 종류
/// </summary>
public enum TireCompound
{
    Soft,
    Medium,
    Hard
}
/// <summary>
/// 코너 종류
/// </summary>
public enum CornerPhase
{
    None,
    Entry,
    Apex,
    Exit
}
/// <summary>
/// 코너 문제 유형
/// </summary>
enum CornerIssue
{
    None,
    Understeer,
    Oversteer,
    TooFastEntry,
    TooSlowExit
}
/// <summary>
/// 재생용
/// </summary>
public struct ReplayFrame
{
    public float time;

    public Vector3 position;
    public float yaw;

    public float throttle;
    public float brake;
    public float steer;
}

[System.Serializable]
public struct TelemetryFrame
{
    public float time;

    // 차량 상태
    public float Ux;
    public float Uy;
    public float yawRate;
    public float steer;

    // 타이어 슬립
    public float slipFL, slipFR, slipRL, slipRR;

    // 타이어 온도
    public float tempFL, tempFR, tempRL, tempRR;

    // 타이어 하중 (핵심 🔥)
    public float FzFL, FzFR, FzRL, FzRR;
}
/// <summary>
/// 코너 정보
/// </summary>
[System.Serializable]
public class Corner
{
    public int start;
    public int end;

    public int entryStart;
    public int apex;
    public int lateApex;
    public int exitEnd;
}
/// <summary>
/// 지점 정보
/// </summary>
[System.Serializable]
public class Waypoint
{
    public Vec3 position;
}
/// <summary>
/// 테스트 결과
/// </summary>
[System.Serializable]
public class TestResult
{
    public string setupName;
    public float bestLap;
}
#endregion

public class DOF3RigidBody : MonoBehaviour
{
    float delta = 0.05f;//고정 조향각
    float currentUx, prevUx;
    float a = 1.2f;//55%
    float b = 1.6f;//45%
    float h = 0.5f;//세단 높이, CG Height
    float L = 2.8f;
    float m = 1500;
    float FzFront, FzRear, staticFront, staticRear;
    float mu = 1.0f;//마른 아스팔트 기준
    float Cf = 40000;
    float Cr = 30000;
    float vy_dot, r_dot;
    float wheelbase;  // 축간 거리
    float absStrength;
    float baseAbsStrength;
    float driverBrake;
    float brakeBias = 0.7f; // 앞 70%

    float Iz = 2500;//yaw 관성
    float g = 9.81f;
    Vector3 local;

    float escThreshold = 0.2f;  // rad/s
    float steerThreshold = 0.02f;   // rad
    float minSpeed = 5f;        // m/s

    DriveType driveType = DriveType.RWD;

    CustomRigidBody rb;

    // ==== 공기 저항 ====
    float airDensity = 1.2f;   // 공기 밀도
    float Cd = 0.32f;          // 항력 계수
    float frontalArea = 2.2f;  // 전면 면적
    float Cl = 1.2f; // 다운포스 계수 (차량마다 다름)

    // ===== 상태 =====
    float Ux;//종방향 속도
    float Uy;//횡방향 속도
    float Vy;
    float ay;
    float ax;
    float prevVy;

    // ==== 타이어 제어용 변수 ====
    float slipFL, slipFR, slipRL, slipRR;
    float wFL, wFR, wRL, wRR; // wheel angular velocity
    float R = 0.3f; // 타이어 반지름

    TireCompound currentCompound = TireCompound.Medium;//타이어 종류
    float compoundGrip = 1.0f;//기본 접지력
    float compoundWearRate = 1.0f;//닳는 속도
    float compoundHeatGain = 1.0f;//열 오르는 속도

    // ==== roll ====
    float roll;//차체 기울기, roll angle (rad)
    float rollRate;// roll rate (rad/s)
    float rollAcc;
    float rollTransferFront;
    float rollTransferRear;

    // ==== 회전 ====
    float r;//yaw rate
    float yaw;

    //==== 위치 ====
    Vec3 position;

    // ==== Ackermann factor ====
    float ackermannFactor = 1.0f; // 0 ~ 1

    // ==== camber ====
    float camberFront = -2f * MathUtility.Deg2Rad; // -2도
    float camberRear = -1.5f * MathUtility.Deg2Rad;

    // ===== 좌우 하중 =====
    float staticFz;
    float FzFL;
    float FzFR;
    float FzRL;
    float FzRR;

    // ==== 전 방향 하중 ====
    float FxFL;
    float FxFR;
    float FxRL;
    float FxRR;
    float FyFL;
    float FyFR;
    float FyRL;
    float FyRR;

    // ===== Setup =====
    // Spring
    public float springFront = 30000f;
    public float springRear = 28000f;

    // Progressive
    public float spring2Front = 90000f;
    public float spring2Rear = 70000f;

    // Damper
    public float damperCompFront = 3000f;
    public float damperRebFront = 7000f;

    public float damperCompRear = 2500f;
    public float damperRebRear = 6000f;

    // Anti Roll Bar
    public float arbFront = 15000f;
    public float arbRear = 10000f;

    // Tire
    public float baseMuSetup = 1.2f;
    public float loadSensitivitySetup = 0.00002f;

    // ==== Load Sensivity ====
    float baseMu = 1.2f;
    float loadSensitivity = 0.00002f;

    // ===== Differential =====
    public float lsdStrength = 0.8f;   // 0 ~ 1
    public float lsdPower = 2.0f;      // 민감도

    // ==== 엔진 모델 ====
    float throttle; // 0~1, 엔진 힘
    float FxEngine => throttle * 4000f;
    float brakeInput; // 0~1, 브레이크
    float brakeForce;
    float dragCoeff = 0.4f;// 공기저항
    float rollingCoeff = 30f;
    float maxSpeed = 55f; // m/s (약 200km/h)
    float FxDrag;
    float brake;
    float maxSteer;
    float maxAccel;
    float maxBrake;
    float maxBrakeForce = 12000f;
    float maxDriveForce = 8000f;
    float engineRPM;
    float downforceCoeff = 20f; // 튜닝값, 공기저항
    float aeroBalance = 0.5f; // 0 = rear, 1 = front

    // ==== Suspension ====
    float springK = 30000f;     // 스프링 강성
    float springK2 = 80000f; // 비선형 계수 (핵심)
    float restLength = 0.3f;    // 기본 길이

    float susFL, susFR, susRL, susRR; // 현재 서스펜션 길이

    // ==== Damper ====
    float damperCompression = 3000f; // 눌릴 때
    float damperRebound = 7000f;     // 올라올 때 (더 강함 ⭐)

    float prevSusFL, prevSusFR, prevSusRL, prevSusRR;
    float prevFzFL, prevFzFR, prevFzRL, prevFzRR;

    // ==== Tire Wear ====
    float tireWearFL = 1.0f;
    float tireWearFR = 1.0f;
    float tireWearRL = 1.0f;
    float tireWearRR = 1.0f;

    float wearRate = 0.00001f;
    float wearTempFactor = 0.00002f;

    // ===== 종력 =====
    float FxTotal;
    float FxDrive;
    float FxDriveRear;
    float FxBrake;
    float FxBrakeRear;
    float FxFront;
    float FxRear;
    float FxRoll;

    // ===== 슬립각 =====
    float alphaF;
    float alphaR;
    float alphaFL;
    float alphaFR;

    // ===== 하중 =====
    float FzF;
    float FzR;

    // ===== 횡력 =====
    float Fy_f;
    float Fy_r;

    // ===== ECS =====
    public bool escEnabled = false;
    public float escGain = 2500f;
    public float maxEscYaw = 4000f;
    public float escMaxYaw;
    public float maxEscBrake = 1000f;
    public float currentEscBrake;
    
    // === TCS ====
    public float tcsFactor;

    // ==== Anti Roll Bar ====
    float arbStiffnessFront = 15000f;
    float arbStiffnessRear = 10000f;

    // ==== AnimationCurve ====
    AnimationCurve torqueCurve;
    float maxTorque = 300f;
    float maxRPM = 7000f;

    // ==== 제어용 변수 ====
    float slipTargetBrake = -0.1f;
    float slipTargetAccel = 0.1f;
    float slipThreshold = 0.02f;
    float softThreshold = 0.02f;
    float hardThreshold = 0.06f;
    float smoothingSpeed = 5f;

    // ==== Tire Temperature ====
    float tireTempFL = 70f;
    float tireTempFR = 70f;
    float tireTempRL = 70f;
    float tireTempRR = 70f;

    float heatGain = 0.0005f;
    float coolingRate = 0.5f;
    float ambientTemp = 25f;

    // Roll 준비
    float trackWidth = 1.6f;   // m
    float Ixx = 600f;          // roll inertia
    float KphiFront = 18000f;
    float KphiRear = 12000f;// roll stiffness
    float Cphi = 50000f;        // roll damping
    float Kphi => KphiFront+KphiRear;
    float Iphi = 450f;

    //기어
    int currentGear = 1;
    float upshiftRPM = 6000f;
    float downshiftRPM = 2000f;

    float shiftDelay = 0.5f;//쿨타임
    float lastShiftTime = 0f;

    int maxGear = 5;
    int minGear = 1;
    float[] gearRatios = { 0f, 3.5f, 2.1f, 1.4f, 1.0f, 0.8f };
    float finalDrive = 3.2f;

    // ==== 노면 ====
    SurfaceType currentSurface = SurfaceType.Dry;
    float surfaceGrip = 1.0f;
    float surfaceSlipMultiplier = 1.0f;

    // ==== 온도 ====
    List<TelemetryFrame> telemetryLog;
    float telemetryTimer;
    float telemetryInterval = 0.05f; // 20Hz
    int maxSamples = 5000;

    //Transform
    Transform3D transform3D = new Transform3D();
    CustomCollider3D collider3D;

    // ==== 입력 ====
    float inputThrottle;
    float inputBrake;
    float inputSteer;

    [Header("차량 데이터")]
    public CarSetup currentSetup;

    [Header("로그 데이터")]
    public List<ReplayFrame> replayLog = new List<ReplayFrame>(5000);
    bool isReplaying = false;
    int replayIndex = 0;

    [Header("AI Driver")]
    public bool useAI = true;
    public List<Waypoint> path = new List<Waypoint>();
    public List<Waypoint> racingLine = new List<Waypoint>();

    int currentIndex = 0;
    public int lookAhead = 5;
    public float waypointThreshold = 5f;
    float smoothSteer = 0f;

    //랩 타이머
    float lapStartTime;
    float bestLapTime = float.MaxValue;
    float lastLapTime;
    int lapCount = 0;
    public Transform startLine;
    Vec3 lastPosition;

    // ==== 곡률 관련 ====
    float minLookAhead = 4f;
    float maxLookAhead = 20f;

    float curvatureGain = 5f;
    float steerLookReduce = 0.5f;

    // ====코너====
    List<Corner> corners = new List<Corner>();

    //결과 저장
    List<TestResult> results = new List<TestResult>();
    List<float> speedProfile = new List<float>();

    private void Start()
    {
        InitForce();
        ApplySetup(currentSetup);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Q)) arbFront += 100f;
        if (Input.GetKey(KeyCode.A)) arbFront -= 100f;

        if (Input.GetKey(KeyCode.W)) arbRear += 100f;
        if (Input.GetKey(KeyCode.S)) arbRear -= 100f;

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartReplay();
        }
    }
    #region 전체 흐름
    private void FixedUpdate()
    {
        if (isReplaying)
        {
            PlayReplay();
            return; // 🔥 물리 완전 차단
        }
        // 0. 입력
        if (useAI)
        {
            UpdateCurrentIndex(); // 🔥 추가
            UpdateAIInput();
        }
        else
        {
            GetInput();
        }


        UpdateSlip();
        ApplyTCS();
        UpdateGear();
        UpdateEngineRPM();
        // 1. Longitudinal force (새로 추가된 것), 종방향
        ComputeLongitudinal();   // ax 계산됨

        // 차체 거동
        // 2. Slip angle, 횡방향 준비
        ComputeSlipAngle();
        // 4. Roll + Pitch dynamics, 차체 거동
        UpdateRollDynamics(ay, Time.fixedDeltaTime);//roll 계산
        // 5. Weight transfer → Fz 완성, 하중 통합
        UpdateWheelLoads();

        //제어 시스템
        ApplyABS();

        UpdateSurfaceProperties();
        UpdateCompoundProperties();

        // 6. Tire force (이제는 새로운 Fz 기반), 타이어 힘
        ComputeTireForce();
        ComputeLongitudinalTireForce();//Fx 계산

        UpdateTireTemperature();  // 기존
        UpdateTireWear();         // 🔥 추가

        ApplyTemperatureGrip();   // 기존
        ApplyWearGrip();          // 🔥 추가
        ApplySurfaceEffect();
        ApplyCompoundGrip();

        // 👉 🔥 여기 추가
        UpdateTireTemperature();

        // 👉 🔥 여기 추가
        ApplyTemperatureGrip();


        // 3. ay 계산
        ay = (Fy_f + Fy_r) / m;
        ApplyESC();
        // 7. 슬립 결합
        ApplyCombinedSlipAll();//Fx, Fy조절
        
        // 8. 마찰 제한
        ApplyTireCoupling();

        // 9. 차량 운동 적용
        IntegrateVehicle();

        // AI 운전→ Telemetry 기록→ Lap 측정→ Setup 비교→ 결과 출력
        // 랩 타이머
        UpdateLapTimer();

        RecordTelemetry();
        RecordReplay(); // 🔥 추가

        ApplySpeedControlAdvanced();
        ApplySmartBraking();
    }
    #endregion

    #region 재생 관련
    /// <summary>
    /// 재생 함수
    /// </summary>
    void PlayReplay()
    {
        if (replayIndex >= replayLog.Count)
        {
            isReplaying = false;
            return;
        }

        var f = replayLog[replayIndex];

        transform.position = f.position;
        yaw = f.yaw;

        transform.rotation = QuaternionUtility.Euler(0f, yaw * MathUtility.Rad2Deg, 0f);

        replayIndex++;
    }
    /// <summary>
    /// 재생 시작
    /// </summary>
    void StartReplay()
    {
        isReplaying = true;
        replayIndex = 0;
    }
    #endregion

    #region 초기화
    /// <summary>
    /// 데이터 셋업
    /// </summary>
    /// <param name="setup"></param>
    void ApplySetup(CarSetup setup)
    {
        // Spring
        springFront = setup.springFront;
        springRear = setup.springRear;

        spring2Front = setup.spring2Front;
        spring2Rear = setup.spring2Rear;

        // Damper
        damperCompFront = setup.damperCompFront;
        damperRebFront = setup.damperRebFront;

        damperCompRear = setup.damperCompRear;
        damperRebRear = setup.damperRebRear;

        // ARB
        arbStiffnessFront = setup.arbFront;
        arbStiffnessRear = setup.arbRear;

        // Tire
        baseMu = setup.baseMu;
        loadSensitivity = setup.loadSensitivity;

        // 온도
        telemetryLog = new List<TelemetryFrame>(5000);
    }

    /// <summary>
    /// 힘 초기화
    /// </summary>
    void InitForce()
    {
        staticFz = m * g;

        staticFront = staticFz * (b / (a + b));
        staticRear = staticFz * (a / (a + b));

        FzFront = staticFront;
        FzRear = staticRear;

        // 좌우 초기 하중도 세팅
        FzFL = staticFront * 0.5f;
        FzFR = staticFront * 0.5f;
        FzRL = staticRear * 0.5f;
        FzRR = staticRear * 0.5f;

        //정적 하중 계산
        staticFront = m * g * (b / wheelbase);
        staticRear = m * g * (a / wheelbase);
    }
    #endregion

    #region 입력
    /// <summary>
    /// 입력값 받기
    /// </summary>
    void GetInput()
    {

    }
    #endregion

    #region 차량 물리
    /// <summary>
    /// 자동 변속 로직
    /// </summary>
    void UpdateGear()
    {
        if (Time.time - lastShiftTime < shiftDelay)
            return;

        if (engineRPM > upshiftRPM && currentGear < maxGear)//기어 증가, RPm 상승
        {
            currentGear++;
            lastShiftTime = Time.time;
        }
        else if (engineRPM < downshiftRPM && currentGear > minGear)//기어 감소, RPm 하락
        {
            currentGear--;
            lastShiftTime = Time.time;
        }
    }
    /// <summary>
    /// 종방향 힘 계산 + 속도 업데이트, ux,ax 계산
    /// </summary>
    void ComputeLongitudinal()
    {
        // 1. 입력
        float throttle = inputThrottle; // 0 ~ 1
        float brake = inputBrake;       // 0 ~ 1

        // 2. Drive Force (엔진)
        // 🔥 TCS 적용
        float finalThrottle = throttle * (1f - tcsFactor);

        // 1. 엔진 토크
        float torque = GetEngineTorque(engineRPM);

        // 2. 기어 적용
        float gearRatio = gearRatios[currentGear];
        float wheelTorque = torque * gearRatio * finalDrive;

        // 3. TCS
        float finalTorque = wheelTorque * (1f - tcsFactor);

        // 4. 디퍼렌셜
        float slipDiff = slipRR - slipRL;
        float bias = MathUtility.ClampValue(slipDiff * 0.5f, -0.5f, 0.5f);

        // ===== Drive Distribution =====

        float torqueFL = 0f;
        float torqueFR = 0f;
        float torqueRL = 0f;
        float torqueRR = 0f;

        switch (driveType)
        {
            case DriveType.FWD:
                {
                    slipDiff = slipFR - slipFL;
                    bias = MathUtility.ClampValue(slipDiff * 0.5f, -0.5f, 0.5f);

                    torqueFL = finalTorque * (0.5f - bias);
                    torqueFR = finalTorque * (0.5f + bias);
                    break;
                }

            case DriveType.RWD:
                {
                    slipDiff = slipRR - slipRL;
                    bias = MathUtility.ClampValue(slipDiff * 0.5f, -0.5f, 0.5f);

                    torqueRL = finalTorque * (0.5f - bias);
                    torqueRR = finalTorque * (0.5f + bias);
                    break;
                }

            case DriveType.AWD:
                {
                    float frontSplit = 0.5f; // 50:50 (튜닝 가능 ⭐)

                    float torqueFront = finalTorque * frontSplit;
                    float torqueRear = finalTorque * (1f - frontSplit);

                    // Front diff
                    float slipDiffF = slipFR - slipFL;
                    float biasF = MathUtility.ClampValue(slipDiffF * 0.5f, -0.5f, 0.5f);

                    torqueFL = torqueFront * (0.5f - biasF);
                    torqueFR = torqueFront * (0.5f + biasF);

                    // Rear diff
                    float slipDiffR = slipRR - slipRL;
                    float biasR = MathUtility.ClampValue(slipDiffR * 0.5f, -0.5f, 0.5f);

                    torqueRL = torqueRear * (0.5f - biasR);
                    torqueRR = torqueRear * (0.5f + biasR);
                    break;
                }
            default:
                break;
        }
        // Front
        ApplyLSD(ref torqueFL, ref torqueFR, slipFL, slipFR);
        // Rear
        ApplyLSD(ref torqueRL, ref torqueRR, slipRL, slipRR);

        // ===== Force 변환 =====
        FxFL = torqueFL / R;
        FxFR = torqueFR / R;
        FxRL = torqueRL / R;
        FxRR = torqueRR / R;

        // 3. Brake Force
        brakeForce = brake * maxBrakeForce;
        float frontBrake = brakeForce * brakeBias;
        float rearBrake = brakeForce * (1f - brakeBias);

        // Front
        FxFL -= frontBrake * 0.5f;
        FxFR -= frontBrake * 0.5f;

        // Rear
        FxRL -= rearBrake * 0.5f;
        FxRR -= rearBrake * 0.5f;

        // 속도 방향 반대로 작용
        if (MathUtility.Abs(Ux) > 0.1f)
            FxBrake *= MathUtility.Sign(Ux);
        else
            FxBrake = 0f;
        // 4. Drag (공기저항)
        FxDrag = ComputeDrag(MathUtility.Abs(Ux)) * MathUtility.Sign(Ux);

        // 5. Rolling Resistance
        FxRoll = rollingCoeff * Ux;

        // 9. 정지 안정화 (중요)
        if (MathUtility.Abs(Ux) < 0.05f && throttle < 0.01f)
            Ux = 0f;
    }

    /// <summary>
    /// LSD 적용
    /// </summary>
    /// <param name="torqueL"></param>
    /// <param name="torqueR"></param>
    /// <param name="slipL"></param>
    /// <param name="slipR"></param>
    void ApplyLSD(ref float torqueL, ref float torqueR, float slipL, float slipR)
    {
        float slipDiff = slipR - slipL;

        float norm = MathUtility.ClampValue(slipDiff, -1f, 1f);
        float shaped = MathUtility.Sign(norm) * MathUtility.Pow(MathUtility.Abs(norm), lsdPower);

        float bias = shaped * lsdStrength * 0.5f;

        float total = torqueL + torqueR;

        torqueL = total * (0.5f - bias);
        torqueR = total * (0.5f + bias);
    }
    /// <summary>
    /// 공기저항 계산
    /// </summary>
    /// <param name="speed"></param>
    /// <returns></returns>
    float ComputeDrag(float speed)
    {
        return 0.5f * airDensity * Cd * frontalArea * speed * speed;
    }
    /// <summary>
    /// 다운포스 계산
    /// </summary>
    /// <param name="speed"></param>
    /// <returns></returns>
    float ComputeDownforce(float speed)
    {
        return 0.5f * airDensity * Cl * frontalArea * speed * speed;
    }
    /// <summary>
    /// 슬립각 계산
    /// </summary>
    void ComputeSlipAngle()
    {
        // 2. 슬립각 계산
        float U = MathUtility.Abs(Ux);
        if (U < 0.5f)//저속에서는 타이어 모델을 끈다.
        {
            alphaFL = alphaFR = 0f;
            alphaR = 0f;
            return;
        }

        // 🔥 1. Ackermann
        float steerLeft, steerRight;
        ComputeSteeringAngles(out steerLeft, out steerRight);

        // 🔥 2. 앞바퀴 (좌/우 분리)
        alphaFL = steerLeft - (Vy + a * r) / U;
        alphaFR = steerRight - (Vy + a * r) / U;

        // 🔥 3. 뒤는 그대로
        alphaR = (b * r - Vy) / U;
    }
    /// <summary>
    /// 조향각 생성, 조향 입력 변형
    /// </summary>
    /// <param name="steerLeft"></param>
    /// <param name="steerRight"></param>
    void ComputeSteeringAngles(out float steerLeft, out float steerRight)
    {
        steerLeft = delta;
        steerRight = delta;

        if (MathUtility.Abs(delta) < 0.001f)
            return;

        float turnRadius = L / MathUtility.Tan(MathUtility.Abs(delta));

        float inner = MathUtility.Atan(L / (turnRadius - trackWidth * 0.5f));
        float outer = MathUtility.Atan(L / (turnRadius + trackWidth * 0.5f));

        if (delta > 0f) // 좌회전
        {
            steerLeft = inner;
            steerRight = outer;
        }
        else
        {
            steerLeft = outer;
            steerRight = inner;
        }
    }
    /// <summary>
    /// 스프링 힘 계산
    /// </summary>
    /// <param name="currentLength"></param>
    /// <returns></returns>
    float ComputeSpringForce(float currentLength)
    {
        float x = restLength - currentLength;
        x = MathUtility.Max(x, 0f);

        float linear = springK * x;
        float progressive = springK2 * x * x;

        float force = linear + progressive;

        return MathUtility.Max(force, 0f);
    }
    /// <summary>
    /// 댐퍼 계산
    /// </summary>
    /// <param name="currentLength"></param>
    /// <param name="prevLength"></param>
    /// <returns></returns>
    float ComputeDamperForce(float currentLength, float prevLength)
    {
        float velocity = (currentLength - prevLength) / Time.fixedDeltaTime;

        float force;

        if (velocity < 0f)//중력 방향
        {
            // Compression (눌림)
            force = damperCompression * velocity;
        }
        else//중력 반대 방향
        {
            // Rebound (복원)
            force = damperRebound * velocity;
        }
        force *= (1f + MathUtility.Abs(velocity) * 0.1f);
        return force;
    }
    /// <summary>
    /// 엔진 토크 계산
    /// </summary>
    /// <param name="rpm"></param>
    /// <returns></returns>
    float GetEngineTorque(float rpm)
    {
        float normalizedRPM = rpm / maxRPM;

        float curveValue = torqueCurve.Evaluate(normalizedRPM);

        return curveValue * maxTorque;
    }
    /// <summary>
    /// 엔진 RPM 계산
    /// </summary>
    void UpdateEngineRPM()
    {
        float wheelRPM = (Ux / (2f * MathUtility.PI * R)) * 60f;

        float gearRatio = gearRatios[currentGear] * finalDrive;

        engineRPM = wheelRPM * gearRatio;

        engineRPM = MathUtility.ClampValue(engineRPM, 800f, maxRPM);
    }
    /// <summary>
    /// 타이어 힘 계산
    /// 하중(Fz)이 커지면 타이어 힘도 커진다
    /// </summary>
    void ComputeTireForce()
    {
        // 1. 하중 안정화
        FzFL = MathUtility.Max(FzFL, 0f);
        FzFR = MathUtility.Max(FzFR, 0f);
        FzRL = MathUtility.Max(FzRL, 0f);
        FzRR = MathUtility.Max(FzRR, 0f);

        // 2. 슬립각 제한 (중요)
        float alphaF_clamped = MathUtility.ClampValue(alphaF, -1.0f, 1.0f);
        float alphaR_clamped = MathUtility.ClampValue(alphaR, -1.0f, 1.0f);

        // 3. Pacejka 적용
        FyFL = PacejkaFy(alphaFL, FzFL)*MathUtility.Cos(camberFront);
        FyFR = PacejkaFy(alphaFR, FzFR) * MathUtility.Cos(camberFront);

        FyRL = PacejkaFy(alphaR_clamped, FzRL) * MathUtility.Cos(camberRear);
        FyRR = PacejkaFy(alphaR_clamped, FzRR) * MathUtility.Cos(camberRear);

        // 4. 축 합산 (기존 유지)
        Fy_f = FyFL + FyFR;
        Fy_r = FyRL + FyRR;
    }
    /// <summary>
    /// mu 계산 함수
    /// </summary>
    /// <param name="Fz"></param>
    /// <returns></returns>
    float GetMu(float Fz)
    {
        float mu = baseMu * MathUtility.Pow(Fz, -0.1f); ;
        return MathUtility.Max(mu, 0.5f); // 최소 grip 보장
    }
    float PacejkaFx(float slipRatio, float Fz)
    {
        float B = 12.0f;
        float C = 1.3f;
        float E = 0.97f;
        float localMu = GetMu(Fz);
        float effectiveFz = MathUtility.Pow(Fz, loadSensitivity);
        float D = localMu * effectiveFz;

        float Bx = B * slipRatio;
        float term = Bx - E * (Bx - MathUtility.Atan(Bx));

        float Fx = D * MathUtility.Sin(C * MathUtility.Atan(term));

        return Fx;
    }
    float PacejkaFy(float alpha, float Fz)
    {
        float B = 10.0f;
        float C = 1.3f;
        float E = 0.97f;
        float localMu = GetMu(Fz);
        float effectiveFz = MathUtility.Pow(Fz, loadSensitivity);
        float D = localMu * effectiveFz;

        float Bx = B * alpha;
        float term = Bx - E * (Bx - MathUtility.Atan(Bx));

        float Fy = D * MathUtility.Sin(C * MathUtility.Atan(term));

        return Fy;
    }

    /// <summary>
    /// Roll 업데이트
    /// </summary>
    void UpdateRollDynamics(float ay, float dt)
    {
        float M_roll = m * h * ay;

        rollAcc = (M_roll - Kphi * roll - Cphi * rollRate) / Iphi;

        //적분
        rollRate += rollAcc * dt;
        roll += rollRate * dt;
    }

    void UpdateWheelLoads()
    {
        /// 1. 월드 위치 : 로컬 좌표계 -> 월드 좌표계
        Vec3 posFL = transform3D.TransformPoint(new Vec3(-trackWidth * 0.5f, 0f, a),yaw);
        Vec3 posFR = transform3D.TransformPoint(new Vec3(trackWidth * 0.5f, 0f, a), yaw);
        Vec3 posRL = transform3D.TransformPoint(new Vec3(-trackWidth * 0.5f, 0f, -b), yaw);
        Vec3 posRR = transform3D.TransformPoint(new Vec3(trackWidth * 0.5f, 0f, -b), yaw);

        // 2. 서스펜션 길이 (🔥 핵심)
        susFL = GetSuspensionLength(posFL);
        susFR = GetSuspensionLength(posFR);
        susRL = GetSuspensionLength(posRL);
        susRR = GetSuspensionLength(posRR);

        // 2. 스프링 힘 = Fz
        float springFL = ComputeSpringForce(susFL);
        float springFR = ComputeSpringForce(susFR);
        float springRL = ComputeSpringForce(susRL);
        float springRR = ComputeSpringForce(susRR);

        // 3. 댐퍼
        float damperFL = ComputeDamperForce(susFL, prevSusFL);
        float damperFR = ComputeDamperForce(susFR, prevSusFR);
        float damperRL = ComputeDamperForce(susRL, prevSusRL);
        float damperRR = ComputeDamperForce(susRR, prevSusRR);

        // 🔥 핵심: 스프링 - 댐퍼
        FzFL = springFL - damperFL;
        FzFR = springFR - damperFR;
        FzRL = springRL - damperRL;
        FzRR = springRR - damperRR;

        // ===== Anti-Roll Bar =====
        float arbForceFront = arbStiffnessFront * (susFL - susFR);
        float arbForceRear = arbStiffnessRear * (susRL - susRR);

        FzFL -= arbForceFront;
        FzFR += arbForceFront;
        FzRL -= arbForceRear;
        FzRR += arbForceRear;

        FzFL = SmoothFz(prevFzFL, FzFL);
        FzFR = SmoothFz(prevFzFR, FzFR);
        FzRL = SmoothFz(prevFzRL, FzRL);
        FzRR = SmoothFz(prevFzRR, FzRR);

        FzFL = MathUtility.Max(FzFL, 0f);
        FzFR = MathUtility.Max(FzFR, 0f);
        FzRL = MathUtility.Max(FzRL, 0f);
        FzRR = MathUtility.Max(FzRR, 0f);

        prevFzFL = FzFL;
        prevFzFR = FzFR;
        prevFzRL = FzRL;
        prevFzRR = FzRR;

        // 6. 이전값 저장 (🔥 매우 중요)
        prevSusFL = susFL;
        prevSusFR = susFR;
        prevSusRL = susRL;
        prevSusRR = susRR;

        // 바퀴 다운포스
        float totalDownforce = ComputeDownforce(Ux);// F = kV^2
        float speedFactor = MathUtility.ClampValue(Ux / 50f, 0f, 1f);

        float dynamicBalance = MathUtility.Lerp(0.5f, aeroBalance, speedFactor);
        float frontDownforce = totalDownforce * dynamicBalance;
        float rearDownforce = totalDownforce * (1f - dynamicBalance);

        // Front
        float frontRatio = 0.45f; // 언더스티어/오버스티어 조절 핵심
        FzFL += frontDownforce * frontRatio;
        FzFR += frontDownforce * (1-frontRatio);

        // Rear
        FzRL += rearDownforce * 0.5f;
        FzRR += rearDownforce * 0.5f;
    }
    void UpdateSlip()
    {
        slipFL = ComputeSlipRatio(wFL, R, Ux);
        slipFR = ComputeSlipRatio(wFR, R, Ux);
        slipRL = ComputeSlipRatio(wRL, R, Ux);
        slipRR = ComputeSlipRatio(wRR, R, Ux);
    }
    /// <summary>
    /// 슬립 비율 계산
    /// </summary>
    /// <param name="wheelAngularVel"></param>
    /// <param name="radius"></param>
    /// <param name="Ux"></param>
    /// <returns></returns>
    float ComputeSlipRatio(float wheelAngularVel, float radius, float Ux)
    {
        float Vwheel = wheelAngularVel * radius;

        float denom = MathUtility.Max(MathUtility.Abs(Ux), 0.1f); // 0 나눗셈 방지

        return (Vwheel - Ux) / denom;
    }
    /// <summary>
    /// ray로 서스펜션 길이 계산
    /// </summary>
    /// <param name="wheelPos"></param>
    /// <returns></returns>
    float GetSuspensionLength(Vec3 wheelPos)
    {
        Ray3D ray; 
        ray.origin = wheelPos;
        ray.dir = VectorMathUtils.DownVector3D();
        RaycastHit3D hit;

        float maxLength = restLength + 0.2f;

        if (collider3D.RayCast(ray, maxLength, out hit))
        {
            float distance = (hit.position - ray.origin).Magnitude;
            return distance;
        }

        return maxLength; // 공중
    }
   
    /// <summary>
    /// ABS 적용
    /// 브레이크를 줄이는 게 아니라, 잠기려는 순간만 풀어준다
    /// </summary>
    void ApplyABS()
    {
        if (brakeInput < 0.01f) return;//브레이크 상태가 아님
        if (inputThrottle > 0.2f) return; // 가속 중엔 꺼라
        if (MathUtility.Abs(Ux) < 2f) return;//저속
       
        float threshold = -0.2f;
        float slipFront = MathUtility.Min(slipFR, slipFL);
        float slipRear = MathUtility.Min(slipRR, slipRL);
        float worstSlip = MathUtility.Min(slipFront, slipRear);

        if (worstSlip < threshold)
        {
            brakeForce *= 0.7f;
        }
    }
    /// <summary>
    /// TCS 적용
    /// 가속이 아닌 미끄러짐을 막음, 엔진 토크 제한
    /// </summary>
    void ApplyTCS()
    {
        if (inputBrake > 0.1f) return; // 브레이크 중엔 꺼라
        if (inputThrottle < 0.01f) return;//가속 상태가 아님
        if (MathUtility.Abs(Ux) < 2f) return;//저속

        bool isCornering = MathUtility.Abs(delta) > steerThreshold;

        // 🔥 ESC 우선 → 코너링 중이면 TCS 제한
        if (isCornering)
        {
            tcsFactor *= 0.3f; // 약하게만 개입
            return;
        }

        // 🔥 구동축 기준 (RWD 기준) - 뒷바퀴만 구동
        float driveSlip = MathUtility.Max(slipRL, slipRR);

        float optimalSlip = 0.1f;
        float maxSlip = 0.25f;
        //접지된 만큼만 개입
        float loadFactor = FzRL / (m * g * 0.25f);
        tcsFactor *= loadFactor;
        tcsFactor = MathUtility.InverseLerp(optimalSlip, maxSlip, driveSlip);
    }
    /// <summary>
    /// 타이어 힘 Fx + Fy 적용
    /// 4개 바퀴 모두 적용
    /// </summary>
    void ApplyTireCoupling()
    {
        ApplyCoupling(ref FxFL, ref FyFL, FzFL);
        ApplyCoupling(ref FxFR, ref FyFR, FzFR);
        ApplyCoupling(ref FxRL, ref FyRL, FzRL);
        ApplyCoupling(ref FxRR, ref FyRR, FzRR);
    }

    void ComputeLongitudinalTireForce()
    {
        // Pacejka 적용
        FxFL = PacejkaFx(slipFL, FzFL);
        FxFR = PacejkaFx(slipFR, FzFR);
        FxRL = PacejkaFx(slipRL, FzRL);
        FxRR = PacejkaFx(slipRR, FzRR);
    }

    /// <summary>
    /// 각 방향에 해당되는 타이어 힘 계산
    /// </summary>
    /// <param name="Fx"></param>
    /// <param name="Fy"></param>
    /// <param name="Fz"></param>
    void ApplyCoupling(ref float Fx,ref float Fy, float Fz)
    {
        float grip = mu * Fz;

        float forceMag = MathUtility.Root(Fx * Fx + Fy * Fy);

        if (forceMag > grip && forceMag > 0f)
        {
            float scale = grip / forceMag;

            Fx *= scale;
            Fy *= scale;
        }
    }
    /// <summary>
    /// ESC 적용
    /// </summary>
    void ApplyESC()
    {
        if (!escEnabled) return;
        if (MathUtility.Abs(Ux) < minSpeed) return;
        if (MathUtility.Abs(delta) < steerThreshold) return;
        if (FzFL < 100f) return; // 떠있는 바퀴는 무시

        float safeUx = MathUtility.Max(Ux, 0.1f);

        float alphaF = MathUtility.Atan2(Vy + a * r, safeUx) - delta;
        float alphaR = MathUtility.Atan2(Vy - b * r, safeUx);

        float slipDiff = alphaR - alphaF;
        float absSlip = MathUtility.Abs(slipDiff);

        float escFactor = Mathf.InverseLerp(softThreshold, hardThreshold, absSlip);
        if (escFactor <= 0f) return;

        // 🔥 브레이크 계산
        float escBrake = escGain * MathUtility.Abs(escFactor);
        escBrake = MathUtility.ClampValue(escBrake, 0f, maxEscBrake);
        // 🔥 속도 기반
        float speedFactor = MathUtility.ClampValue(Ux / 10f, 0f, 1f);
        escBrake *= speedFactor;

        // 🔥 스무딩
        currentEscBrake = MathUtility.Lerp(currentEscBrake, escBrake, 5f * Time.fixedDeltaTime);

        float finalBrake = MathUtility.Max(driverBrake, currentEscBrake);

        // 🔥 ABS 조정 (누적 방지)
        bool isESCBraking = currentEscBrake > 0.01f;
        absStrength = isESCBraking ? baseAbsStrength * 0.5f : baseAbsStrength;

        // 🔥 바퀴 적용
        if (slipDiff > 0f)
        {
            // 오버스티어 → 바깥 앞
            if (delta > 0f)
                FxFL -= finalBrake; // 좌측 (바깥)
            else
                FxFR -= finalBrake;
        }
        else if (slipDiff < 0f)
        {
            // 언더스티어 → 안쪽 뒤
            if (delta > 0f)
                FxRR -= finalBrake; // 우측 (안쪽)
            else
                FxRL -= finalBrake;
        }
    }
    /// <summary>
    /// Fx, Fy 조절
    /// </summary>
    void ApplyCombinedSlipAll()
    {
        ApplyCombinedSlip(ref FxFL, ref FyFL, FzFL);
        ApplyCombinedSlip(ref FxFR, ref FyFR, FzFR);
        ApplyCombinedSlip(ref FxRL, ref FyRL, FzRL);
        ApplyCombinedSlip(ref FxRR, ref FyRR, FzRR);
    }
    void ApplyCombinedSlip(ref float Fx, ref float Fy, float Fz)
    {
        if (Fz <= 0f) return;

        //최댓값
        float effectiveFz = MathUtility.Pow(Fz, loadSensitivity);
        float Fx_max = mu * effectiveFz;
        float Fy_max = mu * effectiveFz;

        //힘비율
        float nx = Fx / Fx_max;
        float ny = Fy / Fy_max;
        float usage = MathUtility.Root(nx * nx + ny * ny);

        //한계초과시 정규화
        if (usage > 1f)
        {
            Fx /= usage;
            Fy /= usage;
        }

        float grip = mu * Fz;

        float FxNorm = Fx / grip;
        float FyNorm = Fy / grip;
        FxNorm = MathUtility.ClampValue(FxNorm, -1f, 1f);
        FyNorm = MathUtility.ClampValue(FyNorm, -1f, 1f);

        float reduction = MathUtility.Root(1f - FxNorm * FxNorm);
        // friction ellipse 기반
        float Gx = MathUtility.Root(1f - FyNorm * FyNorm);
        float Gy = MathUtility.Root(1f - FxNorm * FxNorm);

        //종, 횡력에 모두 적용
        Fx *= Gx;
        Fy *= Gy;
    }
    /// <summary>
    /// Fz 스무딩
    /// </summary>
    /// <param name="prev"></param>
    /// <param name="current"></param>
    /// <returns></returns>
    float SmoothFz(float prev, float current)
    {
        return MathUtility.Lerp(prev, current, 10f * Time.fixedDeltaTime);
    }
    /// <summary>
    /// 차량 운동 적용
    /// </summary>
    void IntegrateVehicle()
    {
        float dt = Time.fixedDeltaTime;

        // 1. 전체 힘 합 (차량 기준 좌표)
        float Fx = FxFL + FxFR + FxRL + FxRR;
        float Fy = FyFL + FyFR + FyRL + FyRR;

        // 2. 가속도 계산 (차량 좌표계)
        float ax_body = Fx / m + r * Uy;
        float ay_body = Fy / m - r * Ux;

        // 3. 속도 업데이트 (body frame)
        Ux += ax_body * dt;
        Uy += ay_body * dt;

        // 4. Yaw moment 계산
        float Mz =
            a * (FyFL + FyFR)   // front lateral
          - b * (FyRL + FyRR);  // rear lateral

        // 5. yaw rate 업데이트
        float r_dot = Mz / Iz;
        r += r_dot * dt;

        // 6. 월드 좌표로 변환
        float cosYaw = MathUtility.Cos(yaw);
        float sinYaw = MathUtility.Sin(yaw);

        float Vx_world = cosYaw * Ux - sinYaw * Uy;
        float Vy_world = sinYaw * Ux + cosYaw * Uy;

        // 7. 위치 업데이트
        transform.position += new Vector3(Vx_world, 0f, Vy_world) * dt;

        // 8. 회전 업데이트
        yaw += r * dt;
        transform.rotation = Quaternion.Euler(0f, yaw * MathUtility.Rad2Deg, 0f);
    }
    #endregion

    #region 온도
    /// <summary>
    /// 각 타이어별 온도 업데이트
    /// </summary>
    void UpdateTireTemperature()
    {
        UpdateSingleTemp(ref tireTempFL, slipFL, FxFL, FyFL);
        UpdateSingleTemp(ref tireTempFR, slipFR, FxFR, FyFR);
        UpdateSingleTemp(ref tireTempRL, slipRL, FxRL, FyRL);
        UpdateSingleTemp(ref tireTempRR, slipRR, FxRR, FyRR);
    }
    /// <summary>
    /// 온도 업데이트
    /// </summary>
    /// <param name="temp"></param>
    /// <param name="slip"></param>
    /// <param name="Fx"></param>
    /// <param name="Fy"></param>
    void UpdateSingleTemp(ref float temp, float slip, float Fx, float Fy)
    {
        float heat = MathUtility.Abs(slip) * (MathUtility.Abs(Fx) + MathUtility.Abs(Fy)) * heatGain * compoundHeatGain;

        float cooling = (temp - ambientTemp) * coolingRate;

        temp += (heat - cooling) * Time.fixedDeltaTime;

        temp = MathUtility.ClampValue(temp, 0f, 200f);
    }
    float GetTempGrip(float temp)
    {
        float optimal = 90f;
        float range = 40f;

        float diff = temp - optimal;

        return MathUtility.Exp(-(diff * diff) / (2f * range * range));
    }
    void ApplyTemperatureGrip()
    {
        float gripFL = GetTempGrip(tireTempFL);
        float gripFR = GetTempGrip(tireTempFR);
        float gripRL = GetTempGrip(tireTempRL);
        float gripRR = GetTempGrip(tireTempRR);

        FxFL *= gripFL;
        FyFL *= gripFL;

        FxFR *= gripFR;
        FyFR *= gripFR;

        FxRL *= gripRL;
        FyRL *= gripRL;

        FxRR *= gripRR;
        FyRR *= gripRR;
    }
    #endregion

    #region 타이어 함수
    void UpdateTireWear()
    {
        UpdateSingleWear(ref tireWearFL, slipFL, tireTempFL);
        UpdateSingleWear(ref tireWearFR, slipFR, tireTempFR);
        UpdateSingleWear(ref tireWearRL, slipRL, tireTempRL);
        UpdateSingleWear(ref tireWearRR, slipRR, tireTempRR);
    }

    void UpdateSingleWear(ref float wear, float slip, float temp)
    {
        float slipWear = MathUtility.Abs(slip) * wearRate * compoundWearRate;

        float tempWear = MathUtility.Max(temp - 90f, 0f) * wearTempFactor;

        wear -= (slipWear + tempWear) * Time.fixedDeltaTime;

        wear = MathUtility.ClampValue(wear, 0.5f, 1.0f);
    }
    void ApplyWearGrip()
    {
        FxFL *= tireWearFL;
        FyFL *= tireWearFL;

        FxFR *= tireWearFR;
        FyFR *= tireWearFR;

        FxRL *= tireWearRL;
        FyRL *= tireWearRL;

        FxRR *= tireWearRR;
        FyRR *= tireWearRR;
    }
    #endregion

    #region 노면
    /// <summary>
    /// 노면별 값 세팅
    /// </summary>
    void UpdateSurfaceProperties()
    {
        switch (currentSurface)
        {
            case SurfaceType.Dry:
                surfaceGrip = 1.0f;
                surfaceSlipMultiplier = 1.0f;
                break;

            case SurfaceType.Wet:
                surfaceGrip = 0.7f;
                surfaceSlipMultiplier = 1.3f;
                break;

            case SurfaceType.Snow:
                surfaceGrip = 0.4f;
                surfaceSlipMultiplier = 2.0f;
                break;
        }
    }
    void ApplySurfaceEffect()
    {
        FxFL *= surfaceGrip;
        FyFL *= surfaceGrip;

        FxFR *= surfaceGrip;
        FyFR *= surfaceGrip;

        FxRL *= surfaceGrip;
        FyRL *= surfaceGrip;

        FxRR *= surfaceGrip;
        FyRR *= surfaceGrip;
    }
    #endregion

    #region 타이어 종류
    /// <summary>
    /// 컴파운드 설정
    /// </summary>
    void UpdateCompoundProperties()
    {
        switch (currentCompound)
        {
            case TireCompound.Soft:
                compoundGrip = 1.2f;
                compoundWearRate = 2.0f;
                compoundHeatGain = 1.5f;
                break;

            case TireCompound.Medium:
                compoundGrip = 1.0f;
                compoundWearRate = 1.0f;
                compoundHeatGain = 1.0f;
                break;

            case TireCompound.Hard:
                compoundGrip = 0.85f;
                compoundWearRate = 0.5f;
                compoundHeatGain = 0.7f;
                break;
        }
    }
    void ApplyCompoundGrip()
    {
        FxFL *= compoundGrip;
        FyFL *= compoundGrip;

        FxFR *= compoundGrip;
        FyFR *= compoundGrip;

        FxRL *= compoundGrip;
        FyRL *= compoundGrip;

        FxRR *= compoundGrip;
        FyRR *= compoundGrip;
    }
    #endregion

    #region 온도
    /// <summary>
    /// 온도 기록
    /// </summary>
    void RecordTelemetry()
    {
        telemetryTimer += Time.fixedDeltaTime;

        if (telemetryTimer < telemetryInterval)
            return;

        telemetryTimer = 0f;

        TelemetryFrame frame = new TelemetryFrame();

        frame.time = Time.time;

        // 차량
        frame.Ux = Ux;
        frame.Uy = Uy;
        frame.yawRate = r;
        frame.steer = delta;

        // 슬립
        frame.slipFL = slipFL;
        frame.slipFR = slipFR;
        frame.slipRL = slipRL;
        frame.slipRR = slipRR;

        // 온도
        frame.tempFL = tireTempFL;
        frame.tempFR = tireTempFR;
        frame.tempRL = tireTempRL;
        frame.tempRR = tireTempRR;

        // 하중 🔥 핵심
        frame.FzFL = FzFL;
        frame.FzFR = FzFR;
        frame.FzRL = FzRL;
        frame.FzRR = FzRR;

        // 저장
        if (telemetryLog.Count >= maxSamples)
            telemetryLog.RemoveAt(0);

        telemetryLog.Add(frame);
    }
    /// <summary>
    /// 자동 튜닝
    /// </summary>
    /// <param name="f"></param>
    public void AutoTune(TelemetryFrame f)
    {
        float rearSlip = MathUtility.Max(MathUtility.Abs(f.slipRL), MathUtility.Abs(f.slipRR));
        float frontSlip = MathUtility.Max(MathUtility.Abs(f.slipFL), MathUtility.Abs(f.slipFR));

        if (rearSlip > frontSlip + 0.1f)
        {
            // 오버스티어 → 뒤 안정화
            arbRear += 100f;
            lsdStrength -= 0.01f;
        }

        if (frontSlip > rearSlip + 0.1f)
        {
            // 언더스티어 → 앞 그립 증가
            arbFront -= 100f;
        }
    }
    #endregion

    #region 그래프 데이터 뽑기
    public List<float> GetSpeedData()
    {
        List<float> data = new List<float>();

        foreach (var f in telemetryLog)
            data.Add(f.Ux);

        return data;
    }

    public List<float> GetSlipRLData()
    {
        List<float> data = new List<float>();

        foreach (var f in telemetryLog)
            data.Add(f.slipRL);

        return data;
    }
    /// <summary>
    /// 기록 재생
    /// </summary>
    public void RecordReplay()
    {
        ReplayFrame f = new ReplayFrame();

        f.time = Time.time;

        f.position = transform.position;
        f.yaw = yaw;

        f.throttle = inputThrottle;
        f.brake = inputBrake;
        f.steer = delta;

        if (replayLog.Count >= maxSamples)
            replayLog.RemoveAt(0);

        replayLog.Add(f);
    }
    #endregion

    #region AI 추척 로직
    /// <summary>
    /// AI 입력
    /// </summary>
    void UpdateAIInput()
    {
        //경로가 없음
        if (path == null || path.Count == 0) return;

        // 1. waypoint 진행
        UpdateWaypoint();

        // 2. 방향 계산
        Vec3 localDir = GetLocalTargetDir();

        // 3. 조향
        float steer = CalculateSteer(localDir);
        delta = steer * 0.6f; // 최대 조향각 스케일

        // 🔥 4. Speed Control (핵심)
        CornerPhase phase = GetCornerPhase(currentIndex);

        ApplyCornerBehavior(phase);

        // 🔥 5. 안정화 (오늘 핵심)
        ApplySlipControl();
        ApplyYawControl();
        ApplyUnderOverSteerControl();

        //AI
        Vec3 target = GetTargetPositionDynamic();
        inputSteer = ComputeSteering(target);
        float targetSpeed = speedProfile[currentIndex];
        float speedError = targetSpeed - Ux;

        if (speedError > 0)
        {
            inputThrottle = MathUtility.ClampValue(speedError * 0.1f,0,1);
            inputBrake = 0f;
        }
        else
        {
            inputThrottle = 0f;
            inputBrake = MathUtility.ClampValue(-speedError * 0.1f,0,1);
        }

    }
    /// <summary>
    /// 경로 업데이트
    /// </summary>
    void UpdateWaypoint()
    {
        if (path == null || path.Count == 0) return;

        Vec3 currentTarget = new Vec3(path[currentIndex].position.x, 
            path[currentIndex].position.y, path[currentIndex].position.z);

        Vec3 pos = new Vec3(transform.position.x, transform.position.y, transform.position.z);
        //현재 목표지점과 위치간 거리
        float dist = VectorMathUtils.Dist(pos, currentTarget);

        if (dist < waypointThreshold)
        {
            currentIndex++;

            if (currentIndex >= path.Count)
                currentIndex = 0;
        }
    }
   
    /// <summary>
    /// 방향 계산
    /// </summary>
    /// <returns></returns>
    Vec3 GetLocalTargetDir()
    {
        Vec3 target = GetTargetFromRacingLine();//타겟 위치

        float worldDirX = target.x - transform.position.x;
        float worldDirY = target.x - transform.position.y;
        float worldDirZ = target.x - transform.position.z;
        Vec3 worldDir = new Vec3(worldDirX, worldDirY, worldDirZ);

        // 🔥 월드 → 로컬 변환
        float cos = MathUtility.Cos(yaw);
        float sin = MathUtility.Sin(yaw);

        float localX = cos * worldDir.x + sin * worldDir.z;
        float localZ = -sin * worldDir.x + cos * worldDir.z;

        return new Vec3(localX, 0f, localZ).Normalized;
    }
    /// <summary>
    /// Steer 각도(조향각) 계산
    /// </summary>
    /// <param name="localDir"></param>
    /// <returns></returns>
    float CalculateSteer(Vec3 localDir)
    {
        float angle = MathUtility.Atan2(localDir.x, localDir.z);
        // 1. 감도 조절 (속도 기반)
        float steerSensitivity = MathUtility.Lerp(1.5f, 0.5f, MathUtility.ClampValue(Ux / 30f,0,1));

        float targetSteer = angle * steerSensitivity;

        // 2. 스무딩 (핵심🔥)
        smoothSteer = MathUtility.Lerp(smoothSteer, targetSteer, 5f * Time.fixedDeltaTime);

        return MathUtility.ClampValue(smoothSteer, -1f, 1f);
    }
    /// <summary>
    /// 타겟 방향을 향하게
    /// </summary>
    /// <returns></returns>
    float GetDynamicLookAhead()
    {
        // 1. 속도 기반
        float speedFactor = MathUtility.Lerp(minLookAhead, maxLookAhead, Ux / maxSpeed);

        // 2. 곡률 기반
        float curvature = MathUtility.Max(GetCurvature(currentIndex), GetUpcomingCurvature());
        float curvatureFactor = MathUtility.ClampValue(1f - curvature * curvatureGain,0,1);

        // 3. 조향 기반
        float steerFactor = 1f - MathUtility.Abs(inputSteer) * steerLookReduce;

        // 최종
        float lookAhead = speedFactor * curvatureFactor * steerFactor;

        return MathUtility.ClampValue(lookAhead, minLookAhead, maxLookAhead);
    }
    /// <summary>
    /// 반지름 가져오기
    /// </summary>
    /// <param name="curvature"></param>
    /// <returns></returns>
    float GetRadius(float curvature)
    {
        return 1f / MathUtility.Max(curvature, 0.001f);
    }

    void BuildSpeedProfile()
    {
        GenerateSpeedProfile();   // 기본 속도
        ApplyBrakeLimit();        // 🔥 제일 중요
        ApplyAccelerationLimit(); // 현실화
    }
    /// <summary>
    /// 스피드 프로파일
    /// </summary>
    void GenerateSpeedProfile()
    {
        speedProfile.Clear();

        for (int i = 0; i < racingLine.Count; i++)
        {
            float curvature = GetCurvature(i);
            float radius = GetRadius(curvature);

            float v = MathUtility.Root(mu * 9.81f * radius);

            // 최대 속도 제한
            v = MathUtility.Min(v, maxSpeed);

            speedProfile.Add(v);
        }
    }
    /// <summary>
    /// 브레이크 제한
    /// </summary>
    void ApplyBrakeLimit()
    {
        for (int i = speedProfile.Count - 2; i >= 0; i--)
        {
            float dist = MathUtility.Abs(speedProfile[i]- speedProfile[i + 1]);

            float vNext = speedProfile[i + 1];

            float maxV = Mathf.Sqrt(
                vNext * vNext + 2f * maxBrake * dist
            );

            speedProfile[i] = Mathf.Min(speedProfile[i], maxV);
        }
    }
    /// <summary>
    /// 가속 제한
    /// </summary>
    void ApplyAccelerationLimit()
    {
        for (int i = 1; i < speedProfile.Count; i++)
        {
            float dist = MathUtility.Abs(speedProfile[i-1] - speedProfile[i]);

            float vPrev = speedProfile[i - 1];

            float maxV = Mathf.Sqrt(
                vPrev * vPrev + 2f * maxAccel * dist
            );

            speedProfile[i] = Mathf.Min(speedProfile[i], maxV);
        }
    }
    /// <summary>
    /// 앞을 봤을때의 곡률
    /// </summary>
    /// <returns></returns>
    float GetUpcomingCurvature()
    {
        int lookIndex = (currentIndex + 5) % racingLine.Count;
        return GetCurvature(lookIndex);
    }
    /// <summary>
    /// 타켓을 향한 속도 결정
    /// </summary>
    /// <returns></returns>
    float GetLookAheadSpeed()
    {
        int lookIndex = currentIndex + 5;

        if (lookIndex >= path.Count)
            lookIndex %= path.Count;

        return GetTargetSpeed(lookIndex);
    }
    /// <summary>
    /// Throttle, Brake 제어
    /// </summary>
    void ApplyCornerBehavior(CornerPhase phase)
    {
        switch (phase)
        {
            case CornerPhase.Entry:
                // 감속 + 바깥으로 벌림 준비
                inputThrottle = 0.2f;
                inputBrake = 0.3f;
                break;

            case CornerPhase.Apex:
                // 최대 회전
                inputThrottle = 0.3f;
                inputBrake = 0f;
                break;

            case CornerPhase.Exit:
                // 가속
                inputThrottle = 1f;
                inputBrake = 0f;
                break;

            default:
                // 직선
                inputThrottle = 1f;
                inputBrake = 0f;
                break;
        }
    }
    
    /// <summary>
    /// 곡률 계산
    /// a,b,c 점의 두 벡터 ab,bc를 구한 뒤 두 벡터사이의 각을 반환
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    float GetCurvature(int index)
    {
        int prev = (index - 1 + racingLine.Count) % racingLine.Count;
        int next = (index + 1) % racingLine.Count;

        Vec3 a = (racingLine[index].position - racingLine[prev].position).Normalized;
        Vec3 b = (racingLine[next].position - racingLine[index].position).Normalized;

        float angle = MathUtility.Acos(MathUtility.ClampValue(Vec3.Dot(a, b), -1f, 1f));

        return angle; // 클수록 코너
    }
    /// <summary>
    /// 곡률 -> 속도로 변환
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    float GetTargetSpeed(int index)
    {
        //곡률 구하기(각도)
        float curvature = GetCurvature(index);

        float maxSpeed = 35f;
        float minSpeed = 10f;

        return MathUtility.Lerp(maxSpeed, minSpeed, curvature);
    }
    /// <summary>
    /// 슬립 제어
    /// </summary>
    void ApplySlipControl()
    {
        float rearSlip = MathUtility.Max(MathUtility.Abs(slipRL), MathUtility.Abs(slipRR));

        float slipLimit = 0.2f;

        if (rearSlip > slipLimit)
        {
            float reduce = MathUtility.InverseLerp(slipLimit, 0.5f, rearSlip);

            inputThrottle *= (1f - reduce);
        }
    }
    /// <summary>
    /// Yaw 기반 Steering 제한
    /// </summary>
    void ApplyYawControl()
    {
        float yawLimit = 0.5f;

        float absYaw = MathUtility.Abs(r);

        if (absYaw > yawLimit)
        {
            float reduce = MathUtility.InverseLerp(yawLimit, 1.5f, absYaw);

            delta *= (1f - reduce);
        }
    }
    /// <summary>
    /// 언더 스티어, 오버 스티어 보정
    /// </summary>
    void ApplyUnderOverSteerControl()
    {
        float frontSlip = MathUtility.Max(MathUtility.Abs(slipFL), MathUtility.Abs(slipFR));
        float rearSlip = MathUtility.Max(MathUtility.Abs(slipRL), MathUtility.Abs(slipRR));

        float diff = rearSlip - frontSlip;

        // 오버스티어
        if (diff > 0.05f)
        {
            inputThrottle *= 0.7f;
            delta *= 0.8f;
        }
        // 언더스티어
        else if (diff < -0.05f)
        {
            delta *= 1.1f;
        }
    }
    #endregion
    #region 랩 타이머
    /// <summary>
    /// 라인 통과 감지
    /// </summary>
    void UpdateLapTimer()
    {
        Vec3 currentPos = new Vec3(transform.position.x, transform.position.y, transform.position.z );

        Vec3 lineDir = new Vec3(startLine.forward.x, startLine.forward.y, startLine.forward.z);
        Vec3 startPos = new Vec3(startLine.position.x, startLine.position.y, startLine.position.z);
        Vec3 toCar = currentPos - startPos;

        float dotNow = Vec3.Dot(lineDir, toCar);
        float dotPrev = Vec3.Dot(lineDir, lastPosition - startPos);

        // 앞 → 뒤 통과
        if (dotPrev > 0f && dotNow <= 0f)
        {
            OnLapComplete();
        }

        lastPosition = currentPos;
    }
    /// <summary>
    /// 랩 완료 처리
    /// </summary>
    void OnLapComplete()
    {
        if (lapCount == 0)
        {
            lapStartTime = Time.time;
            lapCount++;
            return;
        }

        lastLapTime = Time.time - lapStartTime;
        lapStartTime = Time.time;

        if (lastLapTime < bestLapTime)
            bestLapTime = lastLapTime;

        lapCount++;

        Debug.Log($"Lap {lapCount} : {lastLapTime:F2}s (Best: {bestLapTime:F2}s)");
    }
    #endregion

    #region 랩 결과 저장
    /// <summary>
    /// 자동 테스트 루프
    /// </summary>
    /// <param name="setups"></param>
    /// <returns></returns>
    IEnumerator RunAutoTest(List<CarSetup> setups)
    {
        foreach (var setup in setups)
        {
            ApplySetup(setup);

            ResetCar();

            yield return new WaitForSeconds(10f); // 안정화

            yield return RunLaps(3);

            SaveResult(setup.name);
        }

        PrintResults();
    }
    /// <summary>
    /// 랩 실행
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    IEnumerator RunLaps(int count)
    {
        int startLap = lapCount;

        while (lapCount < startLap + count)
        {
            yield return null;
        }
    }
    /// <summary>
    /// 결과 출력
    /// </summary>
    void PrintResults()
    {
        Debug.Log("===== TEST RESULTS =====");

        foreach (var r in results)
        {
            Debug.Log($"{r.setupName} : {r.bestLap:F2}s");
        }
    }
    /// <summary>
    /// 차량 리셋
    /// </summary>
    void ResetCar()
    {
        transform.position = startLine.position + Vector3.forward * 5f;
        yaw = startLine.eulerAngles.y * MathUtility.Deg2Rad;

        Ux = Uy = r = 0f;

        lapStartTime = Time.time;
        lapCount = 0;
    }
    /// <summary>
    /// 랩 결과 기록
    /// </summary>
    /// <param name="name"></param>
    void SaveResult(string name)
    {
        TestResult r = new TestResult();
        r.setupName = name;
        r.bestLap = bestLapTime;

        results.Add(r);

        Debug.Log($"Saved: {name} - {bestLapTime:F2}s");
    }
    #endregion

    #region 코너
    /// <summary>
    /// 코너 추출
    /// </summary>
    void DetectCorners()
    {
        corners.Clear();

        bool inCorner = false;
        int start = 0;

        for (int i = 0; i < path.Count; i++)
        {
            if (IsCorner(i))
            {
                if (!inCorner)//코너 안
                {
                    inCorner = true;
                    start = i;
                }
            }
            else
            {
                if (inCorner)//코너 밖
                {
                    int end = i;
                    CreateCorner(start, end);
                    inCorner = false;
                }
            }
        }
    }
    /// <summary>
    /// 코너 생성
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    void CreateCorner(int start, int end)
    {
        int length = (end - start + path.Count) % path.Count;

        int apex = FindApex(start, length);
        int lateApex = GetLateApex(apex, length);

        Corner c = new Corner();
        c.start = start;
        c.end = end;
        c.apex = apex;
        c.lateApex = lateApex;

        corners.Add(c);
    }
    /// <summary>
    /// Entry ,Exit 분리
    /// </summary>
    /// <param name="c"></param>
    void SplitCorner(ref Corner c)
    {
        int length = (c.end - c.start + path.Count) % path.Count;

        // Entry = 앞 40%
        int entryLength = (int)(length * 0.4f);

        // Exit = 뒤 40%
        int exitLength = (int)(length * 0.4f);

        c.entryStart = c.start;
        c.exitEnd = c.end;

        // Apex는 이미 있음
    }
    /// <summary>
    /// 현재 위치가 어떤 코너인지 파악
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    CornerPhase GetCornerPhase(int index)
    {
        foreach (var c in corners)
        {
            if (IsInside(index, c.start, c.end))
            {
                if (IsNear(index, c.lateApex, 2))
                    return CornerPhase.Apex;

                if (IsInside(index, c.start, c.lateApex))
                    return CornerPhase.Entry;

                return CornerPhase.Exit;
            }
        }
        return CornerPhase.None;
    }
    /// <summary>
    /// Inside 여부
    /// </summary>
    /// <param name="idx"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    bool IsInside(int idx, int start, int end)
    {
        if (start <= end)
            return idx >= start && idx <= end;

        return idx >= start || idx <= end;
    }
    /// <summary>
    /// Near 여부
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    bool IsNear(int a, int b, int range)
    {
        return MathUtility.Abs(a - b) <= range;
    }
    /// <summary>
    /// 레이싱 라인 초기화
    /// </summary>
    void GenerateRacingLine()
    {
        racingLine.Clear();

        // 기본은 중앙선 복사
        foreach (var wp in path)
        {
            Waypoint w = new Waypoint();
            w.position = wp.position;
            racingLine.Add(w);
        }
        foreach (var c in corners)
        {
            ApplyRacingLineToCorner(c);
        }
    }
    float GetApexWeight(int idx, Corner c)
    {
        int dist = (int)MathUtility.Abs(idx - c.lateApex);

        float maxDist = 10f; // 튜닝

        return MathUtility.ClampValue(dist / maxDist,0,1);
    }
    /// <summary>
    /// 경로의 법선벡터 구하기
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    Vec3 GetNormal(int index)
    {
        int next = (index + 1) % path.Count;

        Vec3 dir = (path[next].position - path[index].position).Normalized;

        // 오른쪽 법선
        return new Vec3(-dir.z, 0f, dir.x);
    }
    /// <summary>
    /// 실제 라인 변형
    /// </summary>
    /// <param name="c"></param>
    void ApplyRacingLineToCorner(Corner c)
    {
        int length = (c.end - c.start + path.Count) % path.Count;

        for (int i = 0; i < length; i++)
        {
            int idx = (c.start + i) % path.Count;

            float t = (float)i / length;

            float offset = 0f;

            if (t < 0.5f)
            {
                // Entry → Apex
                offset = MathUtility.Lerp(2.0f, -1.0f, t * 2f);
            }
            else
            {
                // Apex → Exit
                offset = MathUtility.Lerp(-1.0f, 2.0f, (t - 0.5f) * 2f);
            }

            Vec3 normal = GetNormal(idx);
            //중앙선 → 레이싱 라인
            racingLine[idx].position += normal * offset;
        }
    }
    /// <summary>
    /// 레이싱라인의 타겟 가져오기
    /// </summary>
    /// <returns></returns>
    Vec3 GetTargetFromRacingLine()
    {
        int dynamicLook = MathUtility.RoundToInt(GetDynamicLookAhead());

        int targetIndex = (currentIndex + dynamicLook) % racingLine.Count;

        return new Vec3(
            racingLine[targetIndex].position.x,
            racingLine[targetIndex].position.y,
            racingLine[targetIndex].position.z
        );
    }
    /// <summary>
    /// 코너 감지
    /// 두 선분이 이루는 각도와 임계각 비교
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    bool IsCorner(int index)
    {
        int prev = (index - 2 + path.Count) % path.Count;
        int next = (index + 2) % path.Count;

        Vec3 p0 = path[prev].position;
        Vec3 p1 = path[index].position;
        Vec3 p2 = path[next].position;

        Vec3 d1 = (p1 - p0).Normalized;
        Vec3 d2 = (p2 - p1).Normalized;

        float angle = VectorMathUtils.Angle(d1, d2);

        float threshold = 10f; // 🔥 튜닝 포인트 (8~15 추천)

        return angle > threshold;
    }
    /// <summary>
    /// 가장 꺽인 지점 찾기
    /// 두 연속된 선분들 비교하면서 이루는 각도가 가장 큰것
    /// </summary>
    /// <param name="startIndex"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    int FindApex(int startIndex, int length)
    {
        float maxAngle = -1f;
        int apexIndex = startIndex;

        for (int i = 0; i < length; i++)
        {
            int idx = (startIndex + i) % path.Count;

            int prev = (idx - 1 + path.Count) % path.Count;
            int next = (idx + 1) % path.Count;

            Vec3 p0 = path[prev].position;
            Vec3 p1 = path[idx].position;
            Vec3 p2 = path[next].position;

            Vec3 d1 = (p1 - p0).Normalized;
            Vec3 d2 = (p2 - p1).Normalized;

            float angle = VectorMathUtils.Angle(d1, d2);

            if (angle > maxAngle)
            {
                maxAngle = angle;
                apexIndex = idx;
            }
        }

        return apexIndex;
    }
    /// <summary>
    /// LateApex 값 계산, 레이싱용 가장 크게 꺽인 코너
    /// </summary>
    /// <param name="apexIndex"></param>
    /// <param name="cornerLength"></param>
    /// <returns></returns>
    int GetLateApex(int apexIndex, int cornerLength)
    {
        // 코너가 길수록 더 늦게
        float factor = MathUtility.ClampValue(cornerLength / 20f,0,1);

        int offset = MathUtility.RoundToInt(MathUtility.Lerp(2f, 6f, factor));

        return (apexIndex + offset) % path.Count;
    }
    /// <summary>
    /// 곡률 -> 반지름
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    float GetRadius(int index)
    {
        int prev = (index - 1 + racingLine.Count) % racingLine.Count;
        int next = (index + 1) % racingLine.Count;

        Vec3 p0 = racingLine[prev].position;
        Vec3 p1 = racingLine[index].position;
        Vec3 p2 = racingLine[next].position;

        Vec3 a = p1 - p0;
        Vec3 b = p2 - p1;

        float angle = VectorMathUtils.Angle(a, b) * MathUtility.Deg2Rad;

        float length = a.Magnitude;

        if (angle < 0.01f) return 999f; // 직선

        return length / angle;
    }
    /// <summary>
    /// 물리기반 목표 속도
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    float GetTargetSpeedAdvanced(int index)
    {
        float radius = GetRadius(index);

        float mu = 1.2f; // 현재 타이어 grip
        float g = 9.81f;

        float maxSpeed = MathUtility.Root(mu * g * radius);

        return MathUtility.ClampValue(maxSpeed, 5f, 50f);
    }
    /// <summary>
    /// Look Ahead기반 감속
    /// </summary>
    /// <returns></returns>
    float GetLookAheadSpeedAdvanced()
    {
        int lookIndex = currentIndex + 10;

        if (lookIndex >= racingLine.Count)
            lookIndex %= racingLine.Count;

        return GetTargetSpeedAdvanced(lookIndex);
    }
    /// <summary>
    /// 속도 제어
    /// </summary>
    void ApplySpeedControlAdvanced()
    {
        float targetSpeed = GetLookAheadSpeedAdvanced();

        float error = targetSpeed - Ux;

        // 🔥 브레이크는 더 공격적으로
        if (error < -1f)
        {
            inputThrottle = 0f;
            inputBrake = MathUtility.ClampValue(-error / 10f, 0f, 1f);
        }
        else if (error > 1f)
        {
            inputThrottle = 1f;
            inputBrake = 0f;
        }
        else
        {
            inputThrottle = 0.4f;
            inputBrake = 0f;
        }
    }
    /// <summary>
    /// 감속 예측
    /// </summary>
    /// <param name="currentSpeed"></param>
    /// <param name="targetSpeed"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    float PredictRequiredDecel(float currentSpeed, float targetSpeed, float distance)
    {
        if (distance <= 0.1f) return 0f;

        return (currentSpeed * currentSpeed - targetSpeed * targetSpeed) / (2f * distance);
    }
    /// <summary>
    /// 브레이크 적용
    /// </summary>
    void ApplySmartBraking()
    {
        int lookIndex = currentIndex + 10;

        if (lookIndex >= racingLine.Count)
            lookIndex %= racingLine.Count;

        float targetSpeed = GetTargetSpeedAdvanced(lookIndex);

        float distance = 10f; // lookAhead 거리 기반으로 바꿔도 됨

        float requiredDecel = PredictRequiredDecel(Ux, targetSpeed, distance);

        if (requiredDecel > 1f)
        {
            if (ShouldBrake())
            {
                inputBrake = ComputeBrakeInput();
                inputThrottle = 0f;
            }
            else
            {
                inputThrottle = 1f;
                inputBrake = 0;
            }
        }
        inputBrake = ApplyTrailBraking(inputBrake);
    }
    #endregion

    #region AI 개선
    /// <summary>
    /// 슬립기반 문제 탐지
    /// </summary>
    /// <param name="f"></param>
    /// <returns></returns>
    bool IsBadCorner(TelemetryFrame f)
    {
        float rearSlip = Mathf.Max(Mathf.Abs(f.slipRL), Mathf.Abs(f.slipRR));
        float frontSlip = Mathf.Max(Mathf.Abs(f.slipFL), Mathf.Abs(f.slipFR));

        // 오버스티어 or 언더스티어
        if (rearSlip > frontSlip + 0.1f) return true;
        if (frontSlip > rearSlip + 0.1f) return true;

        return false;
    }
    /// <summary>
    /// Brake 입력값 계산
    /// </summary>
    float ComputeBrakeInput()
    {
        float dist = 0f;
        int idx = currentIndex;

        float currentSpeed = Ux;

        while (dist < 100f)
        {
            int next = (idx + 1) % speedProfile.Count;

            float segmentDist = MathUtility.Abs(speedProfile[idx] - speedProfile[next]);
            dist += segmentDist;

            float targetSpeed = speedProfile[next];

            float brakeDist = GetBrakeDistance(currentSpeed, targetSpeed);

            if (brakeDist >= dist)
            {
                float ratio = brakeDist / MathUtility.Max(dist, 0.1f);

                return MathUtility.ClampValue(ratio,0,1);
            }

            idx = next;
        }
        return 0f;
    }
    /// <summary>
    /// 조향, 브레이크 연동
    /// </summary>
    /// <param name="brakeInput"></param>
    /// <returns></returns>
    float ApplyTrailBraking(float brakeInput)
    {
        float steerFactor = 1f - MathUtility.Abs(inputSteer);

        return brakeInput * steerFactor;
    }
    /// <summary>
    /// 프레임 분석
    /// </summary>
    /// <param name="f"></param>
    /// <returns></returns>
    CornerIssue AnalyzeFrame(TelemetryFrame f)
    {
        float rearSlip = MathUtility.Max(MathUtility.Abs(f.slipRL), MathUtility.Abs(f.slipRR));
        float frontSlip = MathUtility.Max(MathUtility.Abs(f.slipFL), MathUtility.Abs(f.slipFR));

        if (rearSlip > frontSlip + 0.1f)
            return CornerIssue.Oversteer;

        if (frontSlip > rearSlip + 0.1f)
            return CornerIssue.Understeer;

        if (f.Ux > 30f && frontSlip > 0.2f)
            return CornerIssue.TooFastEntry;

        if (f.Ux < 15f)
            return CornerIssue.TooSlowExit;

        return CornerIssue.None;
    }
    /// <summary>
    /// 라인 조정
    /// </summary>
    /// <param name="index"></param>
    /// <param name="issue"></param>
    void AdjustRacingLine(int index, CornerIssue issue)
    {
        Vec3 normal = GetNormal(index);

        float adjust = 0f;

        switch (issue)
        {
            case CornerIssue.Understeer:
                // 더 바깥 → 회전 여유 확보
                adjust = 1.5f;
                break;

            case CornerIssue.Oversteer:
                // 덜 aggressive
                adjust = -1.0f;
                break;

            case CornerIssue.TooFastEntry:
                // 더 바깥 → 감속 여유
                adjust = 2.0f;
                break;

            case CornerIssue.TooSlowExit:
                // 더 안쪽 → 빠른 exit
                adjust = -2.0f;
                break;
        }
        float maxOffset = trackWidth * 0.5f;
        Vec3 offset = racingLine[index].position - path[index].position;
        racingLine[index].position = path[index].position + offset;
    }
    /// <summary>
    /// 속도 수정
    /// </summary>
    /// <param name="index"></param>
    /// <param name="issue"></param>
    void AdjustSpeed(int index, CornerIssue issue)
    {
        float speedLearnRate = 0.02f;

        if (issue == CornerIssue.TooFastEntry)
        {
            speedProfile[index] *= (1f - speedLearnRate);
        }

        if (issue == CornerIssue.TooSlowExit)
        {
            speedProfile[index] *= (1f + speedLearnRate);
        }
    }
    /// <summary>
    /// 전체 학습 루프
    /// </summary>
    void OptimizeLap()
    {
        Dictionary<int, List<CornerIssue>> cornerIssues = new();

        foreach (var f in telemetryLog)
        {
            int index = GetClosestWaypointIndex(f);

            var issue = AnalyzeFrame(f);

            if (issue == CornerIssue.None) continue;

            if (!cornerIssues.ContainsKey(index))
                cornerIssues[index] = new List<CornerIssue>();

            cornerIssues[index].Add(issue);
        }

        foreach (var kv in cornerIssues)
        {
            int index = kv.Key;

            CornerIssue finalIssue = GetDominantIssue(kv.Value);

            AdjustRacingLine(index, finalIssue);
            AdjustSpeed(index, finalIssue);
        }
    }
    CornerIssue GetDominantIssue(List<CornerIssue> issues)
    {
        return issues
            .GroupBy(x => x)
            .OrderByDescending(g => g.Count())
            .First().Key;
    }
    /// <summary>
    /// 타겟 위치 가져오기
    /// </summary>
    /// <returns></returns>
    Vec3 GetTargetPositionDynamic()
    {
        float lookAheadDist = GetDynamicLookAhead();

        float dist = 0f;
        int idx = currentIndex;

        while (dist < lookAheadDist)
        {
            int next = (idx + 1) % racingLine.Count;

            dist += (racingLine[next].position - racingLine[idx].position).Magnitude;

            idx = next;
        }
        return racingLine[idx].position;
    }
    /// <summary>
    /// 현재 위치 인덱스 업데이트
    /// </summary>
    void UpdateCurrentIndex()
    {

    }
    /// <summary>
    /// Steering 계산
    /// </summary>
    /// <returns></returns>
    float ComputeSteering(Vec3 target)
    {
        Vec3 toTarget = target - new Vec3(transform.position.x, transform.position.y, transform.position.z);

        float localX = Vec3.Dot(VectorMathUtils.LeftVector3D(), toTarget);
        float localZ = Vec3.Dot(VectorMathUtils.FrontVector3D(), toTarget);

        float Ld = toTarget.Magnitude;

        if (Ld < 0.001f) return 0f;

        float curvature = (2f * localX) / (Ld * Ld);

        float steer = MathUtility.Atan(curvature * wheelbase);

        return MathUtility.ClampValue(steer, -maxSteer, maxSteer);
    }
    /// <summary>
    /// 브레이크 제동 거리
    /// </summary>
    /// <param name="currentSpeed"></param>
    /// <param name="targetSpeed"></param>
    /// <returns></returns>
    float GetBrakeDistance(float currentSpeed, float targetSpeed)
    {
        if (currentSpeed <= targetSpeed)
            return 0f;

        float a = maxBrake; // 양수로 사용

        return (currentSpeed * currentSpeed - targetSpeed * targetSpeed)
               / (2f * a);
    }
    /// <summary>
    /// 브레이크 결정
    /// </summary>
    /// <returns></returns>
    bool ShouldBrake()
    {
        float dist = 0f;
        int idx = currentIndex;

        float currentSpeed = Ux;

        while (dist < 100f) // 최대 탐색 거리
        {
            int next = (idx + 1) % speedProfile.Count;

            float segmentDist = MathUtility.Abs(speedProfile[idx]- speedProfile[next]);
            dist += segmentDist;

            float targetSpeed = speedProfile[next];

            float brakeDist = GetBrakeDistance(currentSpeed, targetSpeed);

            if (brakeDist >= dist)
                return true;

            idx = next;
        }

        return false;
    }
    #endregion
}
