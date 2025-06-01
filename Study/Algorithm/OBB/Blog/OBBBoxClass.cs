using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// OBB 정보를 저장할 구조체 (로컬 공간 기준)
public struct OBB
{
    public Vector3 Center;   // OBB의 로컬 중심 좌표
    public Quaternion Rotation; // OBB의 로컬 회전
    public Vector3 Size;     // OBB의 로컬 크기 (extents * 2)

    public Bounds GetBounds()
    {
        return new Bounds(Vector3.zero, Size); // OBB는 회전을 통해 표현되므로 AABB는 (0,0,0) 중심
    }
}

namespace MakeOBB
{
    public class OBBBoxClass
    {
        MeshCombineClass meshCombineClass = new MeshCombineClass();

        public MeshFilter targetMeshFilter; // OBB를 생성할 메시 필터
        public bool drawOBBGizmos = true; // OBB를 기즈모로 그릴지 여부

        private Bounds _obbBounds; // OBB의 로컬 경계 (AABB 형태)
        private Quaternion _obbRotation; // OBB의 로컬 회전
        private Vector3 _obbCenter; // OBB의 로컬 중심

        /// <summary>
        /// 합친 메시로 OBB박스 생성
        /// </summary>
        public OBB MakeOBBAboutCombineMeshs(List<MeshFilter> meshFilterList)
        {
            Mesh newMesh = meshCombineClass.CombineAllMeshes(meshFilterList);
            OBB obb = MakeOBBFlow(newMesh);
            return obb;
        }

        /// <summary>
        /// OBB박스 만드는 함수
        /// 1) 정점 데이터 추출
        /// 2) 데이터 중심 계산
        /// 3) 공분산 행렬 계산
        /// 4) 고윳값 분해하여 OBB세가지 주축을 얻는다.
        /// 5) OBB박스 회전량, 크기 결정
        /// </summary>
        /// <param name="mesh">메시</param>
        public OBB MakeOBBFlow(Mesh mesh)
        {
            Vector3[] meshVertices = GetMeshVerticesArray(mesh);
            Vector3 meshCenter = CalculateDataCenter(meshVertices);
            Matrix4x4 meshCovarianceMatrix = CalculateCovarianceMatrix(meshVertices, meshCenter);
            Quaternion obbRotation = CalEigenvalueDecomposition(meshCovarianceMatrix);
            OBB createOBBBox = MakeOBB(obbRotation, mesh, meshVertices, meshCenter);

            DrawOBBBOX(createOBBBox, meshCenter);//디버그용
            return createOBBBox;
        }

        /// <summary>
        /// 해당 메시의 정점 배열 가져오기
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        Vector3[] GetMeshVerticesArray(Mesh mesh)
        {
            if (mesh == null || mesh.vertexCount == 0)
            {
                _obbBounds = new Bounds(Vector3.zero, Vector3.zero);
                _obbRotation = Quaternion.identity;
                _obbCenter = Vector3.zero;
                return null;
            }
            return mesh.vertices;
        }

        /// <summary>
        /// 데이터 중심 계산
        /// </summary>
        /// <param name="vertices"></param>
        /// <returns></returns>
        Vector3 CalculateDataCenter(Vector3[] vertices, float epcilon = 0.04f)
        {
            Vector3 sum = Vector3.zero;
            float vecCount = 0;
            for (int idx = 0; idx < vertices.Length; idx++)
            {
                if (idx <0)
                {
                    if ((vertices[idx - 1] - vertices[idx]).sqrMagnitude < epcilon)
                        continue;
                }

                sum += vertices[idx];
                vecCount += 1;
            }
            Vector3 centroid = sum / vecCount;
            return centroid;
        }
        /// <summary>
        /// 공분산 행렬 계산
        /// </summary>
        /// <param name="vertices">메시 리스트</param>
        /// <param name="centerPos">메시 중심 좌표</param>
        /// <returns></returns>
        Matrix4x4 CalculateCovarianceMatrix(Vector3[] vertices, Vector3 centerPos)
        {
            //공분산 행렬
            // [ xx xy xz ]
            // [ xy yy yz ]
            // [ xz yz zz ]
            float xx = 0, yy = 0, zz = 0, xy = 0, xz = 0, yz = 0;
            foreach (Vector3 v in vertices)
            {
                // (x-x_avg)^2, (y-y_avg)^2, (z-z_avg)^2, (x-x_avg)(y-y_avg),...
                Vector3 p = v - centerPos; // 중심으로부터의 벡터
                xx += p.x * p.x;
                yy += p.y * p.y;
                zz += p.z * p.z;
                xy += p.x * p.y;
                xz += p.x * p.z;
                yz += p.y * p.z;
            }

            // 공분산 행렬 (대칭 행렬)
            Matrix4x4 covarianceMatrix = Matrix4x4.identity;
            covarianceMatrix.m00 = xx / vertices.Length; covarianceMatrix.m01 = xy / vertices.Length; covarianceMatrix.m02 = xz / vertices.Length;
            covarianceMatrix.m10 = xy / vertices.Length; covarianceMatrix.m11 = yy / vertices.Length; covarianceMatrix.m12 = yz / vertices.Length;
            covarianceMatrix.m20 = xz / vertices.Length; covarianceMatrix.m21 = yz / vertices.Length; covarianceMatrix.m22 = zz / vertices.Length;

            return covarianceMatrix;
        }

        /// <summary>
        /// 고윳값 분해
        /// 공분산 행렬의 **고유값(Eigenvalues)**과 **고유 벡터(Eigenvectors)**를 계산
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="vertices"></param>
        /// <param name="centerPos"></param>
        Quaternion CalEigenvalueDecomposition(Matrix4x4 meshCovMatrix)
        {
            //고유 벡터 구하기
            EigenVectorDecomposition eigenVectorDecomposition = new EigenVectorDecomposition();
            var eigenvectors = eigenVectorDecomposition.SolveEigenVectorFlow(meshCovMatrix).eigenvectors;

            //고유 벡터를 각 x,y,z축의 주축으로 사용
            Vector3 principalAxisX = new Vector3((float) eigenvectors[0, 0], (float) eigenvectors[1, 0], (float) eigenvectors[2, 0]).normalized;
            Vector3 principalAxisY = new Vector3((float) eigenvectors[0, 1], (float) eigenvectors[1, 1], (float) eigenvectors[2, 1]).normalized;
            Vector3 principalAxisZ = new Vector3((float) eigenvectors[0, 2], (float) eigenvectors[1, 2], (float) eigenvectors[2, 2]).normalized;

            //오른손 좌표계 확인 및 조정
            if (Vector3.Dot(Vector3.Cross(principalAxisX, principalAxisY), principalAxisZ) < 0)
            {
                principalAxisZ = -principalAxisZ;
            }

            // 고유 벡터들로 OBB의 회전 쿼터니언 생성
            Quaternion obbRotation = Quaternion.LookRotation(principalAxisZ, principalAxisY);
            return obbRotation;
        }

        /// <summary>
        /// OBB 크기 및 중심 결정
        /// </summary>
        /// <param name="mesh">메시</param>
        /// <param name="vertices">메시 정점 배열</param>
        /// <param name="centerPos">메시 중심 좌표</param>
        /// <returns></returns>
        OBB MakeOBB(Quaternion obbRotation, Mesh mesh, Vector3[] vertices, Vector3 centerPos)
        {
            // 5. OBB 크기 및 중심 결정
            // OBB의 회전으로 정점들을 변환하여 AABB를 계산
            Vector3 minBounds = Vector3.one * float.MaxValue;
            Vector3 maxBounds = Vector3.one * float.MinValue;

            // OBB의 회전의 역변환을 사용하여 정점들을 OBB의 로컬 축 공간으로 변환
            Quaternion inverseObbRotation = Quaternion.Inverse(obbRotation);

            foreach (Vector3 v in vertices)
            {
                // 정점을 OBB의 로컬 축 공간으로 변환 (중심 기준)
                Vector3 transformedPoint = inverseObbRotation * (v - centerPos);

                minBounds = Vector3.Min(minBounds, transformedPoint);
                maxBounds = Vector3.Max(maxBounds, transformedPoint);
            }

            Vector3 obbSize = maxBounds - minBounds;
            // OBB의 로컬 중심은 변환된 공간에서의 중앙점을 다시 원래 로컬 공간으로 변환
            Vector3 obbLocalCenter = (obbRotation * ((minBounds + maxBounds) * 0.5f));

            return new OBB
            {
                Center = obbLocalCenter,
                Rotation = obbRotation,
                Size = obbSize
            };
        }

        public void DrawOBBBOX(OBB obb, Vector3 centerPos)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //Local로 생성
            cube.transform.position = obb.Center + centerPos;
            cube.transform.rotation = obb.Rotation;
            cube.transform.localScale = obb.Size;
        }
    }
}
