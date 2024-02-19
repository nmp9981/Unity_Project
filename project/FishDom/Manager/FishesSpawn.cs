using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishesSpawn : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < 3; i++)
        {
            for (int j=3; j < 3; j++)
            {

                GameManager.ObjectFill.MakeObj(0).transform.position = new Vector3(i,j,0);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
