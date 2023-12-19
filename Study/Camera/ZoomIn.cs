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
