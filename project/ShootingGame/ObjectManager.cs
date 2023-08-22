using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    const int bulletMaxCount = 100;//총알 최대 개수
    //프리팹 준비
    public GameObject playerBulletA_Prefab;

    //오브젝트 배열
    GameObject[] playerBulletA;


    //타겟 오브젝트
    GameObject[] targetPool;
    // Start is called before the first frame update
    void Awake()
    {
        playerBulletA = new GameObject[bulletMaxCount];

        Generate();//생성
    }

    //오브젝트 생성
    void Generate()
    {
        //총알
        for (int i = 0; i < playerBulletA.Length; i++)
        {
            playerBulletA[i] = Instantiate(playerBulletA_Prefab);//프리팹 필요
            playerBulletA[i].SetActive(false);//처음엔 비활성화
        }

    }

    //오브젝트 생성
    public GameObject MakeGameObject(string type)
    {
        switch (type)
        {
            case "BulletPlayerA":
                targetPool = playerBulletA;
                break;
        }
        for(int i = 0; i < targetPool.Length; i++)
        {
            //활성화후 넘김
            if (!targetPool[i].activeSelf)
            {
                targetPool[i].SetActive(true);
                return targetPool[i];
            }
        }
        return null;//없을시
    }
    //지정한 오브젝트 풀 가져오기
    public GameObject[] GetObject(string type)
    {
        switch (type)
        {
            case "BulletPlayerA":
                targetPool = playerBulletA;
                break;
        }
        return targetPool;
    }
}
