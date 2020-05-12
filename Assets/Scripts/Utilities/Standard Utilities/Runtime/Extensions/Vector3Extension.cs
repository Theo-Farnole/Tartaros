using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Extension
{
    public static Vector3 GetClosestPoint(this Vector3 currentPosition, Vector3[] points)
    {
        Vector3 closestPoint = Vector3.one * Mathf.Infinity;
        float minimunDistance = Mathf.Infinity;

        for (int i = 0; i < points.Length; i++)
        {
            float distance = Vector3.Distance(points[i], currentPosition);

            if (distance < minimunDistance)
            {
                closestPoint = points[i];
                minimunDistance = distance;
            }
        }
        return closestPoint;
    }

    public static Vector3 SetX(this Vector3 v, float value)
    {
        return new Vector3( value, v.y, v.z);                
    }

    public static Vector3 SetY(this Vector3 v, float value)
    {
        return new Vector3(v.x, value, v.z);
    }

    public static Vector3 SetZ(this Vector3 v, float value)
    {
        return new Vector3(v.x, v.y, value);
    }
	
	/// <summary>
    /// Project vector on Y plane
    /// </summary>
    public static Vector3 ToXY(this Vector3 v)
    {
        return new Vector3(v.x, v.y, 0);
    }
}
