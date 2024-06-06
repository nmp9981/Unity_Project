using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    ObjectFulling objectfulling;
    [SerializeField]
    Transform startDragPosition;
    void Awake()
    {
        objectfulling = GameObject.Find("ObjectManager").GetComponent<ObjectFulling>(); 
    }

    void Update()
    { 
        if (Input.GetKeyDown(KeyCode.X))
        {
            StartCoroutine(ShotDrag());
        }
        
    }
    IEnumerator ShotDrag()
    {
        for (int i = 0; i < 2; i++)
        {
            GameObject gm = objectfulling.MakeObj(2);
            gm.transform.position = startDragPosition.transform.position;//캐릭터 위치에서 날리기 시작
            yield return new WaitForSeconds(0.2f);
        }
    }
}
