using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Occlusion : MonoBehaviour
{
    CullingGroup group;

    void OcclusionTest()
    {
        group = new CullingGroup();
        //그룸이 사용하는 카메라 지정
        group.targetCamera = Camera.main;

        //구형 바운드 여러개
        BoundingSphere[] spheres = new BoundingSphere[1000];
        spheres[0] = new BoundingSphere(Vector3.zero, 1f);
        group.SetBoundingSpheres(spheres);
        group.SetBoundingSphereCount(1);

        //이 포인트에 컬링 그룹은 단일 구체의 가시성을 프레임마다 계산하기 시작합니다.
    }
    //컬링 그룹을 지우고 이 그룹이 사용하는 메모리를 모두 비우려면 스탠다드 .NET IDisposable 메커니즘을 사용하여 컬링 그룹을 폐기해야 합니다.
    void CullingDestroy()
    {
        group.Dispose();
        group = null;
    }
    
    //콜백을 통해 받기
    private void StateChangedMethod(CullingGroupEvent evt)
    {
        group.onStateChanged = StateChangedMethod;
        if (evt.hasBecomeVisible)
            Debug.LogFormat("Sphere {0} has become visible!", evt.index);
        if (evt.hasBecomeInvisible)
            Debug.LogFormat("Sphere {0} has become invisible!", evt.index);
    }
}
