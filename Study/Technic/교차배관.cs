/// <summary>
/// 외적으로 교차배관 방향 구하기
/// </summary>
/// <returns></returns>
Vector3 DirectionInstallCrossPipe(Vector3 pipeDir, Vector3 up)
{
    return Vector3.Cross(up, pipeDir);
}
/// <summary>
/// 교차배관 설치
/// 1) 간격에 따라 파이프 설치
/// 1-1) 교차 배관 설치 시작 지점
/// 1-2) 교차 배관 설치 끝지점
/// 1-3) 위 두 점을 연결ㅋ
/// 2) 이를 모든 파이프에 대해 반복
/// </summary>
public void InstallCrossPipe()
{
    List<Vector3> realRoomPointList = new List<Vector3>();
    realRoomPointList = realHousePointList;
    
    int currentSp = 0;
    for (int idx = 0; idx < realRoomPointList.Count; idx++)
    {

        //파이프
        Vector3 pipeInRoom = Vector3.zero;
        if (idx == realRoomPointList.Count - 1)
        {
            pipeInRoom = realRoomPointList[idx] - realRoomPointList[idx-1];
        }
        else
        {
            pipeInRoom = realRoomPointList[idx + 1] - realRoomPointList[idx];
        }
        Vector3 pipeDir = pipeInRoom.normalized;

        //설치 방향
        Vector3 installDir = Vector3.zero;
        installDir = DirectionInstallCrossPipe(pipeDir, Vector3.up);

        //방 크기            
        float roomLength = GetRoomLength(idx);
       
        //교차 배관 설치 시작 지점
        //Vector3 crossPipeStartPoint = realRoomPointList[idx] + (roomLength - ((crossPipeCount - 1) * diffSprinkler)) * 0.5f * pipeDir;
        Vector3 crossPipeStartPoint = realRoomPointList[idx];

        //layer : 열개수라 가정 -> 최종 설치하는 파이프 개수
        var layer = roomDataList[idx].SprinklerLayer;
        int count = (int)(layer.Datas.Count / layer.GetLayer(pipeDir));
        //int count = (int)((roomData[idx].Sprinklers.Count) / roomData[idx].layer);
        //배관설치 간격
        float diffSprinkler = roomLength / (count+2);

        //교차 배관 설치
        int jIndexStart = - (count / 2);
        for (int j = jIndexStart; j < jIndexStart+count; j++)
        {
            if (idx == 3)//ㄴ자 예외처리
            {
                if (j < 0)
                {
                    pipeDir = Vector3.right;
                }
                else
                {
                    pipeDir = Vector3.forward;
                }
                
            }
            if(idx==10 && j > 0)
            {
                continue;
            }
            if (idx == 11 && j < 0)
            {
                continue;
            }

            //설치 시작 지점
            Vector3 installStartPos = crossPipeStartPoint + j * diffSprinkler * pipeDir 
                + diffSprinkler * pipeDir * (count%2==0?0.5f:0);//설치 개수가 짝수일 경우 보정 값
           

            //맨 처음
            if(idx==0 && j <= 0)
            {
                installStartPos = crossPipeStartPoint;
            }

            //맨끝
            if (idx == realRoomPointList.Count - 1 && j>=0)
            {
                installStartPos = crossPipeStartPoint;
            }

            GameObject crossPipePoint = Instantiate(shpere);
            crossPipePoint.name = idx.ToString()+"ST";
            crossPipePoint.transform.position = installStartPos;

            //최대 길이 구함
            float maxLength = 0f;

            if (roomDataList[idx].Sprinklers == null)
            {
                continue;
            }
            
            for (int k = 0; k < roomDataList[idx].Sprinklers.Count; k++)
            {
                SprinklerData sprink = roomDataList[idx].Sprinklers[k];
                if (pipeDir.x ==0)//x좌표 비교
                {
                    float xDiff = Mathf.Abs(sprink.Center.X - realRoomPointList[idx].x);
                    maxLength = Mathf.Abs(Mathf.Max(xDiff, maxLength));
                }
                else if (pipeDir.z == 0)//z좌표 비교
                {
                    float zDiff = Mathf.Abs(sprink.Center.Z - realRoomPointList[idx].z);
                    maxLength = Mathf.Abs(Mathf.Max(zDiff, maxLength));
                }
                else//2,10
                {
                    if (idx == 2)
                    {
                        installDir = new Vector3(0, 0, -1);
                        float xDiff = Mathf.Abs(sprink.Center.Z - realRoomPointList[idx].z);
                        maxLength = Mathf.Abs(Mathf.Max(xDiff, maxLength));
                    }
                    if (idx == 10)
                    {
                        installDir = new Vector3(1, 0, 0);
                        float xDiff = Mathf.Abs(sprink.Center.X - realRoomPointList[idx].x);
                        maxLength = Mathf.Abs(Mathf.Max(xDiff, maxLength));
                    }
                    
                }
            }
            

            //설치 종료 지점
            Vector3 installEndPos = installStartPos + maxLength * installDir;
            if (idx == 3 && j == -1)//ㄴ자 예외처리
            {
                installEndPos = installStartPos + maxLength * Vector3.back;
            }

            GameObject crossPipePoint2 = Instantiate(shpere);
            crossPipePoint2.transform.position = installEndPos;
            crossPipePoint2.name = idx.ToString() + "ED";
            //두 점을 잇기
            ConnectTwoPoints(crossPipePoint.transform.position, crossPipePoint2.transform.position,0.3f,PIPETYPE.Cross);
        }
        currentSp = sprinklerNumList[idx];
    }
}
