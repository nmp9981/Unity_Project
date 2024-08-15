using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshVertex : MonoBehaviour
{
    void Start()
    {
        NumOfMesh();
    }

    void NumOfMesh()
    {
        Mesh mesh = gameObject.GetComponent<MeshFilter>().mesh;
        Debug.Log("정점 개수 "+mesh.vertexCount);
        Vector3[] vertexList = mesh.vertices;
        
        for(int idx=0;idx<vertexList.Length-2;idx++)
        {
            Vector3 ver0 = vertexList[idx];
            Vector3 ver1 = vertexList[idx + 1];
            Vector3 ver2 = vertexList[idx + 2];

            Vector3 MeshDir = MeshDirection(ver0,ver1,ver2);
            Debug.Log("법선 벡터 "+MeshDir.x + " " + MeshDir.y + " " + MeshDir.z);
        }
    }
    Vector3 MeshDirection(Vector3 v0, Vector3 v1, Vector3 v2)
    {
        Vector3 ab = v1 - v0;
        Vector3 ac = v2 - v0;
        Vector3 crossVec = Vector3.Cross(ab, ac);
        Vector3 dirVec = crossVec.normalized;
        return dirVec;
    }
}
