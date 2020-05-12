using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GUIHelper
{
    public static void DrawQuad(Rect position, Color color)
    {
        GUI.skin.box.normal.background = GenerateTexture2D(color);
        GUI.Box(position, GUIContent.none);
    }

    public static Texture2D GenerateTexture2D(Color color)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();

        return texture;
    }
}
