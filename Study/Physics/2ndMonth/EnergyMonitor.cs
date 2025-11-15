using UnityEngine;
using System.IO;

public class EnergyMonitor : MonoBehaviour
{
    public Transform body;
    public float mass = 1f;
    public float initialHeight = 5f;
    public Vector3 initialVelocity;
    private float startTime;//총 경과시간

    string path = "D:\\Data\\energy.txt";
    StreamWriter sw;

    private void Awake()
    {
        // 디렉토리가 존재하는지 확인하고 필요하면 생성
        string directory = Path.GetDirectoryName(path);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        sw = new StreamWriter(path);
    }

    void Start() => startTime = 0f;

    void FixedUpdate()
    {
        startTime += Time.fixedDeltaTime;
        Vector3 v = body.GetComponent<CustomRigidBody>()?.velocity ?? Vector3.zero;

        float Ek = 0.5f * mass * v.sqrMagnitude;
        float Ep = mass * Physics.gravity.magnitude * body.position.y;
        float Et = Ek + Ep;
      
       

        if (sw != null)
        {
            Debug.Log($"{body.name} Time:{startTime:F2}  Ek:{Ek:F3} Ep:{Ep:F3} Et:{Et:F3}");
            // 탭 (\t)으로 구분하여 기록
            sw.WriteLine($"{Et:F3}");
            sw.Flush(); // 즉시 파일에 쓰기 (성능 문제에 따라 생략 가능)
        }
    }
    // OnApplicationQuit() 함수를 사용하여 게임이 종료될 때 파일을 안전하게 닫습니다.
    private void OnApplicationQuit()
    {
        if (sw != null)
        {
            sw.Close(); // 파일 닫기
            sw.Dispose();
            sw = null;
        }
    }
}
