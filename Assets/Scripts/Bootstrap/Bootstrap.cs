using Lortedo.Utilities.Managers;
using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private SceneAsset _sceneToLoad;
#endif

    [SerializeField, ReadOnly] private string _sceneToLoadName;

    void Start()
    {
        SceneManager.LoadScene(_sceneToLoadName);
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        _sceneToLoadName = _sceneToLoad.name;
    }
#endif
}
