using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoving : MonoBehaviour
{
    [SerializeField] GameObject target;
    private Vector3 cameraZoom = new Vector3(0, 0, -10);
    
    private void LateUpdate()
    {
        gameObject.transform.position = target.transform.position + cameraZoom;
    }
}
