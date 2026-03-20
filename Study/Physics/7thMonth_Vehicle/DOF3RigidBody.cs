using UnityEngine;

public class DOF3RigidBody : MonoBehaviour
{
    float delta = 0.05f;//고정 조향각
    float currentUx, prevUx;
    float a = 1.2f;//55%
    float b = 1.6f;//45%
    float h = 0.55f;//세단 높이, CG Height
    float L = 2.8f;
    float m = 1500;
    float FzFront, FzRear, staticFront, staticRear;
    float mu = 1.0f;//마른 아스팔트 기준
    float Cf = 40000;
    float Cr = 30000;
    float vy_dot, r_dot;

    float Iz = 2500;
    float g = 9.81f;
    Vector3 local;

    float escThreshold = 0.2f;  // rad/s
    float steerThreshold = 0.02f;   // rad
    float minSpeed = 5f;        // m/s

    CustomRigidBody rb;

    // ===== 상태 =====
    float Ux;
    float Vy;
    float ay;
    float prevVy;

    // ==== roll ====
    float roll;//차체 기울기, roll angle (rad)
    float rollRate;// roll rate (rad/s)
    float rollAcc;

    // ==== 회전 ====
    float r;
    float yaw;

    //==== 위치 ====
    Vec3 position;

    // ===== 좌우 하중 =====
    float staticFz;
    float FzFL;
    float FzFR;
    float FzRL;
    float FzRR;

    // ==== 엔진 모델 ====
    float throttle; // 0~1, 엔진 힘
    float FxEngine => throttle * 4000f;
    float brakeInput; // 0~1, 브레이크
    float brakeForce;
    float FxBrake => brakeInput * brakeForce * MathUtility.Sin(Ux);
    float dragCoeff = 0.5f;// 공기저항
    float FxDrag;
    float brake;
    float maxBrakeForce;
    float maxDriveForce;

    // ===== 종력 =====
    float Fx;
    float FxDriveFront;
    float FxDriveRear;
    float FxBrakeFront;
    float FxBrakeRear;
    float FxFront;
    float FxRear;

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


    private void Start()
    {
        InitForce();
    }

    private void FixedUpdate()
    {
        // 1. Slip angle
        ComputeSlipAngle();

        // 2. Tire force (현재 Fz 기반)
        ComputeTireForce();

        // 3. ay 계산
        ay = (Fy_f + Fy_r) / m;

        // 4. Roll dynamics
        UpdateRollDynamics(ay, Time.fixedDeltaTime);

        // 5. Weight transfer → Fz 업데이트
        UpdateWeightTransfer();
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
    /// 하중 계산
    /// </summary>
    void UpdateWeightTransfer()
    {
        float dF_front = (KphiFront * roll) / trackWidth;
        float dF_rear = (KphiRear * roll) / trackWidth;

        // 앞축
        float leftFront = staticFront / 2 + dF_front / 2;
        float rightFront = staticFront / 2 - dF_front / 2;

        // 뒤축
        float leftRear = staticRear / 2 + dF_rear / 2;
        float rightRear = staticRear / 2 - dF_rear / 2;

        // 적용
        FzFL = MathUtility.Max(leftFront, 0f);
        FzFR = MathUtility.Max(rightFront, 0f);
        FzRL = MathUtility.Max(leftRear, 0f);
        FzRR = MathUtility.Max(rightRear, 0f);

        //축 단위 합
        FzFront = FzFL + FzFR;
        FzRear = FzRL + FzRR;
    }
}
