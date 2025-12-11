using System;
using UnityEngine;

public class TestScripts : MonoBehaviour
{
    public enum Integrator { Euler, SemiImplicit, Verlet ,RK4}
    public Integrator mode = Integrator.Euler;

    public CustomRigidBody rigid;
    public float k;//스프링 상수
    
    void Awake()
    {
        LineRenderer lr = GetComponent<LineRenderer>();
        lr.positionCount = 3000;
        
    }

    void FixedUpdate()
    {
        //rigid.accumulatedForce = VectorMathUtils.RightVector3D()* MathF.Sin(Time.fixedTime);
        //rigid.PhysicsStep(0.02f);
        float x = rigid.currentState.position.x;
        float v = rigid.currentState.velocity.x;

        float E = 0.5f *k* x * x + 0.5f * rigid.mass.value * v * v;

        Debug.Log($"x={x:F4} / v={v:F4} / Energy={E:F6}");

        AddGraphPoint(E);
    }

    int index = 0;
    void AddGraphPoint(float energy)
    {
        LineRenderer lr = GetComponent<LineRenderer>();
        if (index >= lr.positionCount) return;

        lr.SetPosition(index, new Vector3(index * 0.02f, energy, 0));
        index++;
    }
}
