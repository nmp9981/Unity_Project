using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    const int bulletMaxCount = 30;//총알 최대 개수
    const int enemyMaxCount = 20;//적 최대 마릿수
    const int itemMaxCount = 15;//아이템 최대 개수

    //프리팹 준비
    public GameObject playerBulletA_Prefab;//플레이어 총알
    public GameObject enemyA_Prefab;//적기
    public GameObject enemyB_Prefab;
    public GameObject enemyC_Prefab;
    public GameObject enemyBulletA_Prefab;//적 총알
    public GameObject enemyBulletB_Prefab;
    public GameObject powerPrebab;//파워 아이템
    public GameObject posionPrebab;//포션 아이템
    public GameObject coinPrefab;//코인 아이템

    //오브젝트 배열
    GameObject[] playerBulletA;
    GameObject[] enemyA;
    GameObject[] enemyB;
    GameObject[] enemyC;
    GameObject[] enemyBulletA;
    GameObject[] enemyBulletB;
    GameObject[] powerItem;
    GameObject[] posionItem;
    GameObject[] coinItem;

    //타겟 오브젝트
    GameObject[] targetPool;

    // Start is called before the first frame update
    void Awake()
    {
        playerBulletA = new GameObject[bulletMaxCount];
        enemyA = new GameObject[enemyMaxCount];
        enemyB = new GameObject[enemyMaxCount];
        enemyC = new GameObject[enemyMaxCount];
        enemyBulletA = new GameObject[bulletMaxCount];
        enemyBulletB = new GameObject[bulletMaxCount];
        powerItem = new GameObject[itemMaxCount];
        posionItem = new GameObject[itemMaxCount];
        coinItem = new GameObject[itemMaxCount];
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
        for(int i = 0; i < enemyBulletB.Length; i++)
        {
            enemyBulletB[i] = Instantiate(enemyBulletB_Prefab);
            enemyBulletB[i].SetActive(false);
        }
        //적
        for (int i = 0; i < enemyA.Length; i++)
        {
            enemyA[i] = Instantiate(enemyA_Prefab);//프리팹 필요
            enemyA[i].SetActive(false);//처음엔 비활성화
        }
        for (int i = 0; i < enemyB.Length; i++)
        {
            enemyB[i] = Instantiate(enemyB_Prefab);//프리팹 필요
            enemyB[i].SetActive(false);//처음엔 비활성화
        }
        for(int i = 0; i < enemyC.Length; i++)
        {
            enemyC[i] = Instantiate(enemyC_Prefab);//프리팹 필요
            enemyC[i].SetActive(false);//처음엔 비활성화
        }
        //아이템
        for(int i = 0; i < powerItem.Length; i++)
        {
            powerItem[i] = Instantiate(powerPrebab);
            powerItem[i].SetActive(false);
        }
        for(int i = 0; i < posionItem.Length; i++)
        {
            posionItem[i] = Instantiate(posionPrebab);
            posionItem[i].SetActive(false);
        }
        for(int i = 0; i < coinItem.Length; i++)
        {
            coinItem[i] = Instantiate(coinPrefab);
            coinItem[i].SetActive(false);
        }
    }
   
    //오브젝트 생성
    public GameObject MakeGameObject(string type)
    {
        switch (type)
        {
            case "BulletPlayerA":
                targetPool = playerBulletA;
                break;
            case "EnemyA":
                targetPool = enemyA;
                break;
            case "EnemyB":
                targetPool = enemyB;
                break;
            case "EnemyC":
                targetPool = enemyC;
                break;
            case "EnemyBulletA":
                targetPool = enemyBulletA;
                break;
            case "EnemyBulletB":
                targetPool = enemyBulletB;
                break;
            case "PowerItem":
                targetPool = powerItem;
                break;
            case "PosionItem":
                targetPool = posionItem;
                break;
            case "CoinItem":
                targetPool = coinItem;
                break;
        }
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
            case "EnemyB":
                targetPool = enemyB;
                break;
            case "EnemyC":
                targetPool = enemyC;
                break;
            case "EnemyBulletA":
                targetPool = enemyBulletA;
                break;
            case "EnemyBulletB":
                targetPool = enemyBulletB;
                break;
            case "PowerItem":
                targetPool = powerItem;
                break;
            case "PosionItem":
                targetPool = posionItem;
                break;
            case "CoinItem":
                targetPool = coinItem;
                break;
        }
        return targetPool;
    }
}
