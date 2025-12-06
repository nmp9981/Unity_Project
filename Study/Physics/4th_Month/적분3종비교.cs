using UnityEngine;

public class TestScripts : MonoBehaviour
{
    public enum Integrator { Euler, SemiImplicit, Verlet }
    public Integrator mode = Integrator.Euler;

    public float x = 1f;
    public float v = 0f;

    float prevX;
    public float dt = 0.02f;

    void Start()
    {
        prevX = x;
        LineRenderer lr = GetComponent<LineRenderer>();
        lr.positionCount = 2000;
    }

    void FixedUpdate()
    {
        switch (mode)
        {
            case Integrator.Euler: ExplicitEuler(); break;
            case Integrator.SemiImplicit: SemiImplicit(); break;
            case Integrator.Verlet: Verlet(); break;
        }

        float energy = 0.5f * (x * x + v * v);
        AddGraphPoint(energy);
    }

    void ExplicitEuler()
    {
        float newX = x + v * dt;
        float newV = v - x * dt;

        x = newX;
        v = newV;
    }

    void SemiImplicit()
    { 
        float newV = v - x * dt;
        float newX = x + v * dt;

        v = newV;
        x = newX;
    }

    void Verlet()
    {
        float newX = 2f * x - prevX - x * dt * dt;
        prevX = x;
        x = newX;
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
