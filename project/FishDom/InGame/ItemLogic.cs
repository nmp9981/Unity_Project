using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemLogic : MonoBehaviour
{
    private float dir;
    // Start is called before the first frame update
    void Start()
    {
        SetMoveDir();
    }

    // Update is called once per frame
    void Update()
    {
        ItemMove();
    }
    void SetMoveDir()
    {
        dir = Random.Range(0, 10);
        dir = (dir % 2 == 0) ? -1 : 1;
    }
    void ItemMove()
    {
        Vector3 ememyMoveDir = new Vector3(dir, 0, 0);
        gameObject.transform.position += ememyMoveDir * GameManager.Instance.EnemyMoveSpeed * Time.deltaTime;
    }
}
