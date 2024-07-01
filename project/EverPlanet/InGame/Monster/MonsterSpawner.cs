using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public int[] mobCount;
    ObjectFulling objectfulling;

    [SerializeField] List<Transform> spawnPositionList;

    float curTime = 0;
    float coolTime = 7.0f;

    static public List<GameObject> spawnMonster = new List<GameObject>();
    void Awake()
    {
        objectfulling = GameObject.Find("ObjectManager").GetComponent<ObjectFulling>();
        mobCount = new int[2] {0,0};
        curTime = 7.0f;
        GameManager.Instance.MaxMonsterCount = 24;
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
        int spawnPositionNumber = Random.Range(0,7);
        int mapNum = MapNumber(spawnPositionNumber);
        for (int i = 0; i < 5; i++)
        {
            if (mobCount[mapNum] >= GameManager.Instance.MaxMonsterCount) yield break;
            int monsterNumber = MonsterPosition(spawnPositionNumber,mapNum);//스폰할 몬스터
            GameObject gm = objectfulling.MakeObj(monsterNumber);
            gm.transform.position = spawnPositionList[spawnPositionNumber].position +new Vector3(Random.Range(-9, 9), 1f, Random.Range(-9, 9));
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
        }else if (spawnPositionNumber >= 2 && spawnPositionNumber <= 3)
        {
            return Random.Range(4, 6);
        }
        else if (spawnPositionNumber == 4)
        {
            return 5;
        }
        else if (spawnPositionNumber >= 5 && spawnPositionNumber <= 6)
        {
            return Random.Range(5, 7);
        }
        return 0;
    }
}
