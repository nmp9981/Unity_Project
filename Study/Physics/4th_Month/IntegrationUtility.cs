using System.Runtime.ConstrainedExecution;

/// <summary>
/// 위치 속도 미분 - 2차원
/// </summary>
public struct Derivative2D
{
    public Vec3 dPosition;//위치 미분
    public Vec2 dVelocity;//속도 미분
}

/// <summary>
/// 위치 속도 미분 - 3차원
/// </summary>
public struct Derivative
{
    public Vec3 dPosition;//위치 미분
    public Vec3 dVelocity;//속도 미분
}

public static class IntegrationUtility
{
    /// <summary>
    /// Verlet 위치 수치적분 - 지울 예정
    /// </summary>
    /// <param name="dt">시간 간격</param>
    /// <param name="a">가속도</param>
    /// <param name="curX">현재 위치</param>
    /// <param name="preX">이전 위치</param>
    /// <returns>다음 위치</returns>
    public static float PositionVerlet(float dt, float a,float curX, float preX)
    {
        float nextX = 2 * curX - preX + a * dt * dt;
        return nextX;
    }

    /// <summary>
    /// Verlet 수치 적분 : 상태 반환
    /// </summary>
    /// <param name="dt">시간</param>
    /// <param name="curState">현재 상태</param>
    /// <param name="m">질량</param>
    /// <param name="force">힘</param>
    /// <returns></returns>
    public static RigidbodyState VelocityVerlet(float dt, RigidbodyState curState, Vec3 force, float mass)
    {
        Vec3 x = curState.position;
        Vec3 v = curState.velocity;
        Vec3 a = curState.accel;   // 이전 스텝 가속도

        //위치 업데이트
        Vec3 newX = x + v * dt + a * dt * dt*0.5f;

        //가속도 업데이트
        Vec3 newA = PhysicsFormula.Force_HookLaw(newX) / mass;

        //속도 업데이트
        Vec3 newV = v + (a + newA)*0.5f * dt;

        //다음 상태 반환
        return new RigidbodyState
        {
            position = newX,
            velocity = newV,
            accel = newA,
        };
    }
    /// <summary>
    /// Verlet 수치 적분 : 상태 반환
    /// </summary>
    /// <param name="dt">시간</param>
    /// <param name="curState">현재 상태</param>
    /// <param name="m">질량</param>
    /// <param name="force">힘</param>
    /// <returns></returns>
    public static RigidbodyState2D VelocityVerlet2D(float dt, RigidbodyState2D curState, Vec2 force, float mass)
    {
        //위치 업데이트
        Vec3 curPosition = new Vec3(curState.position.x, curState.position.y,0);
        Vec3 curVelocity = new Vec3(curState.velocity.x, curState.velocity.y, 0);
        Vec3 curAccel = new Vec3(curState.accel.x, curState.accel.y, 0);
        Vec3 newX = curPosition + curVelocity * dt + curAccel * dt * dt * 0.5f;

        //가속도 업데이트
        Vec2 newA = force / mass;

        //속도 업데이트(평균 가속도 사용)
        Vec2 newV = curState.velocity + (curState.accel + newA) * dt * 0.5f;

        //다음 상태 반환
        return new RigidbodyState2D
        {
            position = newX,
            velocity = newV,
            accel = newA,
        };
    }

    /// <summary>
    /// 미분
    /// </summary>
    /// <returns></returns>
    private static Derivative2D Evaluate2D(RigidbodyState2D state, Vec2 force, float mass)
    {
        Derivative2D d;
        d.dPosition = new Vec3(state.velocity.x, state.velocity.y,0);//위치 미분하면 속도
        d.dVelocity = force / mass;//속도 미분하면 가속도
        return d;
    }

    /// <summary>
    /// 미분
    /// </summary>
    /// <returns></returns>
    private static Derivative Evaluate(RigidbodyState state, float mass)
    {
        Derivative d;
        d.dPosition = state.velocity;//위치 미분하면 속도
        d.dVelocity = PhysicsFormula.Force_HookLaw(state.position) / mass;//속도 미분하면 가속도
        return d;
    }
    /// <summary>
    /// RK4 - 2D
    /// </summary>
    /// <param name="state">상태</param>
    /// <param name="force">힘</param>
    /// <param name="mass">질량</param>
    /// <param name="dt">시간차</param>
    /// <returns></returns>
    public static RigidbodyState2D Integrate2DRK4(RigidbodyState2D state, Vec2 force, float mass, float dt)
    {
        Derivative2D k1 = Evaluate2D(state, force, mass);

        RigidbodyState2D state2 = new RigidbodyState2D
        {
            position = state.position + k1.dPosition * 0.5f * dt,
            velocity = state.velocity + k1.dVelocity * 0.5f * dt
        };
        Derivative2D k2 = Evaluate2D(state2, force, mass);

        RigidbodyState2D state3 = new RigidbodyState2D
        {
            position = state.position + k2.dPosition * 0.5f * dt,
            velocity = state.velocity + k2.dVelocity * 0.5f * dt
        };
        Derivative2D k3 = Evaluate2D(state3, force, mass);

        RigidbodyState2D state4 = new RigidbodyState2D
        {
            position = state.position + k3.dPosition * dt,
            velocity = state.velocity + k3.dVelocity * dt
        };
        Derivative2D k4 = Evaluate2D(state4, force, mass);

        //가중치 합
        state.position += (k1.dPosition + k2.dPosition * 2 + k3.dPosition * 2 + k4.dPosition) * dt / 6;
        state.velocity += (k1.dVelocity + k2.dVelocity * 2 + k3.dVelocity * 2 + k4.dVelocity) * dt / 6;

        return state;
    }
    /// <summary>
    /// RK4 - 3D
    /// </summary>
    /// <param name="state">상태</param>
    /// <param name="force">힘</param>
    /// <param name="mass">질량</param>
    /// <param name="dt">시간차</param>
    /// <returns></returns>
    public static RigidbodyState IntegrateRK4(RigidbodyState state, Vec3 force, float mass, float dt)
    {
        Derivative k1= Evaluate(state, mass);

        RigidbodyState state2 = new RigidbodyState
        {
            position = state.position + k1.dPosition * 0.5f * dt,
            velocity = state.velocity + k1.dVelocity * 0.5f * dt
        };
        Derivative k2 = Evaluate(state2, mass);

        RigidbodyState state3 = new RigidbodyState
        {
            position = state.position + k2.dPosition * 0.5f * dt,
            velocity = state.velocity + k2.dVelocity * 0.5f * dt
        };
        Derivative k3 = Evaluate(state3, mass);

        RigidbodyState state4 = new RigidbodyState
        {
            position = state.position + k3.dPosition * dt,
            velocity = state.velocity + k3.dVelocity * dt
        };
        Derivative k4 = Evaluate(state4, mass);

        //가중치 합
        state.position += (k1.dPosition + k2.dPosition*2 + k3.dPosition*2 + k4.dPosition) * dt / 6;
        state.velocity += (k1.dVelocity + k2.dVelocity * 2 + k3.dVelocity * 2 + k4.dVelocity) * dt / 6;

        return state;
    }
}
