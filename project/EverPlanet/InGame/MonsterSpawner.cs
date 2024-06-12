using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public int mobCount;
    ObjectFulling objectfulling;

    float curTime = 0;
    float coolTime = 8.0f;

    static public List<GameObject> spawnMonster = new List<GameObject>();
    void Awake()
    {
        objectfulling = GameObject.Find("ObjectManager").GetComponent<ObjectFulling>();
        mobCount = 0;
        curTime = 7.0f;
        GameManager.Instance.MaxMonsterCount = 18;
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
        for (int i = 0; i < 5; i++)
        {
            if (mobCount >= GameManager.Instance.MaxMonsterCount) yield break;
            int monsterNumber = Random.Range(4, 6);
            GameObject gm = objectfulling.MakeObj(monsterNumber);
            gm.transform.position = new Vector3(Random.Range(-9, 9), 1f, Random.Range(-9, 9));
            spawnMonster.Add(gm);
            mobCount++;
        }
    }
}
