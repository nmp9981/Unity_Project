using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fllower : MonoBehaviour
{
    public Transform target;//따라갈 타겟
    public Vector3 offset;//보정값

    // Update is called once per frame
    void Update()
    {
        transform.position = target.position + offset;
    }
}
