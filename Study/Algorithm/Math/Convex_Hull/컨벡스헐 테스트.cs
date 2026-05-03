using System.Collections.Generic;
using UnityEngine;
using ConVexHULL;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
using System.Linq;

[RequireComponent(typeof(LineRenderer))]
public class ALgorithmTest : MonoBehaviour
{
    [SerializeField] GameObject pointObj;
    [SerializeField] LineRenderer lineRenderer;

    List<ConVexHULL.Point> pointList = new List<ConVexHULL.Point>();
    Stack<ConVexHULL.Point> pointStack = new Stack<ConVexHULL.Point>();
    ConvecHull conv = new ();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SpawnPoint();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            pointStack = conv.AllFlow(pointList);
            DrawResult();
        }
    }

    void SpawnPoint()
    {
        for(int i = 0; i < 50; i++)
        {
            int xInt = Random.Range(-5, 5);
            int xPrime = Random.Range(0, 100);
            float x = xInt + 0.01f * xPrime;

            int yInt = Random.Range(-4, 4);
            int yPrime = Random.Range(0, 100);
            float y = yInt + 0.01f * yPrime;

            GameObject obj = Instantiate(pointObj);
            obj.transform.position = new Vector3 (x, y, 0);

            ConVexHULL.Point p = new ConVexHULL.Point(x, y);
            pointList.Add(p);
        }
    }

    /// <summary>
    /// 결과 그리기
    /// </summary>
    void DrawResult()
    {
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

        ConVexHULL.Point first = pointStack.Peek();
        List<Vector3> pointVectors = new List<Vector3>();
        while (pointStack.Count > 0)
        {
            ConVexHULL.Point top = pointStack.Pop();
            pointVectors.Add(new Vector3(top.x,top.y,0));
        }
        pointVectors.Add(new Vector3(first.x,first.y, 0));

        lineRenderer.positionCount = pointVectors.Count;
        lineRenderer.SetPositions(pointVectors.ToArray());
    }
}
