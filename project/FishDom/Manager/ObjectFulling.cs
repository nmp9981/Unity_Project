using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectFulling : MonoBehaviour
{
    //프리팹 준비
    const int fishKinds = 31;
    const int mobMaxCounts = 25;
    public GameObject[] fishPrefabs;
   
    //오브젝트 배열
    GameObject[][] fishes;
    GameObject[] targetPool;

    private void Awake()
    { 
        fishes = new GameObject[fishKinds][]{
            new GameObject[mobMaxCounts],
            new GameObject[mobMaxCounts],
            new GameObject[mobMaxCounts],
            new GameObject[mobMaxCounts],
            new GameObject[mobMaxCounts],
            new GameObject[mobMaxCounts],
            new GameObject[mobMaxCounts],
            new GameObject[mobMaxCounts],
            new GameObject[mobMaxCounts],
            new GameObject[mobMaxCounts],
            new GameObject[mobMaxCounts],
            new GameObject[mobMaxCounts],
            new GameObject[mobMaxCounts],
            new GameObject[mobMaxCounts],
            new GameObject[mobMaxCounts],
            new GameObject[mobMaxCounts],
            new GameObject[mobMaxCounts],
            new GameObject[mobMaxCounts],
            new GameObject[mobMaxCounts],
            new GameObject[mobMaxCounts],
            new GameObject[mobMaxCounts],
            new GameObject[mobMaxCounts],
            new GameObject[mobMaxCounts],
            new GameObject[mobMaxCounts],
            new GameObject[mobMaxCounts],
            new GameObject[mobMaxCounts],
            new GameObject[mobMaxCounts],
            new GameObject[mobMaxCounts],
            new GameObject[mobMaxCounts],
            new GameObject[mobMaxCounts],
            new GameObject[mobMaxCounts],
        };
        Generate();
    }
    void Generate()
    {
        //물고기
        for(int i = 0; i < fishKinds; i++)
        {
            for(int j = 0; j < mobMaxCounts; j++)
            {
                fishes[i][j] = Instantiate(fishPrefabs[i]);
                fishes[i][j].SetActive(false);
            }
        }
    }
   
    public GameObject MakeObj(int stage)
    {
        targetPool = fishes[stage];
        
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
    public GameObject[] GetPool(int stage)//지정한 오브젝트 풀 가져오기
    {
        targetPool = fishes[stage];
        return targetPool;
    }
    //오브젝트들 비활성화
    public void OffObj()
    {
        for (int i = 0; i < fishKinds; i++)
        {
            for (int j = 0; j < mobMaxCounts; j++)
            {
                if (fishes[i][j].activeSelf) fishes[i][j].SetActive(false);
            }
        }
    }
}
