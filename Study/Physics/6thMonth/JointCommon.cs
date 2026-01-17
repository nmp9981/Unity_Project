using System;
using System.Collections.Generic;

public struct SolverRowResult
{
    // 어떤 Constraint인가
    public int jointId;
    public int rowIndex;

    // iteration 정보
    public int iteration;

    // Solver 계산값
    public float Cdot;          // 상대 속도 오차
    public float bias;          // 사용된 bias
    public float deltaImpulse;  // Impulse 변화량
    public float lambda;        // 이번 iteration에서 계산된 impulse
    public float accumulated;   // 누적 impulse

    // Clamp 결과
    public float minImpulse;
    public float maxImpulse;

    // 상태
    public bool active;         // 실제로 impulse가 적용되었는가
    public bool clamped;        // min/max에 걸렸는가
}

public static class JointCommon
{
    public static Dictionary<int, List<ConstraintRowDebug>> jointDebugMap;

    //생성자로 초기화
    static JointCommon()
    {
        jointDebugMap = new Dictionary<int, List<ConstraintRowDebug>>();
    }

    //Solver와 Joint 분리
    public static void BeginStep()
    {
        jointDebugMap.Clear();
    }
    public static void Collect( ConstraintRow row,float computedCdot,float deltaImpulse)
    {
        var debug = new ConstraintRowDebug
        {
            jointId = row.jointId,
            rowIndex = row.rowIndex,

            dof = row.dof,
            mode = row.mode,

            axisWorld = row.axisWorld,
            effectiveMass = row.effectiveMass,

            Cdot = computedCdot,
            bias = row.bias,

            impulse = row.lambda,
            accumulatedImpulse = row.accumulatedImpulse,

            active = row.isActive
        };
    }
   
    /// <summary>
    /// K값 계산
    /// </summary>
    /// <param name="axis"></param>
    /// <param name="rA"></param>
    /// <param name="rB"></param>
    /// <returns></returns>
    public static float ComputeLinearK(Vec3 axis, Vec3 rA, Vec3 rB, CustomRigidBody rigidA, CustomRigidBody rigidB)
    {
        float k = rigidA.invMass + rigidB.invMass
  + Vec3.Dot(Vec3.Cross(rA, axis), Vec3.Cross(rA, axis)) * rigidA.invInertia
  + Vec3.Dot(Vec3.Cross(rB, axis), Vec3.Cross(rB, axis)) * rigidB.invInertia;
        return k;
    }

    /// <summary>
    /// 상대 속도 계산
    /// </summary>
    /// <returns></returns>
    public static Vec3 GetRelativeVelocity(Vec3 rA, Vec3 rB, CustomRigidBody rigidA, CustomRigidBody rigidB)
    {
        //상대 속도 계산
        Vec3 vRel = rigidB.velocity + Vec3.Cross(rigidB.angularVelocity, rB) - rigidA.velocity - Vec3.Cross(rigidA.angularVelocity, rA);
        return vRel;
    }

    /// <summary>
    /// 슬라이더 거리
    /// </summary>
    /// <returns></returns>
    public static float CurrentDistanceAlongAxis(Vec3 rA, Vec3 rB, CustomRigidBody rigidA, CustomRigidBody rigidB, Vec3 localAxisA)
    {
        Vec3 axis = VectorMathUtils.GetWorldAxis(rigidA.rotation, localAxisA);
        Vec3 pA = rigidA.position + rA;
        Vec3 pB = rigidB.position + rB;

        //상대 위치, 월드 축 내적값
        return Vec3.Dot(pB - pA, axis);
    }
    /// <summary>
    /// Impulse 적용
    /// </summary>
    /// <param name="impulse"></param>
    /// <param name="rA"></param>
    /// <param name="rB"></param>
    public static void ApplyLinearImpulse(Vec3 impulse, Vec3 rA, Vec3 rB, CustomRigidBody rigidA, CustomRigidBody rigidB)
    {
        rigidA.velocity -= impulse * rigidA.invMass;
        rigidB.velocity += impulse * rigidB.invMass;

        rigidA.angularVelocity -= Vec3.Cross(rA, impulse) * rigidA.invInertia;
        rigidB.angularVelocity += Vec3.Cross(rB, impulse) * rigidB.invInertia;
    }
   
    /// <summary>
    /// Debug용 코드
    /// </summary>
    public static void DebugSnapshot(IEnumerable<Joint> joints, Dictionary<ConstraintRow, SolverRowResult> solverResults)
    {
        BeginStep();
        foreach (var joint in joints)
        {
            foreach (var row in joint.constraintRows)
            {
                var result = solverResults[row];
                Collect(row, result.Cdot, result.deltaImpulse);
            }
        }
    }
}
