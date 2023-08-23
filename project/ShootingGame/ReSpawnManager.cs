using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResPawnManager : MonoBehaviour
{
    public ObjectManager objectManager;
    public string[] enemySpawn;//적 소환
    Transform[] enemySpawnPos;//적 소환 위치

    float coolTime;
    float curTime;
    // Start is called before the first frame update
    void Awake()
    {
        enemySpawn = new string[] {"EnemyA"};//적 목록
        curTime = 0.0f;
        coolTime = 2.0f;
    }

    // Update is called once per frame
    void Update()
    {
        curTime += Time.deltaTime;
        if(curTime > coolTime)
        {
            SpawnEnemy();//적 소환
            curTime = 0.0f;
            coolTime = Random.Range(1.8f, 3.8f);
        }
    }
    //적 소환
    void SpawnEnemy()
    {
        int enemyIndex = 0;
        //생성
        GameObject enemy = objectManager.MakeGameObject(enemySpawn[enemyIndex]);
        enemy.transform.position = new Vector3(Random.Range(-2,2), 4, 0);
    }
}
