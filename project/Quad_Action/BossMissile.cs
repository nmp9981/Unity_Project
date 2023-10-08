using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossMissile : Bullet//Bullet의 기능은 그대로 사용해야함
{
    public Transform target;
    NavMeshAgent nav;
    // Start is called before the first frame update
    void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        nav.SetDestination(target.position);//추적
    }
}
