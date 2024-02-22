using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishesSpawn : MonoBehaviour
{
    ObjectFulling _objectFull;
    [SerializeField] GameObject target;
    
    public void Init()
    {
        _objectFull = GameObject.Find("ObjectManager").GetComponent<ObjectFulling>();
    }
    
    public IEnumerator FishSpawnLogic()
    {
        while (true)
        {
            int mobCount = UnityEngine.Random.Range(1, 5);
            GameObject[] fishobjects = _objectFull.GetPool(GameManager.Instance.StageNum - 1);
            for (int i = 0; i < mobCount; i++)
            {
                int spawnX = UnityEngine.Random.Range(-5, 5);
                int spawnY = UnityEngine.Random.Range(-5, 5);
                GameObject gm = _objectFull.MakeObj(GameManager.Instance.StageNum - 1);
                gm.transform.position = new Vector3(target.transform.position.x + spawnX, target.transform.position.y + spawnY, 0);
            }
            yield return new WaitForSeconds(5.0f);
        }
    }
}
