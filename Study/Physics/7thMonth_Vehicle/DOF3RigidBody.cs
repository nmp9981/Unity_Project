using UnityEngine;

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

    //Transform
    Transform3D transform3D = new Transform3D();
    CustomCollider3D collider3D;

    //입력
    float inputThrottle;
    float inputBrake;

    [Header("차량 데이터")]
    public CarSetup currentSetup;

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
    }

    private void FixedUpdate()
    {
        // 0. 입력
        GetInput();

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
    }
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

    /// <summary>
    /// 입력값 받기
    /// </summary>
    void GetInput()
    {

    }
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
        FxDrag = dragCoeff * Ux * MathUtility.Abs(Ux);

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
        float D = localMu * Fz;

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
        float D = localMu * Fz;

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
        float totalDownforce = downforceCoeff * Ux * Ux;// F = kV^2
        float speedFactor = MathUtility.ClampValue(Ux / 50f, 0f, 1f);

        float dynamicBalance = MathUtility.Lerp(0.5f, aeroBalance, speedFactor);
        float frontDownforce = totalDownforce * dynamicBalance;
        float rearDownforce = totalDownforce * (1f - dynamicBalance);

        // Front
        FzFL += frontDownforce * 0.5f;
        FzFR += frontDownforce * 0.5f;

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
}
