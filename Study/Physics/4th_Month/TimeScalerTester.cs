using UnityEngine;

public class TimeScalerTester : MonoBehaviour
{
    public float timeScale = 1f;
   
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) Time.timeScale = 1f;
        if (Input.GetKeyDown(KeyCode.Alpha2)) Time.timeScale = 0.2f;
        if (Input.GetKeyDown(KeyCode.Alpha3)) Time.timeScale = 20f;
    }
}
