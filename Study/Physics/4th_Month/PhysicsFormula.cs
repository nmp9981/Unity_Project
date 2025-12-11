
public static class PhysicsFormula
{
    /// <summary>
    /// 훅의 법칙 적용해 힘 구하기
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public static Vec3 Force_HookLaw(Vec3 position)
    {
        return position * (-1)*TestScripts.k;
    }
}
