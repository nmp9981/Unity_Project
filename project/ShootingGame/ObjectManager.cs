using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    const int bulletMaxCount = 30;//총알 최대 개수
    const int enemyMaxCount = 20;//적 최대 마릿수
    //프리팹 준비
    public GameObject playerBulletA_Prefab;//플레이어 총알
    public GameObject enemyA_Prefab;//적기
    public GameObject enemyBulletA_Prefab;//적 총알

    //오브젝트 배열
    GameObject[] playerBulletA;
    GameObject[] enemyA;
    GameObject[] enemyBulletA;

    //타겟 오브젝트
    GameObject[] targetPool;

    // Start is called before the first frame update
    void Awake()
    {
        playerBulletA = new GameObject[bulletMaxCount];
        enemyA = new GameObject[enemyMaxCount];
        enemyBulletA = new GameObject[bulletMaxCount];
        Generate();//생성
    }

    //오브젝트 생성
    void Generate()
    {
        //총알
        for (int i = 0; i < playerBulletA.Length; i++)
        {
            playerBulletA[i] = Instantiate(playerBulletA_Prefab);//프리팹 필요
            playerBulletA[i].SetActive(false);//처음엔 비활성화
        }
        //적 총알
        for(int i = 0; i < enemyBulletA.Length; i++)
        {
            enemyBulletA[i] = Instantiate(enemyBulletA_Prefab);
            enemyBulletA[i].SetActive(false);
        }
        //적
        for (int i = 0; i < enemyA.Length; i++)
        {
            enemyA[i] = Instantiate(enemyA_Prefab);//프리팹 필요
            enemyA[i].SetActive(false);//처음엔 비활성화
        }
    }

    //오브젝트 생성
    public GameObject MakeGameObject(string type)
    {
        Debug.Log(type);
        switch (type)
        {
            case "BulletPlayerA":
                targetPool = playerBulletA;
                break;
            case "EnemyA":
                targetPool = enemyA;
                break;
            case "EnemyBulletA":
                targetPool = enemyBulletA;
                break;
        }
        Debug.Log(targetPool);
        for(int i = 0; i < targetPool.Length; i++)
        {
            //활성화후 넘김
            if (!targetPool[i].activeSelf)
            { 
                targetPool[i].SetActive(true);
                return targetPool[i];
            }
        }
        return null;//없을시
    }
    //지정한 오브젝트 풀 가져오기
    public GameObject[] GetObject(string type)
    {
        switch (type)
        {
            case "BulletPlayerA":
                targetPool = playerBulletA;
                break;
            case "EnemyA":
                targetPool = enemyA;
                break;
            case "EnemyBulletA":
                targetPool = enemyBulletA;
                break;
        }
        return targetPool;
    }
}

