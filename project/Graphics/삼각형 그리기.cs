using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polygon : MonoBehaviour
{
    [SerializeField]
    private MeshFilter meshFilter;

    public void Start()
    {
        Creating();
    }

    public void Creating()
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[]
        {
            new Vector3(0,1,0),
            new Vector3(1,-1,0),
            new Vector3(-1,-1,0)
        };

        int[] indexes = new int[] { 0, 1, 2 };

        mesh.vertices = vertices;
        mesh.triangles = indexes;

        meshFilter.mesh = mesh;
    }
}
