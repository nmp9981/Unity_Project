Bounds GetBound(GameObject gm)
{
    Bounds newBound = default;
    Vector3 newBoundCenter = Vector3.zero;
    int newBoundCount = 0;

    float minX = 10000;
    float minY = 10000;
    float minZ = 10000;
    float maxX = 0;
    float maxY = 0;
    float maxZ = 0;

    foreach (MeshRenderer child in gm.GetComponentsInChildren<MeshRenderer>(true))//촬영 대상
    {
        Bounds newSubBound = child.GetComponent<MeshRenderer>().bounds;

        newBoundCenter += newSubBound.center;
        newBoundCount++;

        if (minX > newSubBound.center.x - newSubBound.size.x / 2) minX = newSubBound.center.x - newSubBound.size.x / 2;
        if (minY > newSubBound.center.y - newSubBound.size.y / 2) minY = newSubBound.center.y - newSubBound.size.y / 2;
        if (minZ > newSubBound.center.z - newSubBound.size.z / 2) minZ = newSubBound.center.z - newSubBound.size.z / 2;
        if (maxX < newSubBound.center.x + newSubBound.size.x / 2) maxX = newSubBound.center.x + newSubBound.size.x / 2;
        if (maxY < newSubBound.center.y + newSubBound.size.y / 2) maxY = newSubBound.center.y + newSubBound.size.y / 2;
        if (maxZ < newSubBound.center.z + newSubBound.size.z / 2) maxZ = newSubBound.center.z + newSubBound.size.z / 2;
    }
    if (newBoundCount >= 1)
    {
        newBound.center = newBoundCenter / newBoundCount;
        newBound.size = new Vector3(maxX, maxY, maxZ) - new Vector3(minX, minY, minZ);
    }
    return newBound;
}
