using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    //프리팹 준비
    public GameObject enemyBPrefab;
    public GameObject enemyLPrefab;
    public GameObject enemyMPrefab;
    public GameObject enemySPrefab;
    public GameObject itemCoinPrefab;
    public GameObject itemPowerPrefab;
    public GameObject itemBoomPrefab;
    public GameObject bulletPlayerAPrefab;
    public GameObject bulletPlayerBPrefab;
    public GameObject bulletEnemyAPrefab;
    public GameObject bulletEnemyBPrefab;
    public GameObject bulletFollowerPrefab;
    public GameObject bulletBossAPrefab;
    public GameObject bulletBossBPrefab;

    //오브젝트 배열
    GameObject[] enemyB;
    GameObject[] enemyL;
    GameObject[] enemyM;
    GameObject[] enemyS;

    GameObject[] itemCoin;
    GameObject[] itemPower;
    GameObject[] itemBoom;

    GameObject[] bulletPlayerA;
    GameObject[] bulletPlayerB;
    GameObject[] bulletEnemyA;
    GameObject[] bulletEnemyB;
    GameObject[] bulletFollwer;
    GameObject[] bulletBossA;
    GameObject[] bulletBossB;

    GameObject[] targetPool;

    private void Awake()
    {
        enemyB = new GameObject[10];
        enemyL = new GameObject[10];
        enemyM = new GameObject[10];
        enemyS = new GameObject[10];

        itemCoin = new GameObject[20];
        itemPower = new GameObject[10];
        itemBoom = new GameObject[10];

        bulletPlayerA = new GameObject[100];
        bulletPlayerB = new GameObject[100];
        bulletEnemyA = new GameObject[100];
        bulletEnemyB = new GameObject[100];
        bulletFollwer = new GameObject[100];
        bulletBossA = new GameObject[100];
        bulletBossB = new GameObject[100];

        Generate();
    }
    void Generate()
    {
        //적
        for (int i = 0; i < enemyB.Length; i++)
        {
            enemyB[i] = Instantiate(enemyBPrefab);//프리팹 필요
            enemyB[i].SetActive(false);//처음엔 비활성화
        }
        for (int i = 0; i < enemyL.Length; i++)
        {
            enemyL[i] = Instantiate(enemyLPrefab);//프리팹 필요
            enemyL[i].SetActive(false);//처음엔 비활성화
        }
        for (int i = 0; i < enemyM.Length; i++)
        {
            enemyM[i] = Instantiate(enemyMPrefab);//프리팹 필요
            enemyM[i].SetActive(false);//처음엔 비활성화
        }
        for (int i = 0; i < enemyS.Length; i++)
        {
            enemyS[i] = Instantiate(enemySPrefab);//프리팹 필요
            enemyS[i].SetActive(false);//처음엔 비활성화
        }

        //아이템
        for (int i = 0; i < itemCoin.Length; i++)
        {
            itemCoin[i] = Instantiate(itemCoinPrefab);//프리팹 필요
            itemCoin[i].SetActive(false);//처음엔 비활성화
        }
        for (int i = 0; i < itemPower.Length; i++)
        {
            itemPower[i] = Instantiate(itemPowerPrefab);//프리팹 필요
            itemPower[i].SetActive(false);//처음엔 비활성화
        }
        for (int i = 0; i < itemBoom.Length; i++)
        {
            itemBoom[i] = Instantiate(itemBoomPrefab);//프리팹 필요
            itemBoom[i].SetActive(false);//처음엔 비활성화
        }

        //총알
        for (int i = 0; i < bulletEnemyA.Length; i++)
        {
            bulletEnemyA[i] = Instantiate(bulletEnemyAPrefab);//프리팹 필요
            bulletEnemyA[i].SetActive(false);//처음엔 비활성화
        }
        for (int i = 0; i < bulletEnemyB.Length; i++)
        {
            bulletEnemyB[i] = Instantiate(bulletEnemyBPrefab);//프리팹 필요
            bulletEnemyB[i].SetActive(false);//처음엔 비활성화
        }
        for (int i = 0; i < bulletPlayerA.Length; i++)
        {
            bulletPlayerA[i] = Instantiate(bulletPlayerAPrefab);//프리팹 필요
            bulletPlayerA[i].SetActive(false);//처음엔 비활성화
        }
        for (int i = 0; i < bulletPlayerB.Length; i++)
        {
            bulletPlayerB[i] = Instantiate(bulletPlayerBPrefab);//프리팹 필요
            bulletPlayerB[i].SetActive(false);//처음엔 비활성화
        }
        for (int i = 0; i < bulletFollwer.Length; i++)
        {
            bulletFollwer[i] = Instantiate(bulletFollowerPrefab);//프리팹 필요
            bulletFollwer[i].SetActive(false);//처음엔 비활성화
        }
        for (int i = 0; i < bulletBossA.Length; i++)
        {
            bulletBossA[i] = Instantiate(bulletBossAPrefab);//프리팹 필요
            bulletBossA[i].SetActive(false);//처음엔 비활성화
        }
        for (int i = 0; i < bulletBossB.Length; i++)
        {
            bulletBossB[i] = Instantiate(bulletBossBPrefab);//프리팹 필요
            bulletBossB[i].SetActive(false);//처음엔 비활성화
        }
    }

    public GameObject MakeObj(string type)
    {
        switch (type)
        {
            case "EnemyB":
                targetPool = enemyB;
                break;
            case "EnemyL":
                targetPool = enemyL;
                break;
            case "EnemyM":
                targetPool = enemyM;
                break;
            case "EnemyS":
                targetPool = enemyS;
                break;
            case "ItemCoin":
                targetPool = itemCoin;
                break;
            case "ItemPower":
                targetPool = itemPower;
                break;
            case "ItemBoom":
                targetPool = itemBoom;
                break;
            case "BulletPlayerA":
                targetPool = bulletPlayerA;
                break;
            case "BulletPlayerB":
                targetPool = bulletPlayerB;
                break;
            case "BulletEnemyA":
                targetPool = bulletEnemyA;
                break;
            case "BulletEnemyB":
                targetPool = bulletEnemyB;
                break;
            case "BulletFollower":
                targetPool = bulletFollwer;
                break;
            case "BulletBossA":
                targetPool = bulletBossA;
                break;
            case "BulletBossB":
                targetPool = bulletBossB;
                break;
        }
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
    public GameObject[] GetPool(string type)//지정한 오브젝트 풀 가져오기
    {
        switch (type)
        {
            case "EnemyB":
                targetPool = enemyB;
                break;
            case "EnemyL":
                targetPool = enemyL;
                break;
            case "EnemyM":
                targetPool = enemyM;
                break;
            case "EnemyS":
                targetPool = enemyS;
                break;
            case "ItemCoin":
                targetPool = itemCoin;
                break;
            case "ItemPower":
                targetPool = itemPower;
                break;
            case "ItemBoom":
                targetPool = itemBoom;
                break;
            case "BulletPlayerA":
                targetPool = bulletPlayerA;
                break;
            case "BulletPlayerB":
                targetPool = bulletPlayerB;
                break;
            case "BulletEnemyA":
                targetPool = bulletEnemyA;
                break;
            case "BulletEnemyB":
                targetPool = bulletEnemyB;
                break;
            case "BulletFollower":
                targetPool = bulletFollwer;
                break;
            case "BulletBossA":
                targetPool = bulletBossA;
                break;
            case "BulletBossB":
                targetPool = bulletBossB;
                break;
        }
        return targetPool;
    }
}
