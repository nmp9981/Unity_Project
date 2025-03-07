using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpMap
{
    public class MonsterMove : MonoBehaviour
    {
        [SerializeField]
        GameObject area;

        float speed = 4;
        float moveDir = 1;
        Bounds bounds;

        private void Awake()
        {
            bounds = area.GetComponent<SpriteRenderer>().bounds;
        }
        void Update()
        {
            MoveMonster();
        }
        void MoveMonster()
        {
            //왼쪽으로 이동해야함
            if(transform.position.x >= bounds.center.x + bounds.size.x*0.45f)
            {
                moveDir = -1;
            }
            //오른쪽으로 이동해야함
            if (transform.position.x <= bounds.center.x - bounds.size.x*0.45f)
            {
                moveDir = 1;
            }
            transform.position += Vector3.right *moveDir* speed * Time.deltaTime;
        }
    }
}
