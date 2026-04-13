using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TelemetryGraph : MonoBehaviour
{
    public RawImage image;
    Texture2D texture;

    int width = 512;
    int height = 256;

    Color[] clearPixels;

    void Start()
    {
        texture = new Texture2D(width, height);
        image.texture = texture;

        clearPixels = new Color[width * height];
    }

    public void Clear()
    {
        for (int i = 0; i < clearPixels.Length; i++)
            clearPixels[i] = Color.black;

        texture.SetPixels(clearPixels);
    }
    /// <summary>
    /// 연결할 두 점을 찍는다.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="color"></param>
    public void Draw(List<float> data, float min, float max, Color color)
    {
        Clear();

        int count = data.Count;
        if (count < 2) return;

        for (int i = 1; i < count; i++)
        {
            float v0 = MathUtility.InverseLerp(min, max, data[i - 1]);
            float v1 = MathUtility.InverseLerp(min, max, data[i]);

            int x0 = (i - 1) * width / count;
            int x1 = i * width / count;

            int y0 = (int)(v0 * height);
            int y1 = (int)(v1 * height);

            DrawLine(x0, y0, x1, y1, color);
        }

        texture.Apply();
    }
    /// <summary>
    /// 두 점을 연결해서 그린다
    /// </summary>
    /// <param name="x0"></param>
    /// <param name="y0"></param>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="color"></param>
    void DrawLine(int x0, int y0, int x1, int y1, Color color)
    {
        int dx = (int)MathUtility.Abs(x1 - x0);
        int dy = (int)MathUtility.Abs(y1 - y0);
        
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;

        int err = dx - dy;

        //두 지점사이의 여러점을 찍음(출발지에서 목표치 도달할때까지)
        while (true)
        {
            texture.SetPixel(x0, y0, color);

            if (x0 == x1 && y0 == y1) break;

            int e2 = 2 * err;

            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }

            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }
}
