using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkidMarkDestroy : MonoBehaviour
{
    //오브젝트 파괴 시간
    private float destroyTime = 0;

    void Update() {
        //스키드 마크 오브젝트 파괴
        DestroySkidMark();
    }
    /// <summary>
    /// 기능 : 스키드 마크 오브젝트 파괴
    /// 1) 생성 후 2초뒤 파괴
    /// </summary>
    void DestroySkidMark()
    {
        if (destroyTime >= 3f)
        {
            Destroy(gameObject);
        }
        else
        {
            destroyTime += Time.deltaTime;
        }
    }
}
