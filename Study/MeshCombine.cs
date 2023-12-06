using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class InstanceCombiner : MonoBehaviour
{
    [SerializeField] private List<MeshFilter> listMeshFilter;
    [SerializeField] private MeshFilter TargetMesh;

    [ContextMenu("Conbine Meshes")]
    void CombineMesh()
    {
        var combine = new CombineInstance[listMeshFilter.Count];//배열 생성

        //메쉬 설정
        for(int i = 0; i < listMeshFilter.Count; i++)
        {
            combine[i].mesh = listMeshFilter[i].sharedMesh;//공유메시 수집
            combine[i].transform = listMeshFilter[i].transform.localToWorldMatrix;
        }

        var mesh = new Mesh();//빈 메시 만들기
        mesh.CombineMeshes(combine);

        TargetMesh.mesh = mesh;
        
    }
    
}
