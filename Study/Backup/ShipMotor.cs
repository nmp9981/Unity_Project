public int simStep = 0;               // 현재 시뮬레이션 인덱스
private int savedSampleCount = 0;      // 저장된 샘플 개수
private const int saveBufferLimit = 1000;
private double uiv = 0.0, uii = 0.0, ei = 0.0, ucv = 0.0;
private List<double> wdi_k = new List<double>();

// 모터 파라미터
public double R, L, J, Ke, Kt, b;
// PI 제어 파라미터

// 시뮬레이션 관련 파라미터 
public double dt, tmax;
public int nt;
public int controlPeriod;
public bool controlMode = false;

// 시뮬레이션 데이터
public double[] ti, r, w_dt;
public double[,] I_k, W_k;

public List<double> VelocityOut { get; private set; }
private IMotorController control; // 런타임 입력을 읽기 위한 참조

/// <summary>
/// 모터 시뮬레이터 생성자
/// </summary>
/// <param name="motorControl">모터 컨트롤러 참조</param>
public ShipMotor(IMotorController motorControl)
{
    control = motorControl;
    this.R = motorControl.R; this.L = motorControl.L; this.J = motorControl.J;
    this.Ke = motorControl.Ke; this.Kt = motorControl.Kt; this.b = motorControl.b;

    dt = 0.00001; tmax = 1.0;
    controlPeriod = 10;
    controlMode = false;
    nt = (int)(tmax / dt);
    ti = new double[nt];
    r = new double[nt];
    I_k = new double[nt, 1];
    W_k = new double[nt, 1];
    w_dt = new double[nt];

    for (int i = 0; i < nt; i++)
    {
        ti[i] = i * dt;
        r[i] = 0.0; // 런타임에 갱신
        I_k[i, 0] = 0.0;
        W_k[i, 0] = 0.0;
    }

    // VelocityOut 초기화
    VelocityOut = new List<double>();
}

/// <summary>
/// 시뮬레이션 실행 (제어 주기별 전압 입력, FPS 기반)
/// </summary>
/// <param name="stepsPerCall">프레임당 실행할 스텝 수</param>
/// <param name="wOut">출력 각속도 (rad/s)</param>
/// <param name="iOut">출력 전류 (A)</param>
public void RunSimulationControlPeriodVinputFps(int stepsPerCall, out float wOut, out float iOut)
{
    // 입력값 유효성 검사
    if (stepsPerCall <= 0)
    {
        wOut = 0f;
        iOut = 0f;
        return;
    }

    // 이미 호출된 시뮬레이션의 마지막 인덱스(배열 범위 고려)
    int endIndex = Math.Min(simStep + stepsPerCall, nt - 1);
    
    for (int i = simStep; i < endIndex; i++)
    {
        // 제어주기마다 런타임 입력 전압 갱신
        if (i % controlPeriod == 0)
        {
            r[i] = control != null ? control.motor_volt_in : r[i];
            ucv = r[i];
        }
        // 전압 포화제한
        ucv = MathUtility.Sat(ucv, 240);

        // 전기적 시스템 (RL 회로)
        double input_elec = ucv - Ke * W_k[i, 0];
        double[] next_I = MathUtility.RK4_i(this, MathUtility.TF1e, input_elec, new double[] { I_k[i, 0] }, dt);
        I_k[i + 1, 0] = next_I[0];

        // 기계적 시스템 (JB)
        double input_mech = Kt * I_k[i + 1, 0];
        double[] next_W = MathUtility.RK4_i(this, MathUtility.TF1m, input_mech, new double[] { W_k[i, 0] }, dt);
        W_k[i + 1, 0] = next_W[0];

        // 저장(제어 주기마다만 저장)
        if (i % controlPeriod == 0 && savedSampleCount < saveBufferLimit)
        {
            wdi_k.Add(W_k[i + 1, 0]);
            savedSampleCount++;
        }
    }

    // 마지막 실행된 시뮬레이션 인덱스 업데이트 (다음 호출 준비)
    simStep = endIndex;

    // 이미 호출된 시뮬레이션이 끝(nt-1)에 도달했다면 0 반환
    if (simStep >= nt - 1)
    {
        simStep = (nt*9)/10;
        //if (savedSampleCount <= saveBufferLimit)
        //    VelocityOut = new List<double>(wdi_k);
        //wOut = 0f;
        //iOut = 0f;
        //return;
    }

    if (savedSampleCount <= saveBufferLimit)
        VelocityOut = new List<double>(wdi_k);

    // 최소 1칸 뒤의 안전한 인덱스
    int safeIndex = Math.Min(simStep, nt - 2);

    // NaN 및 무한대 체크
    double wValue = W_k[safeIndex, 0];
    double iValue = I_k[safeIndex, 0];

    wOut = (double.IsNaN(wValue) || double.IsInfinity(wValue)) ? 0f : (float)wValue;
    iOut = (double.IsNaN(iValue) || double.IsInfinity(iValue)) ? 0f : (float)iValue;
}
