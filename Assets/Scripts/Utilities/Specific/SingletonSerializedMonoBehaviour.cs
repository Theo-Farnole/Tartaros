using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonSerializedMonoBehaviour<T> : SerializedMonoBehaviour where T : SerializedMonoBehaviour
{
    private static T _instance = null;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                T[] instances =
                    FindObjectsOfType<T>();

                if (instances.Length > 1)
                {
                    Debug.LogWarning(instances[0].name + " There is more than one instance of " + typeof(T) + " Singleton. ");
                }
                if (instances != null && instances.Length > 0)
                {
                    _instance = instances[0];
                }
            }

            return _instance;
        }
    }
}
