using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtension 
{
    /// <summary>
    /// Get component. If no component is attached, add one.
    /// </summary>
    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent(typeof(T)) as T;

        if (component == null)
        {
            component = gameObject.gameObject.AddComponent(typeof(T)) as T;
        }

        return component;
    }
}
