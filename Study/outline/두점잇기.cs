/// <summary>
/// 두 점 잇기
/// 구형 포인트 2개를 잇는다
/// </summary>
public void ConnectTwoPoints(GameObject point1, GameObject point2, float size, PIPETYPE pipeType)
{
    Vector3 dir = point2.transform.position - point1.transform.position;
    GameObject pipeGM = Instantiate(pipeObj);
    if (dir.z <= 0.01f)//z가 움직임
    {
        //90도 회전
        pipeGM.transform.eulerAngles = new Vector3(pipeGM.transform.eulerAngles.x, 90f, pipeGM.transform.eulerAngles.z);

    }
    pipeGM.transform.position = (point2.transform.position + point1.transform.position) * 0.5f;
    pipeGM.transform.localScale = new Vector3(size, dir.magnitude * 0.5f, size);
    pipeGM.name = pipeType.ToString();
}
