using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeAABB : MonoBehaviour
{
    private MeshRenderer meshRendeer;
    public Vector3[] meshPoints;

    public Vector3 minPos;
    public Vector3 maxPos;

    public bool isCollide = false;
    private void Awake()
    {
        meshRendeer = GetComponent<MeshRenderer>();
        MakeAABB_Box();
    }
    void MakeAABB_Box()
    {
        meshPoints = gameObject.GetComponent<Mesh>().vertices;

        minPos = meshPoints[0];
        maxPos = meshPoints[0];

        foreach (Vector3 vec in meshPoints)
        {
            if (vec.x <= minPos.x && vec.y <= minPos.y && vec.z <= minPos.z)
            {
                minPos = vec;
            }
            if (vec.x >= minPos.x && vec.y >= minPos.y && vec.z >= minPos.z)
            {
                maxPos = vec;
            }
        }
    }
    private void Update()
    {
        if (isCollide)
        {
            meshRendeer.material.color = Color.red;
        }
        else
        {
            meshRendeer.material.color = Color.blue;
        }
    }
}
