abstract class Joint
{
    public CustomRigidBody rigidA;
    public CustomRigidBody rigidB;

    public abstract void SolveVelocity(float dt);
    public abstract void SolvePosition(float dt);
    public abstract void WarmStart();
}
