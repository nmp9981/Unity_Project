using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public int mobCount;
    ObjectFulling objectfulling;
    void Awake()
    {
        InvokeRepeating("MonsterSpawn", 0.5f, 8f);
        objectfulling = GameObject.Find("ObjectManager").GetComponent<ObjectFulling>();
        mobCount = 0;
        GameManager.Instance.MaxMonsterCount = 18;
    }
    void MonsterSpawn()
    {
        for(int i = 0; i < 5; i++)
        {
            if (mobCount >= GameManager.Instance.MaxMonsterCount) return;
            int monsterNumber = Random.Range(4, 6);
            GameObject gm = objectfulling.MakeObj(monsterNumber);
            gm.transform.position = new Vector3(Random.Range(-9,9),-1f,Random.Range(-9,9));
            mobCount++;
        }
    }
}
