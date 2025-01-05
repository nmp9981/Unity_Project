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
            int mobNum = Random.Range(0, 2);
            GameObject mob = objectFulling.MakeObj(mobNum);
            mob.transform.position = spawnPoint.position;

            await UniTask.Delay(GameManager.Instance.SpawnTime);
        }
    }
}
