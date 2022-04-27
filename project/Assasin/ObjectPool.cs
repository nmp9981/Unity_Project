using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;//싱글톤 패턴으로 구현, 어디서나 꺼내 사용해야한다
    [SerializeField] 
    private GameObject poolingObjectPrefab;

    Queue<Throw> ObjectQueue = new Queue<Throw>();//객체를 담는 큐 선언

    private void Awake()
    {
        Instance = this;//오브젝트 풀 인스턴스에 자신을 할당
        Initialize(1);//오브젝트 풀 초기화
    }
    private void Initialize(int initcnt)
    {
        for(int i = 0; i < initcnt; i++)
        {
            ObjectQueue.Enqueue(CreatNewObject());//큐에 표창을 넣어준다
        }
    }
    //새 표창객체 생성
    private Throw CreatNewObject()
    {
        var newObj = Instantiate(poolingObjectPrefab).GetComponent<Throw>();//프리팹으로부터 새 오브젝트 생성
        newObj.gameObject.SetActive(true);//처음에는 비활성화
        newObj.transform.SetParent(transform);
        return newObj;
    }
    public static Throw GetObject()//요청시 게임오브젝트를 꺼내줌
    {
        if (Instance.ObjectQueue.Count > 0)
        {
            var obj = Instance.ObjectQueue.Dequeue();
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);
            return obj;
        }
        else//큐에 표창이 없으면 새로 생성
        {
            var obj = Instance.CreatNewObject();
            obj.gameObject.SetActive(true);
            obj.transform.SetParent(null);
            return obj;
        }
    }
    public static void ReturnObject(Throw obj)//오브젝트 반환
    {
        obj.gameObject.SetActive(false);//비활성화
        obj.transform.SetParent(Instance.transform);
        Instance.ObjectQueue.Enqueue(obj);
    }
}
