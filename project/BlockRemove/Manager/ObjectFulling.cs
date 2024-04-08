using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPulling : MonoBehaviour
{
    //프리팹 준비
    const int blockMaxCount = 100;
    const int blockKinds = 6;
    public GameObject[] blockPrefabs;

    //오브젝트 배열
    GameObject[][] blocks;
    GameObject[] targetPool;

    void Awake()
    {
        blocks = new GameObject[blockKinds][]
        {
             new GameObject[blockMaxCount],
             new GameObject[blockMaxCount],
             new GameObject[blockMaxCount],
             new GameObject[blockMaxCount],
             new GameObject[blockMaxCount],
             new GameObject[blockMaxCount]
        };
        Generate();
    }
    void Generate()
    {
        //블록
        for (int i = 0; i < blockKinds; i++)
        {
            for (int j = 0; j < blockMaxCount; j++)
            {
                blocks[i][j] = Instantiate(blockPrefabs[i]);
                blocks[i][j].SetActive(false);
            }
        }
    }
    //오브젝트 생성
    public GameObject MakeObj(int num)
    {
        targetPool = blocks[num];

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
        targetPool = blocks[num];
        return targetPool;
    }
    //오브젝트들 비활성화
    public void OffObj()
    {
        for (int i = 0; i < blockKinds; i++)
        {
            for (int j = 0; j < blockMaxCount; j++)
            {
                if (blocks[i][j].activeSelf) blocks[i][j].SetActive(false);
            }
        }
    }
    //투명 블록 비활성화
    public void BlockTransparentOffObj()
    {
        for (int j = 0; j < blockMaxCount; j++)
        {
            if (blocks[5][j].activeSelf) blocks[5][j].SetActive(false);
        }
    }
}
