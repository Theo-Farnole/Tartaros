using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Object as Dont destroy on load
/// </summary>
public class DontDestroyObject : MonoBehaviour {

    private static DontDestroyObject _instance = null;

    public static DontDestroyObject Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject().AddComponent<DontDestroyObject>();
                _instance.name = "[AUTOGEN] Dont Destroy Object";
                DontDestroyOnLoad(_instance);
            }

            return _instance;
        }
    }
}
