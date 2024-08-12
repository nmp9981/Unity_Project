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
    float coolTime = 7.5f;

    float bearCurTime = 6.5f;
    float bearCoolTime = 900f;
    float bossCurTime = 6.5f;
    float bossCoolTime = 900f;
    float finalBossCurTime = 6.5f;
    float finalBossCoolTime = 1800.0f;

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
        mobMapMaxCount = new int[7] {30,31,58,78,77,44,48};
        curTime = 7.0f;
        bearCurTime = 1.0f;
        bossCurTime = 1.0f;
        finalBossCurTime = 1.0f;
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
        if(bearCurTime >= bearCoolTime)
        {
            if (!objectfulling.IsActiveObj(23))
            {
                StartCoroutine(BearMonsterSpawn());
                bearCurTime = 0;
            }
        }
        if(bossCurTime >= bossCoolTime)
        {
            if (!objectfulling.IsActiveObj(24))
            {
                StartCoroutine(MusllstoneMonsterSpawn());
                bossCurTime = 0;
            }
        }
        if (finalBossCurTime >= finalBossCoolTime)
        {
            if (!objectfulling.IsActiveObj(25))
            {
                StartCoroutine(FinalBossMonsterSpawn());
                finalBossCurTime = 0;
            }
        }
        TimeFlow();
    }
    void TimeFlow()
    {
        curTime += Time.deltaTime;
        bearCurTime += Time.deltaTime;
        bossCurTime += Time.deltaTime;
        finalBossCurTime += Time.deltaTime;
    }
    //일반 몬스터 스폰
    IEnumerator MonsterSpawn()
    {
        int spawnCount = 0;
        for(int spawnPositionNumber = 0; spawnPositionNumber < 38; spawnPositionNumber++)
        {
            int mapNum = MapNumber(spawnPositionNumber);//맵 번호
            if (spawnPositionNumber == 12 || spawnPositionNumber == 31 || spawnPositionNumber == 37) spawnCount = 0;
            else spawnCount = Random.Range(5, 10);//스폰 마릿수
            Debug.Log(spawnCount + "맵 번호 " + mapNum+" 스폰 지점 "+spawnPositionNumber);
            for (int i = 0; i < spawnCount; i++)
            {
                if (mobCount[mapNum] >= mobMapMaxCount[mapNum]) continue;//최대 스폰 몬스터 수 추가
                int monsterNumber = MonsterPosition(spawnPositionNumber, mapNum);//스폰할 몬스터
                Debug.Log(monsterNumber + "몬스터 번호");
                GameObject gm = objectfulling.MakeObj(monsterNumber);
                gm.transform.position = spawnPositionList[spawnPositionNumber].position + new Vector3(Random.Range(-7, 7), 0.5f, Random.Range(-7, 7));
                spawnMonster.Add(gm);
            }
        }
        yield break;
    }
    IEnumerator BearMonsterSpawn()
    {
        int spawnPositionNumber = 12;
        int mapNum = MapNumber(spawnPositionNumber);
        int monsterNumber = MonsterPosition(spawnPositionNumber, mapNum);//스폰할 몬스터
        GameObject gm = objectfulling.MakeObj(monsterNumber);
        gm.transform.position = spawnPositionList[spawnPositionNumber].position + new Vector3(Random.Range(-7, 7), 1f, Random.Range(-7, 7));
        spawnMonster.Add(gm);
        yield break;
    }
    IEnumerator MusllstoneMonsterSpawn()
    {
        int spawnPositionNumber = 31;
        int mapNum = MapNumber(spawnPositionNumber);
        int monsterNumber = MonsterPosition(spawnPositionNumber, mapNum);//스폰할 몬스터
        GameObject gm = objectfulling.MakeObj(monsterNumber);
        gm.transform.position = spawnPositionList[spawnPositionNumber].position + new Vector3(Random.Range(-7, 7), 1f, Random.Range(-7, 7));
        spawnMonster.Add(gm);
        yield break;
    }
    IEnumerator FinalBossMonsterSpawn()
    {
        int spawnPositionNumber = 37;
        int mapNum = MapNumber(spawnPositionNumber);
        int monsterNumber = MonsterPosition(spawnPositionNumber, mapNum);//스폰할 몬스터
        GameObject gm = objectfulling.MakeObj(monsterNumber);
        gm.transform.position = spawnPositionList[spawnPositionNumber].position + new Vector3(Random.Range(-7, 7), 1f, Random.Range(-7, 7));
        spawnMonster.Add(gm);
        yield break;
    }
    int MapNumber(int spawnPositionNumber) {
        if (spawnPositionNumber <= 3) return 0;
        else if (spawnPositionNumber >= 4 && spawnPositionNumber <= 6) return 1;
        else if (spawnPositionNumber >= 7 && spawnPositionNumber <= 13) return 2;
        else if (spawnPositionNumber >= 14 && spawnPositionNumber <= 20) return 3;
        else if (spawnPositionNumber >= 21 && spawnPositionNumber <= 27) return 4;
        else if (spawnPositionNumber >= 28 && spawnPositionNumber <= 31) return 5;
        return 6;
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
        else if (spawnPositionNumber >= 10 && spawnPositionNumber <= 11)
        {
            return 10;
        }
        else if (spawnPositionNumber == 12)//곰
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
        else if (spawnPositionNumber == 31)//머슬스톤
        {
            return 24;
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
        else if (spawnPositionNumber == 37)//멧돼지
        {
            return 25;
        }
        return 0;
    }
    //초기 스폰
    void InitSpawn()
    {
        for(int spawnIdx = 0; spawnIdx < 38; spawnIdx++)//모든 스폰 지점
        {
            int mapNum = MapNumber(spawnIdx);
            //보스는 1마리 씩만
            int spawnCount = (spawnIdx == 12 || spawnIdx == 31 || spawnIdx == 37)?1: Random.Range(5, 9);//스폰 마릿수
            for (int i = 0; i < spawnCount; i++)
            {
                int monsterNumber = MonsterPosition(spawnIdx, mapNum);//스폰할 몬스터
                GameObject gm = objectfulling.MakeObj(monsterNumber);
                gm.transform.position = spawnPositionList[spawnIdx].position + new Vector3(Random.Range(-7, 7), 0.5f, Random.Range(-7, 7));
                spawnMonster.Add(gm);
            }
        }
    }
}
