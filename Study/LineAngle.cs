private void DrawBackSpinInfo(float parentAngle, float value)
    {
        //TODO: 기본적으로는 1600 rpm
        //Transform trArrow = GetObject((int)GameObjects.BackSpinRedArrow).transform;
        //Vector3 ballPosition = GetObject((int)GameObjects.SideViewGolfBall).transform.position;//공의 월드 좌표
        Vector2 ballPosition = GetObject((int)GameObjects.SideViewGolfBall).GetComponent<RectTransform>().anchoredPosition;
        RectTransform trArrow = GetObject((int)GameObjects.BackSpinRedArrow).GetComponent<RectTransform>();
       
        if (value > ZeroPoint)
        {
            GetText((int)Texts.BackSpinTitle).text = $"{Keyword.BackSpin}";

            // 회전을 표시하는 붉은 화살표를 오른쪽으로 표시
            trArrow.localScale = new Vector3(2, trArrow.localScale.y, trArrow.localScale.z);
        }
        else
        {
            GetText((int)Texts.BackSpinTitle).text = $"{Keyword.TopSpin}";

            // 회전을 표시하는 붉은 화살표를 왼쪽으로 표시
            trArrow.localScale = new Vector3(-2, trArrow.localScale.y, trArrow.localScale.z);
        }
        GetText((int)Texts.BackSpinValue).text = $"<size={SpinFontSize}>{Mathf.Abs(value):F0}</size> rpm";

        //BackSpinInfo의 위치
        trArrow.anchoredPosition = new Vector2(0, 0);

        //화살표가 직선의 법선벡터에 위치
        if (trArrow.localScale.x == 2) trArrow.rotation = Quaternion.Euler(0, 0, -ArrowAngle + parentAngle);
        else if (trArrow.localScale.x == -2) trArrow.rotation = Quaternion.Euler(0, 0, ArrowAngle + parentAngle);

        float theta = parentAngle * MathF.PI / 180.0f;
        GetObject((int)GameObjects.BackSpinInfo).GetComponent<RectTransform>().anchoredPosition = new Vector2(lineLength, ballPosition.y + lineLength * Mathf.Sin(theta) / 2.0f +100.0f);

        //GetObject((int)GameObjects.BackSpinInfo).transform.position = new Vector3(lineLength*2.5f, ballPosition.y + lineLength * Mathf.Sin(theta) + 100.0f,0.0f);
        //GetObject((int)GameObjects.BackSpinInfo).transform.localRotation = Quaternion.Euler(new Vector3(ZeroPoint, ZeroPoint, -parentAngle));
    }

 private void DrawDirectionLine(float value)
    {
        if (Mathf.Approximately(ZeroPoint, value))
            GetText((int)Texts.DirectionValue).text = $"<size={DataModeFontSize-1}>{ZeroPoint:F1}{IVUtil.GetAngleSymbol()}</size>";
        else
            GetText((int)Texts.DirectionValue).text =
                $"<size={DataModeFontSize-1}>{(value > ZeroPoint ? Keyword.R : Keyword.L)}{Mathf.Abs(value):F1}{IVUtil.GetAngleSymbol()}</size>";

        Transform trLine = GetObject((int)GameObjects.DirectionLine).transform;
        RectTransform trInfo = GetObject((int)GameObjects.DirectionInfo).GetComponent<RectTransform>();

        float lineXSize = trLine.GetComponent<RectTransform>().sizeDelta.x;
        Vector3 origintrPos = trLine.position+new Vector3(lineLength,0,0);//월드 좌표계 기준 원래 위치

        RotateDirectionLine(trLine, trInfo, value);//회전을 먼저함


        var test = GetLengthOfBetweenArcAndInfoText();

        // TODO: 빨간색 지시자 끝에 정보들이 위치할수있게 계산한다.

        #region 빨간색 지시자 끝에 정보들이 위치할수있게 계산한다.
        if (Mathf.Approximately(ZeroPoint, value)) trInfo.pivot = new Vector2(0, 0f);
        else if (value > 0) trInfo.pivot = new Vector2(0, 1f);
        else if (value < 0) trInfo.pivot = new Vector2(0, 0f);
        
        //월드 좌표계를 사용해서 좌우 고정시키고 위치를 지정한다.
        trInfo.transform.position = new Vector3(origintrPos.x+lineLength*0.15f, origintrPos.y + lineXSize * MathF.Sin(-value*MathF.PI/180.0f)/5.0f, 0.0f);
        #endregion

        float angle = Mathf.Abs(value);
        Vector2 vec2 = RepositionSpinInfo(_topViewOriginGolfBallPos, angle);//회전 변환
        Debug.LogError("공의 원래 위치 " + _topViewOriginGolfBallPos.x);
        //볼을 기준선에 맞춘다.
        RectTransform trGolfBall = GetObject((int)GameObjects.TopViewGolfBall).GetComponent<RectTransform>();
        trGolfBall.localPosition =
            new Vector3(
                _topViewOriginGolfBallPos.x + (_topViewOriginGolfBallPos.x - vec2.x) * 1.414f,
                trGolfBall.localPosition.y,
                trGolfBall.localPosition.z);
    }
