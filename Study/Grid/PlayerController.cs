using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private GridMovement gridMovement;

    private void Awake()
    {
        gridMovement = GetComponent<GridMovement>();
    }
    private void Update()
    {
        float hAxis = Input.GetAxisRaw("Horizontal");
        float vAxis = Input.GetAxisRaw("Vertical");

        if(hAxis !=0 || vAxis != 0)
        {
            gridMovement.moveDir = new Vector3(hAxis, vAxis, 0);
        }
    }
}
