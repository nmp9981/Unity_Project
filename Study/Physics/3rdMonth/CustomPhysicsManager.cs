using System.Collections.Generic;
using UnityEngine;

public class CustomPhysicsManager : MonoBehaviour
{
    List<CustomRigidbody2D> rigidBodies2D = new List<CustomRigidbody2D>();
    List<CustomRigidBody> rigidBodies3D = new List<CustomRigidBody>();
    List<CustomCollider3D> colliders3D = new List<CustomCollider3D>();

    private void Awake()
    {
        rigidBodies2D.AddRange(FindObjectsByType<CustomRigidbody2D>(FindObjectsSortMode.None));
        rigidBodies3D.AddRange(FindObjectsByType<CustomRigidBody>(FindObjectsSortMode.None));
        colliders3D.AddRange(FindObjectsByType<CustomCollider3D>(FindObjectsSortMode.None));
    }

    private void FixedUpdate()
    {
        //모든 물리 step
        foreach (var rb in rigidBodies2D)
        {
            rb.Step(Time.fixedDeltaTime);
        }
        foreach (var rb in rigidBodies3D)
        {
            rb.Step(Time.fixedDeltaTime);
        }

        //충돌 체크
        Handle3DCollisions();
    }
    /// <summary>
    /// 충돌 체크
    /// </summary>
    void Handle3DCollisions()
    {
        for(int i = 0; i < colliders3D.Count; i++)
        {
            for(int j = i + 1; j < colliders3D.Count; j++)
            {
                if (ColiisionUtility.CheckAABBBox(colliders3D[i], colliders3D[j]))
                {
                    Debug.Log("충돌");
                }
            }
        }
    }
}
