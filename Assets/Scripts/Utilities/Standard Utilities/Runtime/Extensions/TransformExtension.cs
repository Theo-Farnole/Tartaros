using System;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtension
{
    /// <summary>
    /// Returns nearest transform from transforms args.
    /// </summary>
    /// <param name="t"></param>
    /// <param name="transforms">List of transforms to check who's the nearest</param>
    /// <returns>Nearest Transform</returns>
    /// <author> 
    /// code from https://forum.unity.com/threads/clean-est-way-to-find-nearest-object-of-many-c.44315/
    /// </author>
    public static Transform GetClosestTransform(this Transform t, Transform[] transforms)
    {
        Vector3 currentPos = t.position;

        Transform tMin = null;
        float minDist = Mathf.Infinity;

        foreach (Transform e in transforms)
        {
            float dist = Vector3.Distance(e.position, currentPos);

            if (dist < minDist)
            {
                tMin = e;
                minDist = dist;
            }
        }
        return tMin;
    }

    public static ObjectByDistance<T> GetClosestComponent<T>(this Transform currentObject, T[] components) where T : MonoBehaviour
    {
        Vector3 currentPos = currentObject.transform.position;

        T nearestObject = null;
        float minDist = Mathf.Infinity;

        for (int i = 0; i < components.Length; i++)
        {
            float dist = Vector3.Distance(components[i].transform.position, currentPos);

            if (dist < minDist)
            {
                nearestObject = components[i];
                minDist = dist;
            }
        }
        return new ObjectByDistance<T>(nearestObject, minDist);
    }

    /// <summary>
    /// Destroy every child inside transform t
    /// </summary>
    /// <param name="t"></param>
    public static void DestroyChildren(this Transform t)
    {
        for (int i = t.childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(t.GetChild(i).gameObject);
        }
    }
	
	/// <summary>
    /// Destroy in Editor every child inside transform t
    /// </summary>
    /// <param name="t"></param>
    public static void DestroyImmediateChildren(this Transform t)
    {
        for (int i = t.childCount - 1; i >= 0; i--)
        {
            GameObject.DestroyImmediate(t.GetChild(i).gameObject);
        }
    }

    public static void ActionForEachChildren(this Transform t, Action<GameObject> action)
    {
        for (int i = 0; i < t.childCount; i++)
        {
            action(t.GetChild(i).gameObject);
        }
    }
}
