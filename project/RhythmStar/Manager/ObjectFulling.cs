using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFulling : MonoBehaviour
{//프리팹 준비
    const int blockMaxCount = 100;
    const int keyKinds = 3;
    public GameObject[] keyPrefabs;

    //오브젝트 배열
    GameObject[][] keynotes;
    GameObject[] targetPool;

    void Awake()
    {
        keynotes = new GameObject[keyKinds][]
        {
             new GameObject[blockMaxCount],
             new GameObject[blockMaxCount],
             new GameObject[blockMaxCount],
        };
        Generate();
    }
    void Generate()
    {
        //블록
        for (int i = 0; i < keyKinds; i++)
        {
            for (int j = 0; j < blockMaxCount; j++)
            {
                keynotes[i][j] = Instantiate(keyPrefabs[i]);
                keynotes[i][j].SetActive(false);
            }
        }
    }
    //오브젝트 생성
    public GameObject MakeObj(int num)
    {
        targetPool = keynotes[num];

        for (int i = 0; i < targetPool.Length; i++)
        {
            if (!targetPool[i].activeSelf)
            {
                targetPool[i].SetActive(true);//활성화 후 넘김
                return targetPool[i];
            }
        }
        return null;//없으면 빈 객체
    }
    //오브젝트 배열 가져오기
    public GameObject[] GetPool(int num)//지정한 오브젝트 풀 가져오기
    {
        targetPool = keynotes[num];
        return targetPool;
    }
    //오브젝트들 비활성화
    public void OffObj()
    {
        for (int i = 0; i < keyKinds; i++)
        {
            for (int j = 0; j < blockMaxCount; j++)
            {
                if (keynotes[i][j].activeSelf) keynotes[i][j].SetActive(false);
            }
        }
    }
}
