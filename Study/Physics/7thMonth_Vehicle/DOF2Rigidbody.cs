using Unity.VisualScripting;
using UnityEngine;

public class DOF2Rigidbody : MonoBehaviour
{
    float delta=0.05f;//고정 조향각
    float currentUx,prevUx;
    float a = 1.2f;//55%
    float b = 1.6f;//45%
    float h = 0.55f;//세단 높이, CG Height
    float L = 2.8f;
    float m = 1500;
    float FzFront, FzRear, staticFront, staticRear;
    float mu=1.0f;//마른 아스팔트 기준
    float Cf = 40000;
    float Cr = 30000;
    float vy_dot, r_dot;
    float Iz = 1;
    Vector3 local;

    float escThreshold = 0.2f;  // rad/s
    float steerThreshold = 0.02f;   // rad
    float minSpeed = 5f;        // m/s

    CustomRigidBody rb;

    // ===== 상태 =====
    float Ux;
    float Vy;
    float ay;

    // ==== roll ====
    float phi;       // roll angle (rad)
    float phiDot;    // roll rate (rad/s)

    // ==== 회전 ====
    float r;
    float yaw;

    //==== 위치 ====
    Vec3 position;

    // ===== 좌우 하중 =====
    float FzFL;
    float FzFR;
    float FzRL;
    float FzRR;

    // ==== 엔진 모델 ====
    float throttle; // 0~1, 엔진 힘
    float FxEngine => throttle * 4000f;
    float brakeInput; // 0~1, 브레이크
    float brakeForce;
    float FxBrake => brakeInput * brakeForce*MathUtility.Sin(Ux);
    float dragCoeff = 0.5f;// 공기저항
    float FxDrag;

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
    float FyF;
    float FyR;

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
    float KphiTotal => KphiFront+KphiRear;
    float Cphi = 3000f;        // roll damping

    private void Start()
    {
        Vector3 velocity = new Vector3(rb.velocity.x, rb.velocity.y, rb.velocity.z);
        local = transform.InverseTransformDirection(velocity);
        vy = local.y;
        r = rb.angularVelocity.y;
    }
    private void FixedUpdate()
    {
        VehicleStep(Time.fixedDeltaTime);
    }
    /// <summary>
    /// 28주차 기록용
    /// </summary>
    /// <param name="dt"></param>
    void DOF2Flow(float dt)
    {
        //1.현재 속도 추출
        Vector3 velocity = new Vector3(rb.velocity.x, rb.velocity.y, rb.velocity.z);
        local = transform.InverseTransformDirection(velocity);
        Ux = local.z;
        prevUx = currentUx;
        currentUx = Ux;
        //2.슬립각 계산
        float U = Mathf.Max(Mathf.Abs(Ux), 0.1f);
        float alphaF = delta - (Vy + a * r) / U;
        float alphaR = (b * r - Vy) / U;
        //3.하중 이동 계산
        float ax = (currentUx - prevUx) / dt;
        float transfer = (h / L) * m * ax;

        FzFront = staticFront + transfer;
        FzFront = MathUtility.Max(FzFront, 0);
        FzRear = staticRear - transfer;
        FzRear = MathUtility.Max(FzRear,0);

        //4.타이어 횡력 계산(비선형)
        float FyF = mu * FzFront * MathUtility.Tanh(Cf * alphaF / (mu * FzFront));
        float FyR = mu * FzRear * MathUtility.Tanh(Cr * alphaR / (mu * FzRear));
        //5.차량 횡력 / 요 모멘트 계산
        vy_dot = (FyF + FyR) / m - U * r;
        r_dot = (a * FyF - b * FyR) / Iz;
        //6.상태 미분 계산
        //7.적분(Euler 또는 RK2)
        Vy += vy_dot * dt;
        r += r_dot * dt;
        //8.Rigidbody에 반영
        Vec3 localVel = new Vec3(0, Vy, Ux);
        Vector3 vel = transform.TransformDirection(new Vector3(localVel.x, localVel.y, localVel.z));
        rb.velocity = new Vec3(vel.x, vel.y,vel.z);
        rb.angularVelocity = new Vec3(0, r, 0);
    }
    /// <summary>
    /// 29주차 이후
    /// </summary>
    /// <param name="dt"></param>
    void VehicleStep(float dt)
    {
        ReadState();
        UpdateLongitudinalDynamics(dt);
        ComputeDerivedStates();
        UpdateRollDynamics(dt);

        ComputeLongitudinalForces();
        ComputeSlipAngles();
        ApplyLongitudinalLoadTransfer();
        ApplyLateralLoadTransfer();

        ComputeLateralForces();
        ApplyFrictionCircle();
        ApplyForcesToRigidBody();
        ApplyTransform(dt);
        ApplyESC();
    }
    /// <summary>
    /// 속도 값 읽어오기
    /// </summary>
    void ReadState()
    {
        //종력 계산
        Fx = throttle * maxDriveForce - brake * maxBrakeForce;

        Vec3 localVel = transform.InverseTransformDirection(rb.velocity);
        Ux = localVel.z;
        Vy = localVel.y;
        r = rb.angularVelocity.y;
    }
    /// <summary>
    /// 종방향 차량 업데이트
    /// </summary>
    /// <param name="dt"></param>
    void UpdateLongitudinalDynamics(float dt)
    {
        float FxDrag = dragCoeff * Ux * MathUtility.Abs(Ux);
        float Fx = FxEngine - FxBrake - FxDrag;
        float UxDot = Fx / m + Vy * r;

        Ux += UxDot * dt;

        // 속도 제한
        float maxSpeed = 60f;
        Ux = MathUtility.ClampValue(Ux, -maxSpeed, maxSpeed);

        // 저속 안정화
        if (MathUtility.Abs(Ux) < 0.1f)
        {
            Ux = 0f;
        }
    }
    /// <summary>
    /// 횡가속도 계산
    /// </summary>
    void ComputeDerivedStates()
    {
        if (MathUtility.Abs(Ux) < 1f)
            ay = 0f;
        else
            ay = Ux * r;
    }
    /// <summary>
    /// 롤 운동 방정식
    /// </summary>
    /// <param name="dt"></param>
    void UpdateRollDynamics(float dt)
    {
        // 롤 모멘트 계산 (비선형)
        float rollMoment =
            -m * h * ay * Mathf.Cos(phi)
            - KphiTotal * Mathf.Sin(phi)
            - Cphi * phiDot;

        // 각가속도
        float phiDDot = rollMoment / Ixx;

        // 적분
        phiDot += phiDDot * dt;
        phi += phiDot * dt;
        phi = MathUtility.ClampValue(phi, -15f *MathUtility.Deg2Rad, 15f * MathUtility.Deg2Rad);//RollClamp

        //저속 안정화
        if (MathUtility.Abs(Ux) < 1f)
        {
            phi *= 0.98f;
            phiDot *= 0.9f;
        }
    }
    void ComputeLongitudinalForces()
    {
        //구동력
        FxDriveFront = 0f;
        FxDriveRear = throttle * maxDriveForce;

        //Brake 6:4분배
        float brakeForce = brake * maxBrakeForce;

        FxBrakeFront = -0.6f * brakeForce;
        FxBrakeRear = -0.4f * brakeForce;

        FxFront = FxDriveFront + FxBrakeFront;
        FxRear = FxDriveRear + FxBrakeRear;
    }

    /// <summary>
    /// 슬립각 계산
    /// </summary>
    void ComputeSlipAngles()
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
    /// 하중 계산
    /// </summary>
    void ApplyLongitudinalLoadTransfer()
    {
        // 3. 하중 계산
        float FxTotal = FxFront + FxRear;
        float transfer = (h / L) * FxTotal;

        FzF = MathUtility.Max(0,staticFront + transfer);
        FzR = MathUtility.Max(0, staticRear - transfer);
    }
    /// <summary>
    /// 좌우 하중 적용
    /// </summary>
    void ApplyLateralLoadTransfer()
    {
        float totalDelta = (KphiTotal * phi) / trackWidth;

        float frontRatio = KphiFront / KphiTotal;
        float rearRatio = KphiRear / KphiTotal;

        float deltaFront = frontRatio * totalDelta;
        float deltaRear = rearRatio * totalDelta;

        // Front axle
        FzFL = FzF * 0.5f - deltaFront;
        FzFR = FzF * 0.5f + deltaFront;

        // Rear axle
        FzRL = FzR * 0.5f - deltaRear;
        FzRR = FzR * 0.5f + deltaRear;

        // 음수 방지
        FzFL = MathUtility.Max(FzFL, 0);
        FzFR = MathUtility.Max(FzFR, 0);
        FzRL = MathUtility.Max(FzRL, 0);
        FzRR = MathUtility.Max(FzRR, 0);
    }
    /// <summary>
    /// 횡력 계산
    /// </summary>
    void ComputeLateralForces()
    {
        FyF = mu * FzF * MathUtility.Tanh(Cf * alphaF / (mu * FzF));
        FyR = mu * FzR * MathUtility.Tanh(Cr * alphaR / (mu * FzR));
    }

    void ApplyFrictionCircle()
    {
        //u-circle 전륜
        float FmaxF = mu * FzF;//원의 반지름
        float magF = MathUtility.Root(FxFront * FxFront + FyF * FyF);
        if (FmaxF > 0.0001f && magF > FmaxF)
        {
            float scale = FmaxF / magF;
            FxFront *= scale;
            FyF *= scale;
        }
        //u-circle 후륜
        float FmaxR = mu * FzR;
        float magR = MathUtility.Root(FxRear * FxRear + FyR * FyR);
        if (FmaxR > 0.0001f && magR > FmaxR)
        {
            float scale = FmaxR / magR;
            FxRear *= scale;
            FyR *= scale;
        }
    }

    /// <summary>
    /// RigidBody에 힘 적용
    /// </summary>
    void ApplyForcesToRigidBody()
    {
        //힘 조립
        float FxTotal = FxFront + FxRear;
        float FyTotal = FyF + FyR;

        // 6. Force 변환
        Vec3 forceLocal = new Vec3(0, FyTotal, FxTotal);
        //TODO : Transform Directio함수 직접 구현
        Vec3 forceWorld = transform.TransformDirection(forceLocal);

        rb.AddForce(forceWorld);
    }
    /// <summary>
    /// 위치 적용
    /// </summary>
    void ApplyTransform(float dt)
    {
        //차량 좌표 -> 월드 좌표
        float cosYaw = MathUtility.Cos(yaw);
        float sinYaw = MathUtility.Sin(yaw);

        float worldVx = Ux * cosYaw - Vy * sinYaw;
        float worldVz = Ux * sinYaw + Vy * cosYaw;

        //위치 업데이트
        position.x += worldVx * dt;
        position.z += worldVz * dt;
      
        transform.position = new Vector3(position.x,transform.position.y,position.z);

        CustomQuaternion rot = QuaternionUtility.Euler(0f, yaw * MathUtility.Rad2Deg, -phi * MathUtility.Rad2Deg);
        transform.rotation = rot;
    }
    /// <summary>
    /// ESC 적용
    /// </summary>
    void ApplyESC()
    {
        if (!escEnabled) return;

        // 7. 요 모멘트
        float r_ref = (Ux / L) * delta;//이론 값 
        float r_error = r - r_ref;//실제 값과의 오차

        bool oversteer = r_error > escThreshold && MathUtility.Abs(delta) > steerThreshold && Ux > minSpeed;
        if (oversteer)//오버스티어인 경우에마 ECS작동
        {
            float escYaw = -escGain * r_error;
            escYaw = MathUtility.ClampValue(escYaw, -maxEscYaw, maxEscYaw);
            rb.AddTorque(new Vec3(0, escYaw, 0));
        }
    }
}
