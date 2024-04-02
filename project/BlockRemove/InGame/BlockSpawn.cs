using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSpawn : MonoBehaviour
{
    ObjectPulling _objectPulling;
    void Start()
    {
        _objectPulling = GameObject.Find("ObjectPulling").GetComponent<ObjectPulling>();
        BlockSetting();
    }

    void BlockSetting()
    {
        for(int i= -GameManager.Instance.RowCount / 2; i < GameManager.Instance.RowCount / 2; i++)
        {
            for(int j = -GameManager.Instance.ColCount / 2; j < GameManager.Instance.ColCount / 2; j++)
            {
                int blockNum = Random.Range(0, GameManager.Instance.BlockKinds);
                GameObject gm = _objectPulling.MakeObj(blockNum);
                gm.transform.position = new Vector3(j,i,0);
            }
        }
    }
}
