using UnityEngine;

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

    float Iz = 2500;//yaw 관성
    float g = 9.81f;
    Vector3 local;

    float escThreshold = 0.2f;  // rad/s
    float steerThreshold = 0.02f;   // rad
    float minSpeed = 5f;        // m/s

    CustomRigidBody rb;

    // ===== 상태 =====
    float Ux;//종방향 속도
    float Uy;//횡방향 속도
    float Vy;
    float ay;
    float ax;
    float prevVy;

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

    // Roll 준비
    float trackWidth = 1.6f;   // m
    float Ixx = 600f;          // roll inertia
    float KphiFront = 18000f;
    float KphiRear = 12000f;// roll stiffness
    float Cphi = 50000f;        // roll damping
    float Kphi => KphiFront+KphiRear;
    float Iphi = 450f;

    //입력
    float inputThrottle;
    float inputBrake;

    private void Start()
    {
        InitForce();
    }

    private void FixedUpdate()
    {
        // 0. 입력
        GetInput();

        // 1. Longitudinal force (새로 추가된 것), 종방향
        ComputeLongitudinal();   // ax 계산됨

        // 2. Slip angle, 횡방향 준비
        ComputeSlipAngle();

        // 3. ay 계산
        ay = (Fy_f + Fy_r) / m;

        // 4. Roll + Pitch dynamics, 차체 거동
        UpdateRollDynamics(ay, Time.fixedDeltaTime);//roll 계산
        ComputeRollTransfer();        // 좌우 차이
        UpdatePitchWeightTransfer( Time.fixedDeltaTime);// ax기반

        // 5. Weight transfer → Fz 완성, 하중 통합
        UpdateWheelLoads();

        // 6. Tire force (이제는 새로운 Fz 기반), 타이어 힘
        ComputeTireForce();

        // 7. 슬립 결합
        ApplyCombinedSlipAll();//Fx, Fy조절
        
        // 8. 마찰 제한
        ApplyTireCoupling();

        // 9. 차량 운동 적용
        IntegrateVehicle();
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
    /// 종방향 힘 계산 + 속도 업데이트, ux,ax 계산
    /// </summary>
    void ComputeLongitudinal()
    {
        // 1. 입력
        float throttle = inputThrottle; // 0 ~ 1
        float brake = inputBrake;       // 0 ~ 1

        // 2. Drive Force (엔진)
        FxDrive = throttle * maxDriveForce * (1f - Ux / maxSpeed);
        FxDrive = MathUtility.Max(FxDrive, 0f);

        // 3. Brake Force
        brakeForce= brake * maxBrakeForce;

        // 속도 방향 반대로 작용
        if (MathUtility.Abs(Ux) > 0.1f)
            FxBrake *= MathUtility.Sign(Ux);
        else
            FxBrake = 0f;

        // 4. Drag (공기저항)
        FxDrag = dragCoeff * Ux * MathUtility.Abs(Ux);

        // 5. Rolling Resistance
        FxRoll = rollingCoeff * Ux;

        // 6. 총 힘
        FxTotal = FxDrive - FxBrake - FxDrag - FxRoll;

        // 7. 가속도
        ax = FxTotal / m;

        // 8. 속도 업데이트
        Ux += ax * Time.deltaTime;

        // 9. 정지 안정화 (중요)
        if (MathUtility.Abs(Ux) < 0.05f && throttle < 0.01f)
            Ux = 0f;
    }

    /// <summary>
    /// 슬립각 계산
    /// </summary>
    void ComputeSlipAngle()
    {
        // 2. 슬립각 계산
        float U = MathUtility.Abs(Ux);
        if (U < 0.5f)//저속에서는 타이어 모델을 끈다
        {
            alphaF = 0;
            alphaR = 0;
        }
        else
        {
            alphaF = delta - (Vy + a * r) / U;
            alphaR = (b * r - Vy) / U;
        }
    }
    /// <summary>
    /// 타이어 힘 계산
    /// 하중(Fz)이 커지면 타이어 힘도 커진다
    /// </summary>
    void ComputeTireForce()
    {
        FzFL = MathUtility.Max(FzFL, 0f);
        FzFR = MathUtility.Max(FzFR, 0f);
        FzRL = MathUtility.Max(FzRL, 0f);
        FzRR = MathUtility.Max(FzRR, 0f);

        float Fy_FL = mu * FzFL * MathUtility.Tanh(Cf * alphaF / (mu * staticFront));
        float Fy_FR = mu * FzFR * MathUtility.Tanh(Cf * alphaF / (mu * staticFront));

        float Fy_RL = mu * FzRL * MathUtility.Tanh(Cr * alphaR / (mu * staticRear));
        float Fy_RR = mu * FzRR * MathUtility.Tanh(Cr * alphaR / (mu * staticRear));

        Fy_f = Fy_FL + Fy_FR;
        Fy_r = Fy_RL + Fy_RR;
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

    /// <summary>
    /// Roll 업데이트
    /// </summary>
    void UpdatePitchWeightTransfer(float dt)
    {
        float dF = (m * h * ax) / wheelbase;

        // 가속 시: 앞 감소, 뒤 증가
        FzFront = staticFront - dF;
        FzRear = staticRear + dF;

        // 안전 처리 (타이어 뜨는거 방지)
        FzFront = MathUtility.Max(FzFront, 0f);
        FzRear = MathUtility.Max(FzRear, 0f);
    }

    void ComputeRollTransfer()
    {
        float dF = (m * h * ax) / wheelbase;

        FzFront = staticFront - dF;
        FzRear = staticRear + dF;
    }
    void UpdateWheelLoads()
    {
        // Pitch 적용된 축 하중
        float FzF = FzFront;
        float FzR = FzRear;

        // Roll 적용 (좌우 분배)
        FzFL = FzF * 0.5f - rollTransferFront;
        FzFR = FzF * 0.5f + rollTransferFront;

        FzRL = FzR * 0.5f - rollTransferRear;
        FzRR = FzR * 0.5f + rollTransferRear;

        // 안전 처리
        FzFL = MathUtility.Max(FzFL, 0f);
        FzFR = MathUtility.Max(FzFR, 0f);
        FzRL = MathUtility.Max(FzRL, 0f);
        FzRR = MathUtility.Max(FzRR, 0f);
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
    /// <summary>
    /// 각 방향에 해당되는 타이어 힘 계산
    /// </summary>
    /// <param name="Fx"></param>
    /// <param name="Fy"></param>
    /// <param name="Fz"></param>
    void ApplyCoupling(ref float Fx,ref float Fy, float Fz)
    {
        float grip = mu * Fz;

        float forceMag = Mathf.Sqrt(Fx * Fx + Fy * Fy);

        if (forceMag > grip && forceMag > 0f)
        {
            float scale = grip / forceMag;

            Fx *= scale;
            Fy *= scale;
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
        FxNorm = MathUtility.ClampValue(FxNorm, -1f, 1f);

        float reduction = MathUtility.Root(1f - FxNorm * FxNorm);

        Fy *= reduction;//횡력에 적용
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
        float cosYaw = Mathf.Cos(yaw);
        float sinYaw = Mathf.Sin(yaw);

        float Vx_world = cosYaw * Ux - sinYaw * Uy;
        float Vy_world = sinYaw * Ux + cosYaw * Uy;

        // 7. 위치 업데이트
        transform.position += new Vector3(Vx_world, 0f, Vy_world) * dt;

        // 8. 회전 업데이트
        yaw += r * dt;
        transform.rotation = Quaternion.Euler(0f, yaw * Mathf.Rad2Deg, 0f);
    }
}
