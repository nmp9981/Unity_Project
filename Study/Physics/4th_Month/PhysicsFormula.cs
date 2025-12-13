
public static class PhysicsFormula
{
    /// <summary>
    /// 훅의 법칙 적용해 힘 구하기 - 3차원
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public static Vec3 Force_HookLaw(Vec3 position)
    {
        return position * (-1)*TestScripts.k;
    }
    /// <summary>
    /// 훅의 법칙 적용해 힘 구하기 - 2차원
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public static Vec2 Force_HookLaw2D(Vec3 position)
    {
        Vec2 pos = new Vec2(position.x, position.y);
        return pos * (-1) * TestScripts.k;
    }
}
