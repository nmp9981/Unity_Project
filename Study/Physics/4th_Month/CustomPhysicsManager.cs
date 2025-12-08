using UnityEngine;

enum Dimension { TwoD, ThreeD }

public class CustomPhysicsManager : MonoBehaviour
{
    [SerializeField]
    Dimension mode;//2D, 3D 결정

    public PhysicsWorld2D world2D;
    public PhysicsWorld world3D;
    public float fixedDt = 0.02f;

    void Start()
    {
        world2D = FindAnyObjectByType<PhysicsWorld2D>();
        world3D = FindAnyObjectByType<PhysicsWorld>();
        SceneBootstrap(mode);    // rigidbody & collider 생성
    }

    void FixedUpdate()
    {
        if (mode == Dimension.TwoD)
        {
            if (!world2D.physicsPaused)
                world2D.PhysicsStep(fixedDt);
        }
        else
        {
            if (!world3D.physicsPaused)
                world3D.PhysicsStep(fixedDt);
        }
    }

    /// <summary>
    /// rigidbody & collider 생성
    /// </summary>
    /// <param name="world"></param>
    void SceneBootstrap(Dimension mode)
    {
        if (mode == Dimension.TwoD) {
            world2D.Create_ColliderAndRigid2D();
        }
        else
        {
            world3D.Create_ColliderAndRigid3D();
        }
    }
}
