using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Racing2D
{
    public class CameraMove : MonoBehaviour
    {
        GameObject player;
      
        void Awake()
        {
            player = GameObject.Find("Car Controller");
        }

      
        private void FixedUpdate()
        {
            gameObject.transform.position = player.transform.position + new Vector3(0, 0, -10);
        }
    }
}
