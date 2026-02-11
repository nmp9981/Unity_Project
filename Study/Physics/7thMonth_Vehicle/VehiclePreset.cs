using UnityEngine;

[CreateAssetMenu(fileName = "VehiclePreset", menuName = "Scriptable Objects/VehiclePreset")]
public class VehiclePreset : ScriptableObject
{
    public string nameString;
    public float mass;
    public float cgHeight;
    public float wheelBase;
    public float frontTrack;
    public float rearTrack;
    public float tireGrip;
    public float frontStiffness;
    public float rearStiffness;
}
