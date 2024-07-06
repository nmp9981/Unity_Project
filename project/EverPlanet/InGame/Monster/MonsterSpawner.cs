using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public int[] mobCount;
    ObjectFulling objectfulling;

    [SerializeField] List<Transform> spawnPositionList;

    float curTime = 6.5f;
    float coolTime = 7.0f;

    static public List<GameObject> spawnMonster = new List<GameObject>();
    void Awake()
    {
        objectfulling = GameObject.Find("ObjectManager").GetComponent<ObjectFulling>();
        mobCount = new int[2] { 0, 0 };
        curTime = 7.0f;
        GameManager.Instance.MaxMonsterCount = 31;
        InitSpawn();
    }
    private void Update()
    {
        if (curTime >= coolTime)
        {
            StartCoroutine(MonsterSpawn());
            curTime = 0;
        }
        TimeFlow();
    }
    void TimeFlow()
    {
        curTime += Time.deltaTime;
    }
    IEnumerator MonsterSpawn()
    {
        int spawnPositionNumber = Random.Range(0, 7);
        int mapNum = MapNumber(spawnPositionNumber);
        int spawnCount = Random.Range(4, 8);//스폰 마릿수
        for (int i = 0; i < spawnCount; i++)
        {
            if (mobCount[mapNum] >= GameManager.Instance.MaxMonsterCount) yield break;
            int monsterNumber = MonsterPosition(spawnPositionNumber, mapNum);//스폰할 몬스터
            GameObject gm = objectfulling.MakeObj(monsterNumber);
            gm.transform.position = spawnPositionList[spawnPositionNumber].position + new Vector3(Random.Range(-7, 7), 1f, Random.Range(-7, 7));
            spawnMonster.Add(gm);
        }
    }
    int MapNumber(int spawnPositionNumber) {
        if (spawnPositionNumber <= 3)
        {
            return 0;
        }
        return 1;
    }

    int MonsterPosition(int spawnPositionNumber, int mapNum)
    {
        mobCount[mapNum] += 1;
        if (spawnPositionNumber <= 1)
        {
            return 4;
        } else if (spawnPositionNumber >= 2 && spawnPositionNumber <= 3)
        {
            return Random.Range(4, 6);
        }
        else if (spawnPositionNumber == 4)
        {
            return 6;
        }
        else if (spawnPositionNumber >= 5 && spawnPositionNumber <= 6)
        {
            return Random.Range(6, 8);
        }
        return 0;
    }
    //초기 스폰
    void InitSpawn()
    {
        for(int spawnIdx = 1; spawnIdx < 7; spawnIdx++)//모든 스폰 지점
        {
            int mapNum = MapNumber(spawnIdx);
            int spawnCount = Random.Range(4, 8);//스폰 마릿수
            for (int i = 0; i < spawnCount; i++)
            {
                int monsterNumber = MonsterPosition(spawnIdx, mapNum);//스폰할 몬스터
                GameObject gm = objectfulling.MakeObj(monsterNumber);
                gm.transform.position = spawnPositionList[spawnIdx].position + new Vector3(Random.Range(-7, 7), 1f, Random.Range(-7, 7));
                spawnMonster.Add(gm);
            }
        }
    }
}
