using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public int[] mobCount;
    public int[] mobMapMaxCount;
    ObjectFulling objectfulling;

    [SerializeField] List<Transform> spawnPositionList;

    float curTime = 6.5f;
    float coolTime = 7.0f;

    static public List<GameObject> spawnMonster = new List<GameObject>();
    void Awake()
    {
        objectfulling = GameObject.Find("ObjectManager").GetComponent<ObjectFulling>();
        mobCount = new int[38] { 0, 0,0,0,0,0,
        0,0,0,0,0,0,
        0,0,0,0,0,0,
        0,0,0,0,0,0,
        0,0,0,0,0,0,
        0,0,0,0,0,0,0,0};
        mobMapMaxCount = new int[6] {30,31, 41,77,33,37};
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
        int spawnPositionNumber = Random.Range(0, 38);
        int mapNum = MapNumber(spawnPositionNumber);
        int spawnCount = Random.Range(4, 8);//스폰 마릿수
        for (int i = 0; i < spawnCount; i++)
        {
            if (mobCount[mapNum] >= mobMapMaxCount[mapNum]) yield break;//최대 스폰 몬스터 수 추가
            int monsterNumber = MonsterPosition(spawnPositionNumber, mapNum);//스폰할 몬스터
            GameObject gm = objectfulling.MakeObj(monsterNumber);
            gm.transform.position = spawnPositionList[spawnPositionNumber].position + new Vector3(Random.Range(-7, 7), 1f, Random.Range(-7, 7));
            spawnMonster.Add(gm);
        }
    }
    int MapNumber(int spawnPositionNumber) {
        if (spawnPositionNumber <= 3) return 0;
        else if (spawnPositionNumber >= 4 && spawnPositionNumber <= 6) return 1;
        else if (spawnPositionNumber >= 7 && spawnPositionNumber <= 13) return 2;
        else if (spawnPositionNumber >= 14 && spawnPositionNumber <= 27) return 3;
        else if (spawnPositionNumber >= 28 && spawnPositionNumber <= 31) return 4;
        return 5;
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
        else if (spawnPositionNumber >= 7 && spawnPositionNumber <= 8)
        {
            return Random.Range(7,9);
        }
        else if (spawnPositionNumber == 9)
        {
            return Random.Range(8, 10);
        }
        else if (spawnPositionNumber == 10)
        {
            return 10;
        }
        else if (spawnPositionNumber >= 11 && spawnPositionNumber <= 12)
        {
            return 23;
        }
        else if (spawnPositionNumber == 13)
        {
            return Random.Range(10,12);
        }
        else if (spawnPositionNumber ==14)
        {
            return 12;
        }
        else if (spawnPositionNumber >= 15 && spawnPositionNumber <= 18)
        {
            return Random.Range(12, 15);
        }
        else if (spawnPositionNumber >= 17 && spawnPositionNumber <= 20)
        {
            return Random.Range(13, 16);
        }
        else if (spawnPositionNumber >= 21 && spawnPositionNumber <= 23)
        {
            return Random.Range(15, 17);
        }
        else if (spawnPositionNumber >= 24 && spawnPositionNumber <= 27)
        {
            return Random.Range(16, 18);
        }
        else if (spawnPositionNumber >= 28 && spawnPositionNumber <= 30)
        {
            return Random.Range(18,20);
        }
        else if (spawnPositionNumber >= 32 && spawnPositionNumber <= 33)
        {
            return Random.Range(19, 21);
        }
        else if (spawnPositionNumber >= 34 && spawnPositionNumber <= 35)
        {
            return Random.Range(20, 22);
        }
        else if (spawnPositionNumber == 36)
        {
            return 22;
        }
        else if (spawnPositionNumber == 37)
        {
            return 25;
        }
        return 0;
    }
    //초기 스폰
    void InitSpawn()
    {
        for(int spawnIdx = 1; spawnIdx < 38; spawnIdx++)//모든 스폰 지점
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
