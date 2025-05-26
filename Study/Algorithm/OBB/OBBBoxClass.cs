using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;

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

public class OBBBoxClass
{
    public MeshFilter targetMeshFilter; // OBB를 생성할 메시 필터
    public bool drawOBBGizmos = true; // OBB를 기즈모로 그릴지 여부

    private Bounds _obbBounds; // OBB의 로컬 경계 (AABB 형태)
    private Quaternion _obbRotation; // OBB의 로컬 회전
    private Vector3 _obbCenter; // OBB의 로컬 중심

    /// <summary>
    /// OBB박스 만드는 함수
    /// 1) 정점 데이터 추출
    /// 2) 데이터 중심 계산
    /// 3) 공분산 행렬 계산
    /// 4) 고윳값 분해하여 OBB세가지 주축을 얻는다.
    /// 5) OBB박스 회전량, 크기 결정
    /// </summary>
    /// <param name="mesh">메시</param>
    public void MakeOBBFlow(Mesh mesh, GameObject parentObj)
    {
        Vector3[] meshVertices = GetMeshVerticesArray(mesh);
        Vector3 meshCenter = CalculateDataCenter(meshVertices);
        Matrix4x4 meshCovarianceMatrix = CalculateCovarianceMatrix(meshVertices, meshCenter);
        Quaternion obbRotation = CalEigenvalueDecomposition(meshCovarianceMatrix);
        OBB createOBBBox = MakeOBB(obbRotation,mesh, meshVertices, meshCenter);
        DrawOBBBOX(createOBBBox,parentObj);
        //CalculateOBB(mesh);
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
    Vector3 CalculateDataCenter(Vector3[] vertices)
    {
        Vector3 sum = Vector3.zero;
        foreach (Vector3 v in vertices)
        {
            sum += v;
        }
        Vector3 centroid = sum / vertices.Length;
        return centroid;
    }
    /// <summary>
    /// 공분산 행렬 계산
    /// </summary>
    /// <param name="vertices">메시 리스트</param>
    /// <param name="centerPos">메시 중심 좌표</param>
    /// <returns></returns>
    Matrix4x4 CalculateCovarianceMatrix(Vector3[] vertices,Vector3 centerPos)
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
        covarianceMatrix.m00 = xx; covarianceMatrix.m01 = xy; covarianceMatrix.m02 = xz;
        covarianceMatrix.m10 = xy; covarianceMatrix.m11 = yy; covarianceMatrix.m12 = yz;
        covarianceMatrix.m20 = xz; covarianceMatrix.m21 = yz; covarianceMatrix.m22 = zz;
       
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
        //3x3 행렬 생성
        Matrix<double> cov = Matrix<double>.Build.DenseOfArray(new double[,]
        {
             {meshCovMatrix.m00, meshCovMatrix.m01, meshCovMatrix.m02},
             {meshCovMatrix.m10, meshCovMatrix.m11, meshCovMatrix.m12},
             {meshCovMatrix.m20, meshCovMatrix.m21, meshCovMatrix.m22}
        });

        //고유 벡터 구하기
        EigenVectorDecomposition eigenVectorDecomposition = new EigenVectorDecomposition();
        var eigenvectors = eigenVectorDecomposition.SolveEigenVectorFlow(meshCovMatrix).eigenvectors;

        //EigenvalueDecomposition<double> eigen = cov.EigenvalueDecomposition();
        //Vector<double> eigenvalues = eigen.EigenValues.Real();//고윳값
        //Matrix<double> eigenvectors = eigen.EigenVectors;//고유벡터

        //고유 벡터를 각 x,y,z축의 주축으로 사용
        Vector3 principalAxisX = new Vector3((float)eigenvectors[0 ,0], (float)eigenvectors[1, 0], (float)eigenvectors[2, 0]).normalized;
        Vector3 principalAxisY = new Vector3((float)eigenvectors[0, 1], (float)eigenvectors[1, 1], (float)eigenvectors[2, 1]).normalized;
        Vector3 principalAxisZ = Vector3.Cross(principalAxisX, principalAxisY).normalized; // 세 번째 축은 직교성을 위해 외적 사용

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
    OBB MakeOBB(Quaternion obbRotation,Mesh mesh, Vector3[] vertices, Vector3 centerPos)
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
        Vector3 obbLocalCenter = centerPos + (obbRotation * ((minBounds + maxBounds) * 0.5f));

        return new OBB
        {
            Center = obbLocalCenter,
            Rotation = obbRotation,
            Size = obbSize
        };
    }
    
    void DrawOBBBOX(OBB obb, GameObject parentObj)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //Local로 생성
        cube.transform.rotation = obb.Rotation;
        cube.transform.localScale = obb.Size;
        cube.transform.position = obb.Center + parentObj.transform.position;
    }

    /// <summary>
    /// 주어진 메시의 로컬 공간에서 OBB를 계산합니다.
    /// </summary>
    /// <param name="mesh">OBB를 계산할 메시</param>
    public void CalculateOBB(Mesh mesh)
    {
        if (mesh == null || mesh.vertexCount == 0)
        {
            _obbBounds = new Bounds(Vector3.zero, Vector3.zero);
            _obbRotation = Quaternion.identity;
            _obbCenter = Vector3.zero;
            return;
        }

        //모든 메시
        Vector3[] vertices = mesh.vertices;

        // 1. 데이터의 중심(평균) 계산
        Vector3 sum = Vector3.zero;
        foreach (Vector3 v in vertices)
        {
            sum += v;
        }
        Vector3 centroid = sum / vertices.Length;

        // 2. 공분산 행렬 계산
        // (x-x_avg)^2, (y-y_avg)^2, (z-z_avg)^2, (x-x_avg)(y-y_avg), ...
        float xx = 0, yy = 0, zz = 0, xy = 0, xz = 0, yz = 0;
        foreach (Vector3 v in vertices)
        {
            Vector3 p = v - centroid; // 중심으로부터의 벡터
            xx += p.x * p.x;
            yy += p.y * p.y;
            zz += p.z * p.z;
            xy += p.x * p.y;
            xz += p.x * p.z;
            yz += p.y * p.z;
        }

        // 공분산 행렬 (대칭 행렬)
        Matrix4x4 covarianceMatrix = Matrix4x4.identity;
        covarianceMatrix.m00 = xx; covarianceMatrix.m01 = xy; covarianceMatrix.m02 = xz;
        covarianceMatrix.m10 = xy; covarianceMatrix.m11 = yy; covarianceMatrix.m12 = yz;
        covarianceMatrix.m20 = xz; covarianceMatrix.m21 = yz; covarianceMatrix.m22 = zz;

        // 3. 고유 벡터와 고유 값 계산 (PCA)
        // 여기서는 직접 계산하기 어렵기 때문에, Unity의 Matrix4x4를 이용하여
        // 특수 목적의 PCA 라이브러리나 직접 고유값 분해 알고리즘을 구현해야 합니다.
        // 이 예제에서는 Unity의 내부 행렬 분해에 의존하거나,
        // 보다 간단한 근사를 사용합니다 (예: 메시의 AABB를 기반으로 회전만 최적화).
        // 완전한 PCA 구현은 훨씬 더 복잡합니다.

        // === 단순화된 접근 (AABB 기반 OBB 회전 추정) ===
        // 실제 PCA는 고유값 분해를 통해 가장 적절한 세 개의 직교 축을 찾아야 합니다.
        // Unity에는 Eigen decomposition을 직접 제공하는 함수가 없습니다.
        // 따라서, 이 부분은 외부 라이브러리나 복잡한 수학적 구현이 필요합니다.
        // 여기서는 임시 방편으로, 메시의 AABB를 얻고,
        // 그 AABB를 둘러싸는 최소 회전된 상자를 찾으려는 시도를 합니다.
        // 이는 정확한 PCA OBB는 아닙니다.

        // C#에서 직접 고유값 분해를 구현하는 것은 복잡하므로,
        // 여기서는 가장 간단한 형태로, 메시의 AABB와 그것을 둘러싸는 OBB의 개념을 혼합합니다.
        // 즉, 메시의 정점들을 OBB의 회전 방향으로 먼저 회전시킨 후 AABB를 계산하는 방식입니다.
        // 이는 완벽한 PCA OBB는 아니지만, 메시의 방향성을 어느 정도 반영할 수 있습니다.

        // 실제 PCA OBB를 구현하려면 다음 단계가 필요합니다.
        // 1. 공분산 행렬의 고유값 분해 (Eigenvalue Decomposition)를 수행하여
        //    세 개의 고유값(eigenvalues)과 세 개의 고유 벡터(eigenvectors)를 얻습니다.
        // 2. 가장 큰 고유값에 해당하는 고유 벡터가 OBB의 주축이 됩니다.
        // 3. 나머지 두 고유 벡터도 OBB의 나머지 축이 됩니다.
        // 4. 이 세 개의 고유 벡터로 OBB의 회전(Quaternion)을 구성합니다.

        // 이 예제는 간단한 대안으로, 메시의 AABB를 기본으로 하되,
        // 임시로 회전을 적용하여 OBB를 추정합니다.
        // *******************************************************************
        // 완전한 PCA 기반 OBB 구현은 아래와 같은 외부 라이브러리를 사용하거나
        // 직접 행렬 고유값 분해를 구현해야 합니다.
        // 예: Math.NET Numerics (Unity에서는 외부 라이브러리 추가가 필요)
        // 또는, 간단한 경우 정점들의 min/max를 여러 방향에서 투영하여 근사 OBB를 찾을 수도 있습니다.
        // *******************************************************************

        // 임시로 메시의 기본 AABB를 가져와서, 이를 OBB의 초기 추정치로 사용합니다.
        Bounds aabb = mesh.bounds;
        _obbCenter = aabb.center;
        _obbRotation = Quaternion.identity; // 초기에는 회전 없음
        _obbBounds = aabb; // 초기 OBB는 AABB와 동일

        // 더 나은 OBB를 찾기 위한 간단한 반복 최적화 (선택 사항, 성능에 영향)
        // 이 부분은 완전한 PCA가 아님. 실제 OBB는 수학적으로 한번에 계산됨.
        // 이 코드는 AABB를 기반으로 한 단순 추정치에 가깝습니다.
        Quaternion bestRotation = Quaternion.identity;
        Vector3 bestSize = Vector3.zero;
        float minVolume = float.MaxValue;

        // 여러 각도에서 AABB를 계산하여 최소 볼륨을 갖는 회전을 찾음
        // (이것은 OBB의 정확한 계산이 아니라, AABB의 한계점을 극복하기 위한 근사치)
        for (int angleX = 0; angleX < 180; angleX += 30)
        {
            for (int angleY = 0; angleY < 180; angleY += 30)
            {
                for (int angleZ = 0; angleZ < 180; angleZ += 30)
                {
                    Quaternion currentRotation = Quaternion.Euler(angleX, angleY, angleZ);
                    Vector3 minBounds = Vector3.one * float.MaxValue;
                    Vector3 maxBounds = Vector3.one * float.MinValue;

                    foreach (Vector3 vert in vertices)
                    {
                        Vector3 rotatedVert = Quaternion.Inverse(currentRotation) * (vert - centroid); // 정점을 임시 회전된 공간으로 변환
                        minBounds = Vector3.Min(minBounds, rotatedVert);
                        maxBounds = Vector3.Max(maxBounds, rotatedVert);
                    }

                    Vector3 currentSize = maxBounds - minBounds;
                    float currentVolume = currentSize.x * currentSize.y * currentSize.z;

                    if (currentVolume < minVolume)
                    {
                        minVolume = currentVolume;
                        bestRotation = currentRotation;
                        bestSize = currentSize;
                        _obbCenter = centroid + (currentRotation * (minBounds + maxBounds) * 0.5f); // 중심 업데이트
                    }
                }
            }
        }

        _obbRotation = bestRotation;
        _obbBounds = new Bounds(Vector3.zero, bestSize); // OBB는 로컬 공간의 AABB + 회전으로 표현
                                                         // _obbCenter는 이미 위에서 계산되었음

        Debug.Log($"OBB 계산 완료: Center={_obbCenter}, Rotation={_obbRotation.eulerAngles}, Size={_obbBounds.size}");
    }

    // OBB 정보를 외부에서 가져갈 수 있도록 public getter 추가
    public Bounds GetOBBBounds()
    {
        return _obbBounds;
    }

    public Quaternion GetOBBRotation()
    {
        return _obbRotation;
    }

    public Vector3 GetOBBCenter()
    {
        return _obbCenter;
    }

    /// <summary>
    /// OBB의 월드 공간 중심을 반환합니다.
    /// </summary>
    public Vector3 GetWorldOBBCenter()
    {
        if (targetMeshFilter != null)
        {
            // 메시 필터의 월드 변환과 OBB의 로컬 변환을 결합
            return targetMeshFilter.transform.TransformPoint(_obbCenter);
        }
        return Vector3.zero;
    }

    /// <summary>
    /// OBB의 월드 공간 회전을 반환합니다.
    /// </summary>
    public Quaternion GetWorldOBBRotation()
    {
        if (targetMeshFilter != null)
        {
            // 메시 필터의 월드 회전과 OBB의 로컬 회전을 결합
            return targetMeshFilter.transform.rotation * _obbRotation;
        }
        return Quaternion.identity;
    }
}
