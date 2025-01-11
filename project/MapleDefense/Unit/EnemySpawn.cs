using UnityEngine;
using Cysharp.Threading.Tasks;

public class EnemySpawn : MonoBehaviour
{
    [SerializeField]
    ObjectFulling objectFulling;
    [SerializeField]
    Transform spawnPoint;

    private void Start()
    {
        SpawnEnemy().Forget();
    }
    /// <summary>
    /// 일정 시간마다 적 소환
    /// </summary>
    /// <returns></returns>
    async UniTaskVoid SpawnEnemy()
    {
        while (true)
        {
            //어떤 적을 소환할지
            int mobNum = SetSpawnEnemy();
            GameObject mob = objectFulling.MakeObj(mobNum);
            mob.transform.position = spawnPoint.position;
            //활성화 몬스터에 추가
            GameManager.Instance.ActiveUnitList.Add(mob);

            await UniTask.Delay(GameManager.Instance.SpawnTime);
        }
    }
    int SetSpawnEnemy()
    {
        int mobNum = 0;
        if (GameManager.Instance.CurrentStage <= 1)
        {
            mobNum = Random.Range(0, 2);
        }
        else
        {
            mobNum = Random.Range(3, 5);
        }
        return mobNum;
    }
}
