using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherComponent : MonoBehaviour
{
    private static OtherComponent instance;//오직 변수 1개, 외부에서 수정X
    public static OtherComponent Instance{
        get
        {
            if (instance == null)
            {
                var obj = FindObjectOfType<OtherComponent>();//또다른 오브젝트 있는지 검사
                if (obj != null)//객체 존재
                {
                    instance = obj;
                }else//없어서 새로 만들어야함
                {
                    var newObj = new GameObject().AddComponent<OtherComponent>();
                    instance = newObj;
                }
            }
            return instance;
        }
    }

    //게임에 단 1개 생성
    private void Awake()
    {
        var objs = FindObjectOfType<OtherComponent>();//오브젝트 개수
        if(objs != null)//이미 있다면
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(instance);//씬 변경에도 그대로
    }
    
}
