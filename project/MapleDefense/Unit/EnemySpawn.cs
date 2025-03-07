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
            //업그레이드 UI창 비활성화, 게임중
            if (!GameManager.Instance.IsOpenUpgradeUI && GameManager.Instance.IsGamePlaying)
            {
                //어떤 적을 소환할지
                int mobNum = SetSpawnEnemy();
                GameObject mob = objectFulling.MakeMonsterObj(mobNum);
                mob.transform.position = spawnPoint.position;
                //활성화 몬스터에 추가
                GameManager.Instance.ActiveUnitList.Add(mob);
            }
            await UniTask.Delay(GameManager.Instance.SpawnTime);
        }
    }
    /// <summary>
    /// 기능 : 소환할 적 번호
    /// </summary>
    /// <returns></returns>
    int SetSpawnEnemy()
    {
        int mobNum = (GameManager.Instance.CurrentStage+ Random.Range(1,6))/4;
        mobNum = Mathf.Min(20, mobNum);
        return mobNum;
    }
}
