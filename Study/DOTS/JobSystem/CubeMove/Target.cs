using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Target : MonoBehaviour
{
    public Vector3 Direction;
    // Update is called once per frame
    public void Update()
    {
        transform.localPosition += Direction * Time.deltaTime;
    }
}
