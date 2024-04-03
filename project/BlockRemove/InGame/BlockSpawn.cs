using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum BlockColor
{
    NULL,Red,Orange,Yellow,Green,Sky,Blue,Pupple,Black
}
public class BlockSpawn : MonoBehaviour
{
    ObjectPulling _objectPulling;
    BlockClick _blockClick;
    private void Awake()
    {
        _blockClick = GameObject.Find("MouseClick").GetComponent<BlockClick>();
    }
    void Start()
    {
        _objectPulling = GameObject.Find("ObjectPulling").GetComponent<ObjectPulling>();
        BlockInitSetting();
    }

    void BlockInitSetting()
    {
        for(int i= -GameManager.Instance.RowCount / 2; i < GameManager.Instance.RowCount / 2; i++)
        {
            for(int j = -GameManager.Instance.ColCount / 2; j < GameManager.Instance.ColCount / 2; j++)
            {
                int blockNum = Random.Range(0, GameManager.Instance.BlockKinds);
                GameObject gm = _objectPulling.MakeObj(blockNum);
                gm.transform.position = new Vector3(j,i,0);
                _blockClick.blockState[(int)(4 - i)][(int)(j + 10)] = BlockState(gm.gameObject.name);
            }
        }
        /*
        for (int i = 0; i < GameManager.Instance.RowCount; i++)
        {
            for (int j = 0; j < GameManager.Instance.ColCount; j++)
            {
                Debug.Log(i+"행"+j+"열  "+ _blockClick.blockState[i][j]);
            }
        }
        */
    }
    public void BlockBlockSetting()
    {
        _objectPulling.OffObj();
        for (int i = -GameManager.Instance.RowCount / 2; i < GameManager.Instance.RowCount / 2; i++)
        {
            for (int j = -GameManager.Instance.ColCount / 2; j < GameManager.Instance.ColCount / 2; j++)
            {
                int blockNum = _blockClick.blockState[4-i][j+10];//여기만 수정(4/4)
                if (blockNum == -1) continue;//빈 공간

                GameObject gm = _objectPulling.MakeObj(blockNum-1);//blockNum : 1~8
                gm.transform.position = new Vector3(j, i, 0);
            }
        }
    }
    public static int BlockState(string name)
    {
        switch (name.Substring(0,name.Length-12))
        {
            case "Red":
                return 1;
            case "Orange":
                return 2;
            case "Yellow":
                return 3;
            case "Green":
                return 4;
            case "Sky":
                return 5;
            case "Blue":
                return 6;
            case "Pupple":
                return 7;
            case "Black":
                return 8;
            case null:
                return 0;
        }
        return 0;
    }
}
