using UnityEngine;

interface IConstraint
{
    //속도
    void SolveVelocity(float dt);
    //위치
    void SovlePosition(float dt);
}
