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
        //위치 업데이트
        Vec3 newX = curState.position + curState.velocity * dt + curState.accel * dt * dt*0.5f;

        //가속도 업데이트
        Vec3 newA = force / mass;

        //속도 업데이트
        Vec3 newV = curState.velocity+(curState.accel+newA)*dt*0.5f;

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

        //속도 업데이트
        Vec2 newV = curState.velocity + (curState.accel + newA) * dt * 0.5f;

        //다음 상태 반환
        return new RigidbodyState2D
        {
            position = newX,
            velocity = newV,
            accel = newA,
        };
    }
}
