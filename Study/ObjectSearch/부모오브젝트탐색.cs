using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject curObj = this.gameObject;
        GameObject root = this.transform.root.gameObject;
        while(curObj != root)
        {
            curObj = curObj.transform.parent.gameObject;
            Debug.Log(curObj.name);//오브젝트 명 출력
            Debug.Log(curObj.GetComponent<Transform>().position.z);//z좌표 출력
        }   
    }
}
