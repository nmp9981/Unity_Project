using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartGraphDraw : MonoBehaviour
{
    [SerializeField]
    Material mt;

    CanvasRenderer renderer;

    string shaderName = "UI/Default";

    void Awake()
    {
        renderer = gameObject.GetComponent<CanvasRenderer>();
        mt.shader = Shader.Find(shaderName);
    }

    private void OnEnable()
    {
        DrawGraph();
    }
    private void OnDisable()
    {
        DestroyGraph();
    }
    //그래프 그리기
    void DrawGraph()
    {
        Mesh mesh = new();

        float speed = (GameManager.Instance.speedLimitList[GameManager.Instance.MouseUpKartNum]-100);
        float startAccel = (GameManager.Instance.startTouqueList[GameManager.Instance.MouseUpKartNum] - 5000)/70f;
        float boosterAccel = GameManager.Instance.boosterTouqueList[GameManager.Instance.MouseUpKartNum]/30f;
        float breakPow = GameManager.Instance.breakePowerList[GameManager.Instance.MouseUpKartNum] / 400f;
        float boosterSpeed = (GameManager.Instance.boosterSpeedLimitList[GameManager.Instance.MouseUpKartNum] - 200);
        /** Vertices **/
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(0f, 1f, 0f)*speed,
            new Vector3(-0.951f, 0.309f, 0f)*startAccel,
            new Vector3(-0.588f, -0.809f, 0f)*boosterAccel,
            new Vector3(0.588f, -0.809f, 0f)*breakPow,
            new Vector3(0.951f, 0.309f, 0f)*boosterSpeed
        };

        /** UV **/
        Vector2[] uv = new Vector2[]
        {
            new Vector2(0f, 1f),
            new Vector2(0.5f, 1f),
            new Vector2(1f, 0.5f),
            new Vector2(0f, 0f),
            new Vector2(1f,0f)
        };

        /** 삼각형 그리는 순서 **/
        int[] triangles = new int[]
        {
            0, 1, 2,
            2, 3, 0,
            0, 3, 4
        };

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        renderer.SetMesh(mesh);
        renderer.SetMaterial(mt,null);
    }
    //그래프 지우기
    void DestroyGraph()
    {
        renderer.Clear();
    }
}
