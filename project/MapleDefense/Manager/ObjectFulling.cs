using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class ObjectFulling : MonoBehaviour
{
    //프리팹 준비
    const int monsterMaxCount = 35;//각 오브젝트 개수
    const int monsterKinds = 21;//종류 개수
    
    const int throwMaxCount = 45;
    const int throwKinds = 12;
    
    const int supportMaxCount = 20;
    const int supportKinds = 12;
    //몬스터 프리팹
    public GameObject[] monsterPrefabs;
    //투사체 프리팹
    public GameObject[] throwPrefabs;
    //소환수 프리팹
    public GameObject[] supportPrefabs;

    //오브젝트 배열
    GameObject[][] monsters;
    GameObject[] monsterTargetPool;

    GameObject[][] throws;
    GameObject[] throwTargetPool;

    GameObject[][] supports;
    GameObject[] supportsTargetPool;

    void Awake()
    {
        monsters = new GameObject[monsterKinds][]
        {
             new GameObject[monsterMaxCount],
             new GameObject[monsterMaxCount],
             new GameObject[monsterMaxCount],
              new GameObject[monsterMaxCount],
               new GameObject[monsterMaxCount],
               new GameObject[monsterMaxCount],
             new GameObject[monsterMaxCount],
             new GameObject[monsterMaxCount],
              new GameObject[monsterMaxCount],
               new GameObject[monsterMaxCount],
              new GameObject[monsterMaxCount],
              new GameObject[monsterMaxCount], new GameObject[monsterMaxCount],
              new GameObject[monsterMaxCount],
              new GameObject[monsterMaxCount], new GameObject[monsterMaxCount],
              new GameObject[monsterMaxCount],
              new GameObject[monsterMaxCount], new GameObject[monsterMaxCount],
              new GameObject[monsterMaxCount],
              new GameObject[monsterMaxCount]
        };

        throws = new GameObject[throwKinds][]
        {
            new GameObject[throwMaxCount],
            new GameObject[throwMaxCount],
            new GameObject[throwMaxCount],
            new GameObject[throwMaxCount],
            new GameObject[throwMaxCount],
            new GameObject[throwMaxCount],
            new GameObject[throwMaxCount],
            new GameObject[throwMaxCount],
            new GameObject[throwMaxCount],
            new GameObject[throwMaxCount],
            new GameObject[throwMaxCount],
            new GameObject[throwMaxCount]
        };

        supports = new GameObject[supportKinds][]
        {
            new GameObject[supportMaxCount],
             new GameObject[supportMaxCount],
              new GameObject[supportMaxCount],
               new GameObject[supportMaxCount],new GameObject[supportMaxCount],
             new GameObject[supportMaxCount],
              new GameObject[supportMaxCount],
               new GameObject[supportMaxCount],new GameObject[supportMaxCount],
             new GameObject[supportMaxCount],
              new GameObject[supportMaxCount],
               new GameObject[supportMaxCount]
        };

        Generate();
    }
    void Generate()
    {
        //몬스터
        for (int i = 0; i < monsterKinds; i++)
        {
            if (i >= 23 && i <= 25)
            {
                for (int j = 0; j < 1; j++)
                {
                    monsters[i][j] = Instantiate(monsterPrefabs[i]);
                    monsters[i][j].SetActive(false);
                }
            }
            else
            {
                for (int j = 0; j < monsterMaxCount; j++)
                {
                    monsters[i][j] = Instantiate(monsterPrefabs[i]);
                    monsters[i][j].SetActive(false);
                }
            }
        }

        for (int i = 0; i < throwKinds; i++)
        {
            for (int j = 0; j < throwMaxCount; j++)
            {
                throws[i][j] = Instantiate(throwPrefabs[i]);
                throws[i][j].SetActive(false);
            }
        }

        for (int i = 0; i < supportKinds; i++)
        {
            for (int j = 0; j < supportMaxCount; j++)
            {
                supports[i][j] = Instantiate(supportPrefabs[i]);
                supports[i][j].SetActive(false);
            }
        }
    }
    //오브젝트 생성
    public GameObject MakeMonsterObj(int num)
    {
        monsterTargetPool = monsters[num];
        for (int i = 0; i < monsterTargetPool.Length; i++)
        {
            if (!monsterTargetPool[i].activeSelf)
            {
                monsterTargetPool[i].SetActive(true);//활성화 후 넘김
                return monsterTargetPool[i];
            }
        }
        return null;//없으면 빈 객체
    }

    public GameObject MakeThrowObj(int num)
    {
        throwTargetPool = throws[num];
        for (int i = 0; i < throwTargetPool.Length; i++)
        {
            if (!throwTargetPool[i].activeSelf)
            {
                throwTargetPool[i].SetActive(true);//활성화 후 넘김
                return throwTargetPool[i];
            }
        }
        return null;//없으면 빈 객체
    }

    public GameObject MakeSupportsObj(int num)
    {
        supportsTargetPool = supports[num];
        for (int i = 0; i < supportsTargetPool.Length; i++)
        {
            if (!supportsTargetPool[i].activeSelf)
            {
                supportsTargetPool[i].SetActive(true);//활성화 후 넘김
                return supportsTargetPool[i];
            }
        }
        return null;//없으면 빈 객체
    }
    //오브젝트 존재여부 확인
    public bool IsActiveObj(int num)
    {
        monsterTargetPool = monsters[num];

        for (int i = 0; i < monsterTargetPool.Length; i++)
        {
            if (monsterTargetPool[i].activeSelf) return true;//활성화된게 있으면
        }
        return false;//없으면 빈 객체
    }
    //오브젝트 존재하면 가져오기
    public GameObject GetObj(int num)
    {
        monsterTargetPool = monsters[num];

        for (int i = 0; i < monsterTargetPool.Length; i++)
        {
            if (monsterTargetPool[i].activeSelf) return monsterTargetPool[i];//활성화된게 있으면
        }
        return null;//없으면 빈 객체
    }
    //오브젝트 배열 가져오기
    public GameObject[] GetPool(int num)//지정한 오브젝트 풀 가져오기
    {
        monsterTargetPool = monsters[num];
        return monsterTargetPool;
    }
    //오브젝트들 비활성화
    public void OffObj()
    {
        for (int i = 0; i < monsterKinds; i++)
        {
            for (int j = 0; j < monsterMaxCount; j++)
            {
                if (monsters[i][j].activeSelf) monsters[i][j].SetActive(false);
            }
        }
    }
}
