using UnityEngine;

public class CollisionSolver : MonoBehaviour
{
    public static float SolveImpulse(float m1, float m2, float relVelAlongNormal, float e)
    {
        return -(1 + e) * relVelAlongNormal / (1 / m1 + 1 / m2);
    }
}
