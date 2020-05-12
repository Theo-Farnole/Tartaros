using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorExtension
{
    public static string ToHex(this Color c)
    {
        string hexR = ((int)(c.r * 255)).ToString("X2");
        string hexG = ((int)(c.g * 255)).ToString("X2");
        string hexB = ((int)(c.b * 255)).ToString("X2");

        return hexR + hexG + hexB;
    }

    public static void SetAlpha(this Color c, float alpha)
    {
        c = new Color(c.r, c.g, c.b, alpha);
    }

    public static Color TintColor(this Color defaultColor, Color hueColor)
    {
        Color.RGBToHSV(hueColor, out float h, out float s, out float v);
        return TintColor(defaultColor, h);
    }

    public static Color TintColor(this Color defaultColor, float newHue)
    {
        Color.RGBToHSV(defaultColor, out float hueDefaultColor, out float satDefaultColor, out float valueDefaultColor);
        Color output = Color.HSVToRGB(newHue, satDefaultColor, valueDefaultColor);
        output.a = defaultColor.a;
        return output;
    }
}
