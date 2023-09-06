using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResPawnManager : MonoBehaviour
{
    public ObjectManager objectManager;
    public string[] enemySpawn;//적 소환
    [SerializeField] Transform[] enemySpawnPos;//적 소환 위치

    float coolTime;
    float curTime;
    float minCoolTime = 1.0f;
    // Start is called before the first frame update
    void Awake()
    {
        enemySpawn = new string[] {"EnemyA", "EnemyB","EnemyC","EnemyD"};//적 목록
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

            coolTime = Random.Range(minCoolTime,Mathf.Max(minCoolTime+0.3f, 4.8f-(float)GamaManager.Instance.Stage));
        }
    }
    //적 소환
    void SpawnEnemy()
    {
        int minIndex = GamaManager.Instance.Stage>4?1:0;
        int maxIndex = Mathf.Min(4, GamaManager.Instance.Stage);
        int enemyIndex = Random.Range(minIndex, maxIndex);//생성 범위
        //생성
        int instiatePos = Random.Range(0, 6);//생성 위치

        GameObject enemy = objectManager.MakeGameObject(enemySpawn[enemyIndex]);
        enemy.transform.position = enemySpawnPos[instiatePos].position;
    }
}
