void Update()
{
    if (_isActive)
    {
        Vector3 targetPosition;

        if (Vector3.Distance(_boundsData.center, Camera.transform.position) < _boundsData.center.y + _boundsData.size.y + Zoomin)
        {
            Camera.transform.LookAt(_targetObject);
            Clear();
            return;
        }
        Zoomin = 0;
        // Bounds 크기에 따라 줌 거리 조절
        //targetPosition = new Vector3(_boundsData.center.x, _boundsData.center.y + _boundsData.size.y + Zoomin, _boundsData.center.z);
        Debug.Log("거리 " +Zoomin);
        targetPosition = new Vector3(_boundsData.center.x - _boundsData.size.x/2+Zoomin, _boundsData.center.y + _boundsData.size.y/2 + Zoomin, _boundsData.center.z- _boundsData.size.z/2+Zoomin);

        Camera.transform.position = Vector3.SmoothDamp(Camera.transform.position, targetPosition, ref _velocity, SmoothTime);
        Camera.transform.LookAt(_targetObject);

        if (Vector3.Distance(targetPosition, Camera.transform.position) < 0.1f)
        {
            Clear();
        }
    }
}


using RTG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class ObjectFocusInCamera : MonoBehaviour
{
    public GameObject Camera;
    public float SmoothTime = 0.3f;
    public float Zoomin = 1.5f;
    public static ObjectFocusInCamera instance;
    private bool _isActive = false;
    private Transform _targetObject;
    private Vector3 _velocity = Vector3.zero;
    public Bounds _boundsData;
    
    private void Start()
    {
        Zoomin = 0f;
        instance = this;
    }
    void Update()
    {
        if (_isActive)
        {
            Vector3 targetPosition;

            // Bounds ũ�⿡ ���� �� �Ÿ� ����
            targetPosition = new Vector3(_boundsData.center.x - _boundsData.size.x / 2 + Zoomin,
                             _targetObject.position.y + _boundsData.size.y  + Zoomin,
                             _boundsData.center.z - _boundsData.size.z / 2 + Zoomin);

            Camera.transform.position = Vector3.SmoothDamp(Camera.transform.position, targetPosition, ref _velocity, SmoothTime);
            Camera.transform.LookAt(_targetObject);

            if (Vector3.Distance(targetPosition, Camera.transform.position) < 0.1f)
            {
                Clear();
            }
        }
    }

    public void SetTarget(GameObject target)
    {
        Debug.Log(target.name);
        if (target.gameObject.tag == "GizmoAxis")
        {
            _isActive = false;
            return;
        }
        if (target == null)
            return;
        var renderers = target.GetComponentsInChildren<Renderer>();

        if (renderers == null)
        {
            return;
        }

        Bounds combinedBounds = new Bounds();
        if (renderers != null)
        {
            foreach (var render in renderers)
            {
                combinedBounds.Encapsulate(render.bounds);
            }
        }
        else
        {
            foreach(Transform child in target.transform)
            {
                if (child.GetComponent<Renderer>() != null)
                {
                    combinedBounds.Encapsulate(child.GetComponent<Renderer>().bounds);      
                }
            }
        }
        _boundsData = combinedBounds;
        _isActive = true;
        _targetObject = target.transform;
    }

    public void Clear()
    {
        SmoothTime = 0.3f;
        _isActive = false;
        _targetObject = null;
    }
}
