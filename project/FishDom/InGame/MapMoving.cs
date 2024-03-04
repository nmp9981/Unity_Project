using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMoving : MonoBehaviour
{
    [SerializeField] GameObject target;
    private void Update()
    {
        gameObject.transform.position = target.transform.position;
    }
}
