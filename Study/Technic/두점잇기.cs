/// <summary>
/// 두 점 잇기
/// 구형 포인트 2개를 잇는다
/// 내적, 외적 활용
/// </summary>
public void ConnectTwoPoints(GameObject point1, GameObject point2, float size, PIPETYPE pipeType)
{
    //벡터 차
    Vector3 dir = point2.transform.position - point1.transform.position;
    
    //내적을 이용해서 y축 회전각도 계산
    //두 벡터 모두 정규화된 벡터이므로 크기는 둘다 1
    float dotValue = Vector3.Dot(dir.normalized, Vector3.forward);
    float angle = Mathf.Acos(dotValue)*180f*(1f/Mathf.PI);//호도법->육십분법

    //좌우판정, 단순 내적만으로는 좌우판정 불가
    //1 : 오른쪽, -1 : 왼쪽
    float judgeRightOrLeftValue = Vector3.Dot(Vector3.up,Vector3.Cross(Vector3.forward,dir.normalized));
    float sideDir = (judgeRightOrLeftValue >= 0) ? 1 : -1;

    //파이프 생성
    GameObject pipeGM = Instantiate(pipeObj);
    pipeGM.name = $"{pipeType.ToString()}_{point1.name}_{point2.name}";
    pipeGM.transform.eulerAngles = new Vector3(pipeGM.transform.eulerAngles.x, angle*sideDir, 0);

    pipeGM.transform.position = (point2.transform.position + point1.transform.position) * 0.5f;
    pipeGM.transform.localScale = new Vector3(size, dir.magnitude * 0.5f, size);
}
