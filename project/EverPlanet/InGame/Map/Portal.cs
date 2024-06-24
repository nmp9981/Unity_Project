using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] int nextPortalNum;
    [SerializeField] GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && PortalManager.PortalInstance.Curtime>= PortalManager.PortalInstance.Cooltime)
        {
            float dist = Vector3.Magnitude(player.transform.position - gameObject.transform.position);
            if (dist < 2)
            {
                PortalManager.PortalInstance.Curtime = 0;
                player.transform.position = PortalManager.PortalInstance.portalist[nextPortalNum].transform.position;
            }
        }
    }
}
