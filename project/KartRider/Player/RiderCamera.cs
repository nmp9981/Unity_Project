using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiderCamera : MonoBehaviour
{
    GameObject player;
    private void Awake()
    {
        player = GameObject.Find("Player");
    }
    private void FixedUpdate()
    {
        gameObject.transform.position = player.transform.position+new Vector3(0,1,-10);
    }
}
