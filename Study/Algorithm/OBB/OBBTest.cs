using System.Collections.Generic;
using UnityEngine;

public class OBBTest : MonoBehaviour
{
    OBBBoxClass obbBoxclass = new OBBBoxClass();
    
    // 메시를 합칠 대상이 되는 부모 오브젝트를 여기에 할당합니다.
    public List<MeshFilter> meshFilterList = new List<MeshFilter>();
    public List<MeshFilter> meshFilterList2 = new List<MeshFilter>();
    public List<MeshFilter> meshFilterList3 = new List<MeshFilter>();
    public List<MeshFilter> meshFilterList4 = new List<MeshFilter>();

    public List<List<MeshFilter>> meshFilterLists = new List<List<MeshFilter>>();

    [Tooltip("합쳐진 메시의 재질. 모든 자식 메시가 동일한 재질을 사용한다고 가정합니다.")]
    public Material combinedMaterial;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            meshFilterLists.Add(meshFilterList);
            meshFilterLists.Add(meshFilterList2);
            meshFilterLists.Add(meshFilterList3);
            meshFilterLists.Add(meshFilterList4);

            for (int i = 0; i < meshFilterLists.Count; i++)
            {
                OBB createObb = new OBB();
                createObb = obbBoxclass.MakeOBBAboutCombineMeshs(meshFilterLists[i], transform);
                Debug.Log(createObb.Center);
                obbBoxclass.DrawOBBBOX(createObb, this.gameObject);
            }
            
        }
    }

    /// <summary>
    /// 지정된 부모 오브젝트의 모든 자식 메시를 하나로 합칩니다.
    /// </summary>
    /// <param name="parent">메시를 합칠 대상 부모 오브젝트.</param>
    public Mesh CombineAllMeshes(List<MeshFilter> meshFilterList, Material combinedMaterial)
    {
        // 1. 합칠 MeshFilter 컴포넌트들을 수집합니다.
        Debug.Log($"총 {meshFilterList.Count}개의 메시를 합치려고 합니다.");

        // 데이터를 저장할 리스트를 초기화합니다.
        List<Vector3> newVertices = new List<Vector3>();
        List<int> newTriangles = new List<int>();
        List<Vector3> newNormals = new List<Vector3>();
        List<Vector2> newUVs = new List<Vector2>();

        // 현재 정점 인덱스 오프셋을 추적합니다.
        int vertexOffset = 0;

        // 2. 각 원본 메시의 데이터를 새로운 메시 데이터에 합칩니다.
        foreach (MeshFilter mf in meshFilterList)
        {
            if (mf == null || mf.sharedMesh == null) continue; // 유효하지 않은 메시 필터는 건너뜁니다.

            Mesh mesh = mf.sharedMesh;
            Transform meshTransform = mf.transform;

            // 각 정점을 월드 공간으로 변환하거나,
            // 이 스크립트가 붙은 GameObject (합쳐진 메시의 부모가 될) 로컬 공간으로 변환합니다.
            // 여기서는 스크립트가 붙은 GameObject의 로컬 공간으로 변환합니다.
            // 이 MeshCombiner가 Mesh를 소유할 새 GameObject에 붙어있다고 가정합니다.

            // 정점 변환: MeshFilter의 로컬 정점을 이 오브젝트 (MeshCombiner가 붙은)의 로컬 공간으로 변환
            // MeshFilter의 로컬 좌표 -> 월드 좌표 -> 이 오브젝트의 로컬 좌표
            foreach (Vector3 vertex in mesh.vertices)
            {
                newVertices.Add(transform.InverseTransformPoint(meshTransform.TransformPoint(vertex)));
            }

            // 법선 변환: MeshFilter의 로컬 법선을 이 오브젝트의 로컬 공간으로 변환
            foreach (Vector3 normal in mesh.normals)
            {
                newNormals.Add(transform.InverseTransformDirection(meshTransform.TransformDirection(normal)));
            }

            // UVs는 Transform에 영향을 받지 않습니다.
            newUVs.AddRange(mesh.uv);

            // 삼각형 인덱스 변환: 현재까지 합쳐진 정점 수만큼 오프셋을 더해줍니다.
            foreach (int triIndex in mesh.triangles)
            {
                newTriangles.Add(triIndex + vertexOffset);
            }

            // 다음 메시를 위한 정점 오프셋을 업데이트합니다.
            vertexOffset += mesh.vertexCount;

            // 원본 메시 렌더러는 비활성화하거나 제거합니다.
            MeshRenderer meshRenderer = mf.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.enabled = false; // 렌더링을 멈춥니다.
                // Destroy(meshRenderer); // 또는 아예 제거할 수도 있습니다.
            }
            // Destroy(mf); // MeshFilter도 제거할 수 있습니다.
        }

        // 3. 새로운 메시 생성 및 데이터 할당
        Mesh combinedMesh = new Mesh();
        combinedMesh.name = meshFilterList.Count + "_CombinedMesh";

        combinedMesh.vertices = newVertices.ToArray();
        combinedMesh.triangles = newTriangles.ToArray();
        combinedMesh.normals = newNormals.ToArray();
        combinedMesh.uv = newUVs.ToArray();

        // 4. 메시 최적화 및 후처리
        combinedMesh.Optimize(); // 최적화
        combinedMesh.RecalculateNormals(); // 법선을 다시 계산 (변환 후 필요할 수 있음)
        combinedMesh.RecalculateTangents(); // 탄젠트를 다시 계산 (노멀 맵 사용 시 필요)
        combinedMesh.RecalculateBounds(); // 바운딩 박스 다시 계산

        // 이 스크립트가 붙은 GameObject에 MeshFilter와 MeshRenderer를 추가하거나 가져옵니다.
        MeshFilter mainMeshFilter = GetComponent<MeshFilter>();
        if (mainMeshFilter == null)
        {
            mainMeshFilter = gameObject.AddComponent<MeshFilter>();
        }
        mainMeshFilter.mesh = combinedMesh;

        MeshRenderer mainMeshRenderer = GetComponent<MeshRenderer>();
        if (mainMeshRenderer == null)
        {
            mainMeshRenderer = gameObject.AddComponent<MeshRenderer>();
        }
        mainMeshRenderer.material = combinedMaterial; // 합쳐진 메시의 재질 할당

        Debug.Log("메시 합치기 완료! 새로운 메시의 정점 수: " + combinedMesh.vertexCount);

        return combinedMesh;
    }
}
