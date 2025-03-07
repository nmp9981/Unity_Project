using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpMap
{
    public class PlayerCollier : MonoBehaviour
    {
        [SerializeField]
        GameObject player;

        BoxCollider2D boxCollider;

        private void Awake()
        {
            boxCollider = GetComponent<BoxCollider2D>();
        }
        // Update is called once per frame
        void Update()
        {
            if ((player.transform.position.y - player.transform.localScale.y * 0.5f) <= this.transform.position.y)
            {
                boxCollider.enabled = false;
            }
            else
            {
                boxCollider.enabled = true;
            }
        }
    }
}
