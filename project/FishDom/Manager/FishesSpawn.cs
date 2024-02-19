using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishesSpawn : MonoBehaviour
{
    ObjectFulling _objectFull;
    GameObject root;
    
    public void Init()
    {
        _objectFull = GameObject.Find("ObjectManager").GetComponent<ObjectFulling>();
    }
    public void FishSpawnLogic()
    {
        for (int i = -2; i < 3; i++)
        {
            for (int j=-3; j < 3; j++)
            {
                GameObject gm = _objectFull.MakeObj(0);
                gm.transform.position = new Vector3(i,j,0);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
