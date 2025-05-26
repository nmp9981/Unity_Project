using System.Collections.Generic;
using UnityEngine;

public class OBBTest : MonoBehaviour
{
    OBBBoxClass obbBoxclass = new OBBBoxClass();

    [SerializeField] 
    List<MeshFilter>filters = new List<MeshFilter>();

    // 메시를 합칠 대상이 되는 부모 오브젝트를 여기에 할당합니다.
    public GameObject parentObjectToCombine;

    [Tooltip("합쳐진 메시의 재질. 모든 자식 메시가 동일한 재질을 사용한다고 가정합니다.")]
    public Material combinedMaterial;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Mesh newMesh = CombineMeshs(filters);
            obbBoxclass.MakeOBBFlow(newMesh, this.gameObject);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Mesh newMesh = CombineAllMeshes(parentObjectToCombine);
            obbBoxclass.MakeOBBFlow(newMesh, this.gameObject);
        }
    }
    /// <summary>
    /// 지정된 부모 오브젝트의 모든 자식 메시를 하나로 합칩니다.
    /// </summary>
    /// <param name="parent">메시를 합칠 대상 부모 오브젝트.</param>
    public Mesh CombineAllMeshes(GameObject parent)
    {
        // 1. 합칠 MeshFilter 컴포넌트들을 수집합니다.
        MeshFilter[] meshFilters = parent.GetComponentsInChildren<MeshFilter>();
        Debug.Log($"총 {meshFilters.Length}개의 메시를 합치려고 합니다.");

        // 데이터를 저장할 리스트를 초기화합니다.
        List<Vector3> newVertices = new List<Vector3>();
        List<int> newTriangles = new List<int>();
        List<Vector3> newNormals = new List<Vector3>();
        List<Vector2> newUVs = new List<Vector2>();

        // 현재 정점 인덱스 오프셋을 추적합니다.
        int vertexOffset = 0;

        // 2. 각 원본 메시의 데이터를 새로운 메시 데이터에 합칩니다.
        foreach (MeshFilter mf in meshFilters)
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
        combinedMesh.name = parent.name + "_CombinedMesh";

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

    /// <summary>
    /// 메쉬 합치기
    /// </summary>
    Mesh CombineMeshs(List<MeshFilter> filters)
    {
        // 현재 오브젝트의 MeshFilter와 MeshRenderer
        MeshFilter currentMeshFilter = GetComponent<MeshFilter>();
        MeshRenderer currentMeshRenderer = GetComponent<MeshRenderer>();

        // 머티리얼별로 CombineInstance를 저장할 딕셔너리
        Dictionary<Material, List<CombineInstance>> materialToCombineInstances = new Dictionary<Material, List<CombineInstance>>();

        // 원래 부모 오브젝트의 위치와 회전을 저장 (메시 합치기 전에 0으로 설정했다가 복원하기 위함)
        // 이렇게 하면 합쳐진 메시가 원래의 월드 위치에 유지됩니다.
        Vector3 originalPosition = transform.position;
        Quaternion originalRotation = transform.rotation;
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;

        // 모든 MeshFilter를 순회하며 머티리얼별로 분류
        foreach (MeshFilter mf in filters)
        {
            //메시가 없을 경우
            if (mf == null) continue;

            MeshRenderer mr = mf.GetComponent<MeshRenderer>();
            if (mr == null || mf.sharedMesh == null)
            {
                Debug.LogWarning($"오브젝트 '{mf.name}'에 MeshRenderer나 Mesh가 없습니다. 건너뜁니다.");
                continue;
            }

            // 각 MeshRenderer가 사용하는 모든 머티리얼을 처리
            for (int subMeshIndex = 0; subMeshIndex < mf.sharedMesh.subMeshCount; subMeshIndex++)
            {
                // 머티리얼이 서브메쉬 개수보다 적음
                if (subMeshIndex >= mr.sharedMaterials.Length)
                {
                    Debug.LogWarning($"오브젝트 '{mf.name}'의 서브메쉬 {subMeshIndex}에 해당하는 머티리얼이 없습니다. 건너뜁니다.");
                    continue;
                }

                Material material = mr.sharedMaterials[subMeshIndex];

                if (material == null)
                {
                    Debug.LogWarning($"오브젝트 '{mf.name}'의 서브메쉬 {subMeshIndex}에 할당된 머티리얼이 null입니다. 건너뜁니다.");
                    continue;
                }

                CombineInstance ci = new CombineInstance
                {
                    mesh = mf.sharedMesh,
                    subMeshIndex = subMeshIndex, // 해당 서브 메시만 가져옴
                    transform = mf.transform.localToWorldMatrix // 각 오브젝트의 월드 변환을 포함
                };

                // 해당 머티리얼에 대한 리스트가 없으면 새로 생성
                if (!materialToCombineInstances.ContainsKey(material))
                {
                    materialToCombineInstances.Add(material, new List<CombineInstance>());
                }
                materialToCombineInstances[material].Add(ci);
            }
        }

        // 최종 합쳐질 메시들을 위한 리스트와 머티리얼들을 위한 리스트
        List<Mesh> finalSubMeshes = new List<Mesh>();
        List<Material> finalMaterials = new List<Material>();

        // 딕셔너리를 순회하며 각 머티리얼별로 메시를 합치고 최종 리스트에 추가
        foreach (var entry in materialToCombineInstances)
        {
            Material material = entry.Key;
            List<CombineInstance> combinesForMaterial = entry.Value;

            if (combinesForMaterial.Count > 0)
            {
                Mesh combinedMeshForMaterial = new Mesh();
                // 해당 머티리얼에 속하는 모든 메시들을 하나의 서브 메시로 합침
                combinedMeshForMaterial.CombineMeshes(combinesForMaterial.ToArray(), true, true);

                finalSubMeshes.Add(combinedMeshForMaterial);
                finalMaterials.Add(material);
            }
        }

        // 모든 서브 메시를 하나의 최종 메시로 합침
        // 이때는 각 combinedMeshForMaterial이 별도의 서브 메시가 되어야 하므로 mergeSubMeshes = false
        // transform은 이미 각 메시의 월드 위치가 적용되었으므로 Matrix4x4.identity 사용
        CombineInstance[] finalCombineInstances = new CombineInstance[finalSubMeshes.Count];
        for (int i = 0; i < finalSubMeshes.Count; i++)
        {
            finalCombineInstances[i].mesh = finalSubMeshes[i];
            finalCombineInstances[i].transform = Matrix4x4.identity;
        }

        Mesh combinedFinalMesh = new Mesh();
        combinedFinalMesh.name = "Combined_Mesh";
        combinedFinalMesh.CombineMeshes(finalCombineInstances, false, false); // mergeSubMeshes=false, useMatrices=false

        // 현재 오브젝트의 MeshFilter와 MeshRenderer에 최종 결과 적용
        currentMeshFilter.mesh = combinedFinalMesh;
        currentMeshRenderer.materials = finalMaterials.ToArray();

        // 메시 합쳐진 후 MeshCollider도 업데이트
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        if (meshCollider != null)
        {
            meshCollider.sharedMesh = combinedFinalMesh;
        }

        // 원래 위치와 회전 복원
        transform.position = originalPosition;
        transform.rotation = originalRotation;

        Debug.Log($"총 {finalMaterials.Count}개의 서브메쉬로 {filters.Count}개의 원본 메시를 합쳤습니다.");

        return combinedFinalMesh;
    }
}
