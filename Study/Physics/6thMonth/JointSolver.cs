using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class JointSolver : MonoBehaviour, IConstraint
{
    float dt;
    public void SolveJoint()
    {
        foreach (contact)
        {
            SolveVelocity(dt);
        }
        foreach (contact)
        {
            SovlePosition(dt);
        }
    }

    public void SolveVelocity(float dt)
    {
        Vector3 pA = bodyA.position + rA;
        Vector3 pB = bodyB.position + rB;

        Vector3 n = pB - pA;
        float dist = n.magnitude;
        n /= dist;

        // 상대 속도
        Vector3 vRel =
            bodyB.velocity + Vector3.Cross(bodyB.angularVelocity, rB) -
            bodyA.velocity - Vector3.Cross(bodyA.angularVelocity, rA);

        // bias
        float C = dist - restLength;
        float b = beta / dt * C;

        // λ
        float lambda = -(Vector3.Dot(n, vRel) + b) * effectiveMass;

        // impulse
        Vector3 impulse = lambda * n;

        // 적용
        bodyA.velocity -= impulse * bodyA.invMass;
        bodyB.velocity += impulse * bodyB.invMass;

        bodyA.angularVelocity -= bodyA.invInertia * Vector3.Cross(rA, impulse);
        bodyB.angularVelocity += bodyB.invInertia * Vector3.Cross(rB, impulse);
    }
    public void SovlePosition(float dt)
    {
        Vector3 correction = C * n;

        bodyA.position += correction * bodyA.invMass / totalInvMass;
        bodyB.position -= correction * bodyB.invMass / totalInvMass;
    }

     //여기서는 loop만 돈다.
 public void SolveJointVelocity(float dt)
 {
     foreach(var joint in jointList)
     {
         joint.SolveVelocity(dt);
     }
 }
 public void SolveJointPosition(float dt)
 {
     foreach (var joint in jointList)
     {
         joint.SolvePosition(dt);
     }
 }
}
