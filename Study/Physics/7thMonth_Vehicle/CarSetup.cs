[System.Serializable]
public class CarSetup
{
    // Spring
    public float springFront;
    public float springRear;

    // Progressive
    public float spring2Front;
    public float spring2Rear;

    // Damper
    public float damperCompFront;
    public float damperRebFront;

    public float damperCompRear;
    public float damperRebRear;

    // ARB
    public float arbFront;
    public float arbRear;

    // Tire
    public float baseMu;
    public float loadSensitivity;

    // Drive
    public float frontSplit;
}
