using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartZone : MonoBehaviour
{
    public GameManager manager;

    private void OnTriggerEnter(Collider other)
    {
        //게임 시작
        if (other.gameObject.tag == "Player")
        {
            manager.StageStart();
        }
    }
}
