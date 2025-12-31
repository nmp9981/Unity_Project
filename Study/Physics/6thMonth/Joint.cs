abstract class Joint
{
    public CustomRigidBody rigidA;
    public CustomRigidBody rigidB;

    public abstract void SolveVelocity();
    public abstract void SolvePosition();
}
