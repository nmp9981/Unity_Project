using TMPro;
using UnityEngine;
using Vectrosity;
using UnityEngine.SceneManagement;
using static IVGameSetting;

public partial class IVDataModeSystem
{
    private const float MinimumDistance25 = 25;
    private int _limitXDistance;
    GameObject totalDistance;
    GameObject targetPos;
    VectorLine sideErrorLine;
    VectorLine targetLine;

    protected virtual void DrawTopViewSideError()
    {
        XCenter = GameObject.Find("XLine_Center");
        totalDistance = GameObject.Find("PlayTotalValue");

        var tr = GetObject((int)GameObjects.TopViewBaseXLine).transform.GetComponent<RectTransform>();
        float deltaX = GetDistance(tr.sizeDelta.x);
        float z = TopViewTargetHeight / (TopViewHeightDivideByCount * TopViewMeterPerOneLine);
        //var endIndicator = GetObject((int)GameObjects.EndIndicator);
        //endIndicator.SetActive(true);

        Vector3 ballPos = lastBallPos;// Ball 의 마지막 값을 가져옴
                                      //Vector3 ballPos = GetEndPos();

        //막대는 1개씩만 그리게한다.
        if(sideErrorLine == null)
        {
            sideErrorLine = VectorLine.SetLine(Color.red, new Vector2(ballPos.x, ballPos.y), new Vector2(ballPos.x, XCenter.transform.position.y));//그리기
            sideErrorLine.lineWidth = 5;
            sideErrorLine.active = true;
        }
        else
        {
            sideErrorLine.active = false;
            sideErrorLine = VectorLine.SetLine(Color.red, new Vector2(ballPos.x, ballPos.y), new Vector2(ballPos.x, XCenter.transform.position.y));//그리기
            sideErrorLine.lineWidth = 5;
            sideErrorLine.active = true;
        }

        //np모드일 경우 타겟까지 하나 더 그리기
        if (SceneManager.GetActiveScene().name == "NearPinDataMode")
        {
            targetPos = GetObject((int)GameObjects.TopViewTargetDistanceRedFocus);
            
            //막대는 1개씩만 그리게 한다.
            if (targetLine == null)
            {
                targetLine = VectorLine.SetLine(Color.red, new Vector2(ballPos.x, ballPos.y), new Vector2(targetPos.transform.position.x, XCenter.transform.position.y));//그리기
                sideErrorLine.lineWidth = 5;
                targetLine.active = true;
            }
            else
            {
                targetLine.active = false;
                targetLine = VectorLine.SetLine(Color.red, new Vector2(ballPos.x, ballPos.y), new Vector2(targetPos.transform.position.x, XCenter.transform.position.y));//그리기
                sideErrorLine.lineWidth = 5;
                targetLine.active = true;
            }
        }
       
        //side Error 값
        float y = ballPos.y / z; // meter로 복원

        string dist = GetText((int)Texts.Total).text;
        string[] distword = dist.Split(' ');
        float sideErrorValue = (lastBallPosCanvas.y / lastBallPosCanvas.x)*float.Parse(distword[0]);

        //단위에 따라 달라짐
        if ((int)IVUtil.GetGameSetting(GameSettingEnum.Distance_unit) == 0) GetText((int)Texts.SideErrorValue).text = $"{Mathf.Abs(sideErrorValue):F1}m";
        else
        {
            string newY = IVUtil.AttachUnitNameToValue(Mathf.Abs(sideErrorValue), IVGameSetting.GameSettingEnum.ft);
            GetText((int)Texts.SideErrorValue).text = newY;

        }
    }
}
