using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Texture2DExtension
{
    public static Sprite ToSprite(this Texture2D tex)
    {
        return Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
    }

    public static void DrawLine(this Texture2D tex, int x0, int y0, int x1, int y1, Color fulfillValue)
    {
        int dx = Mathf.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
        int dy = Mathf.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
        int err = (dx > dy ? dx : -dy) / 2, e2;

        for (; ; )
        {
            tex.SetPixel(x0, y0, fulfillValue);
            if (x0 == x1 && y0 == y1) break;
            e2 = err;
            if (e2 > -dx) { err -= dy; x0 += sx; }
            if (e2 < dy) { err += dx; y0 += sy; }
        }

        tex.Apply();
    }
}
