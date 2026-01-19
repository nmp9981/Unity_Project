using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
public class Motor
{
    public int simStep = 0;               // 현재 시뮬레이션 인덱스
    private int savedSampleCount = 0;      // 저장된 출력 개수
    private const int saveBufferLimit = 480;
    private double uiv = 0.0, uii = 0.0, ei = 0.0, ucv = 0.0;
    private List<double> wdi_k = new List<double>();
    // 모터 파라미터
    public double R, L, J, Ke, Kt, b;
    // PI 제어기 파라미터
    public double Kpv, Kiv, Kpi, Kii;        // 시뮬레이션 파라미터

    //시뮬레이션 동작 파라미터 
    public double Td, dt, tmax;
    public int nt;
    public int controlPeriod;
    public bool controlMode=false;
    // 시뮬레이션 벡터
    public double[] ti, r, w_dt;
    public double[,] I_k, W_k;

    public List<double> VelocityOut { get; private set; }

    public Motor(double R, double L, double J, double Ke, double Kt, double b,
         double Kpv, double Kiv, double Kpi, double Td)
    {
        this.R = R; this.L = L; this.J = J;
        this.Ke = Ke; this.Kt = Kt; this.b = b;
        this.Kpv = Kpv; this.Kiv = Kiv; this.Kpi = Kpi;
        this.Kii = Kpi * R / L;
        this.Td = Td;
        dt = 0.00001; tmax = 0.05;
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
            r[i] = 1.0;
            I_k[i, 0] = 0.0;
            W_k[i, 0] = 0.0;
        }
    }
    public void RunSimulationControlPeriodIcontrol()
    {
        double uiv = 0.0, uii = 0.0, ei = 0.0, ucv = 0.0;
        List<double> wdi_k = new List<double>(); // 제어 주기마다 저장된 속도

        for (int i = 0; i < nt - 1; i++)
        {
            // PI 속도 제어 루프 (controlPeriod 주기마다 실행)
            if (i % controlPeriod == 0)
            {
                double ev = r[i] - W_k[i, 0];
                double upv = Kpv * ev;
                uiv += Kiv * ev * dt * controlPeriod;  // 10배 시간 보정
                ucv = upv + uiv;
            }

            // PI 전류 제어 루프 (매 스텝 실행)
            ei = ucv - I_k[i, 0];
            double upi = Kpi * ei;
            uii += Kii * ei * dt;
            double uci = Util.Sat(upi + uii, 240); // 전압 포화기

            // 전기적 시스템 업데이트 (RL)
            double input_elec = uci - Ke * W_k[i, 0];
            double[] next_I = Util.RK4_i(this, Util.TF1e, input_elec, new double[] { I_k[i, 0] }, dt);
            I_k[i + 1, 0] = next_I[0];

            // 기계적 시스템 업데이트 (JB)
            double input_mech = Kt * I_k[i + 1, 0] + Td;
            double[] next_W = Util.RK4_i(this, Util.TF1m, input_mech, new double[] { W_k[i, 0] }, dt);
            W_k[i + 1, 0] = next_W[0];

            // 속도 결과 저장 (제어 주기마다만 저장)
            if (i % controlPeriod == 0)
            {
                wdi_k.Add(W_k[i + 1, 0]);

            }
            VelocityOut = wdi_k;
        }
        Debug.Log($"제어 주기마다 저장된 출력 개수: {wdi_k.Count}");
    }
    public float RunSimulationControlPeriodIcontrol160()
    {
        int stepsPerCall = 160;

        for (int i = simStep; i < Math.Min(simStep + stepsPerCall, nt - 1); i++)
        {
            // PI 속도 제어 루프 (controlPeriod 주기마다 실행)
            if (i % controlPeriod == 0)
            {
                double ev = r[i] - W_k[i, 0];
                double upv = Kpv * ev;
                uiv += Kiv * ev * dt * controlPeriod;
                ucv = upv + uiv;
            }

            // PI 전류 제어 루프
            ei = ucv - I_k[i, 0];
            double upi = Kpi * ei;
            uii += Kii * ei * dt;
            double uci = Util.Sat(upi + uii, 240);

            // 전기적 시스템 업데이트 (RL)
            double input_elec = uci - Ke * W_k[i, 0];
            double[] next_I = Util.RK4_i(this, Util.TF1e, input_elec, new double[] { I_k[i, 0] }, dt);
            I_k[i + 1, 0] = next_I[0];

            // 기계적 시스템 업데이트 (JB)
            double input_mech = Kt * I_k[i + 1, 0] + Td;
            double[] next_W = Util.RK4_i(this, Util.TF1m, input_mech, new double[] { W_k[i, 0] }, dt);
            W_k[i + 1, 0] = next_W[0];

            // 버퍼 저장은 처음 480개까지만
            if (i % controlPeriod == 0 && savedSampleCount < saveBufferLimit)
            {
                wdi_k.Add(W_k[i + 1, 0]);
                savedSampleCount++;
                Debug.Log($"제어 주기마다 저장된 출력 개수: {wdi_k.Count}");
            }
        }
        simStep += stepsPerCall;
        if (savedSampleCount <= saveBufferLimit)
            VelocityOut = new List<double>(wdi_k);
        int safeIndex = Mathf.Min(simStep, nt - 2); // 최소 1칸 여유!
        return (float)W_k[safeIndex, 0];
    }
    public float RunSimulationControlPeriodIcontrolFps(int stepsPerCall)
    {
        //int stepsPerCall = 160;
        for (int i = simStep; i < Math.Min(simStep + stepsPerCall, nt - 1); i++)
        {
            // PI 속도 제어 루프 (controlPeriod 주기마다 실행)
            if (i % controlPeriod == 0)
            {
                double ev = r[i] - W_k[i, 0];
                double upv = Kpv * ev;
                uiv += Kiv * ev * dt * controlPeriod;
                ucv = upv + uiv;
            }

            // PI 전류 제어 루프
            ei = ucv - I_k[i, 0];
            double upi = Kpi * ei;
            uii += Kii * ei * dt;
            double uci = Util.Sat(upi + uii, 240);

            // 전기적 시스템 업데이트 (RL)
            double input_elec = uci - Ke * W_k[i, 0];
            double[] next_I = Util.RK4_i(this, Util.TF1e, input_elec, new double[] { I_k[i, 0] }, dt);
            I_k[i + 1, 0] = next_I[0];

            // 기계적 시스템 업데이트 (JB)
            double input_mech = Kt * I_k[i + 1, 0] + Td;
            double[] next_W = Util.RK4_i(this, Util.TF1m, input_mech, new double[] { W_k[i, 0] }, dt);
            W_k[i + 1, 0] = next_W[0];

            // 버퍼 저장은 처음 480개까지만
            if (i % controlPeriod == 0 && savedSampleCount < saveBufferLimit)
            {
                wdi_k.Add(W_k[i + 1, 0]);
                savedSampleCount++;
                Debug.Log($"제어 주기마다 저장된 출력 개수: {wdi_k.Count}");
            }
        }
        simStep += stepsPerCall;
        if (savedSampleCount <= saveBufferLimit)
            VelocityOut = new List<double>(wdi_k);
        int safeIndex = Mathf.Min(simStep, nt - 2); // 최소 1칸 여유!
        return (float)W_k[safeIndex, 0];
    }
    public void SaveResultToCSV(string filename)
    {
        string fullPath = Path.Combine(Environment.CurrentDirectory, filename);

        using (StreamWriter writer = new StreamWriter(fullPath))
        {
            // 헤더
            writer.WriteLine("Time(s),Input(r),Output(w)");

            for (int i = 0; i < VelocityOut.Count; i++)
            {
                int idx = i * controlPeriod;

                string line = ti[idx].ToString("F6", CultureInfo.InvariantCulture) + "," +
                              r[idx].ToString("F6", CultureInfo.InvariantCulture) + "," +
                              VelocityOut[i].ToString("F6", CultureInfo.InvariantCulture);
                writer.WriteLine(line);
            }
        }
        Debug.Log($"CSV 저장 완료: {fullPath}");
    }
}
