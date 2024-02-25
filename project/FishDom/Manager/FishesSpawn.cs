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
            int mobCount = UnityEngine.Random.Range(5, 9);
            GameObject[] fishobjects = _objectFull.GetPool(GameManager.Instance.StageNum - 1);
            for (int i = 0; i < mobCount; i++)
            {
                int spawnX = UnityEngine.Random.Range(2, 6);
                int spawnY = UnityEngine.Random.Range(2, 6);
                int spawnMinus = (UnityEngine.Random.Range(-5, 5)>=0)?-1:1;
                GameObject gm = _objectFull.MakeObj(GameManager.Instance.StageNum - 1);
                gm.transform.position = new Vector3(target.transform.position.x + spawnX*spawnMinus, target.transform.position.y + spawnY*spawnMinus, 0);
            }
            yield return new WaitForSeconds(5.0f);
        }
    }
    public IEnumerator ItemSpawnLogic()
    {
        while (true)
        {
            yield return new WaitForSeconds(18.0f);
            int spawnX = UnityEngine.Random.Range(3, 7);
            int spawnY = UnityEngine.Random.Range(3, 7);
            int spawnMinus = (UnityEngine.Random.Range(-5, 5) >= 0) ? -1 : 1;
            GameObject gm = _objectFull.MakeObj(17);
            gm.transform.position = new Vector3(target.transform.position.x + spawnX * spawnMinus, target.transform.position.y + spawnY * spawnMinus, 0);
        }
    }
    // Update is called once per frame
    void Update()
    {
       
    }
}
