private void DrawDescendingRedIndicator(float sensorAngle)
    {
        CheckVectorLine(VectorKind.Desc);

        float radius = _redIndicatorRadiusX[DescendingRadius];
        float angle = GetArcAngle(radius, Mathf.Abs(sensorAngle), RedIndicatorRemainLength);

        if (Equal(ZeroPoint, sensorAngle))
        {
            _vectorLines[(int)VectorKind.Desc].MakeArc(
                _screenCenterPositionsForDrivers[1],
                _redIndicatorRadiusX[DescendingRadius] * Offset.x,
                _redIndicatorRadiusY[DescendingRadius] * Offset.x,
                Angle270, Angle270 + angle);
        }
        else
        {
            if (sensorAngle > 0)
            {
                _vectorLines[(int)VectorKind.Desc].MakeArc(
                    _screenCenterPositionsForDrivers[1],
                    _redIndicatorRadiusX[DescendingRadius] * Offset.x,
                    _redIndicatorRadiusY[DescendingRadius] * Offset.x,
                    Angle270, Angle270 + angle);
            }
            else
            {
                _vectorLines[(int)VectorKind.Desc].MakeArc(
                    _screenCenterPositionsForDrivers[1],
                    _redIndicatorRadiusX[DescendingRadius] * Offset.x,
                    _redIndicatorRadiusY[DescendingRadius] * Offset.x,
                    Angle270 - angle, Angle270);
            }
        }

        _vectorLines[(int)VectorKind.Desc].Draw();
        _vectorLines[(int)VectorKind.Desc].active = true;
    }
