using System.Collections.Generic;
// Joint
// - ConstraintRow를 생성한다
// - Solver 로직 없음
// - RigidBody 상태 직접 변경 금지
public abstract class Joint
{
    //Joint 고유 번호
    public int jointId;

    // Joint 내부에서만 사용 (Jacobian 계산용)
    protected CustomRigidBody rigidA;
    protected CustomRigidBody rigidB;

    protected readonly List<ConstraintRow> constraintRows
        = new List<ConstraintRow>();

    /// <summary>
    /// Jacobian / effectiveMass / bias 정의
    /// </summary>
    public abstract void BuildConstraintRows(float dt);
}
