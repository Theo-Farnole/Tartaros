using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace TF.MultiSceneManager
{
    public class LoadMultiSceneOnAwake : MonoBehaviour, IPreprocessBuildWithReport
    {
#if UNITY_EDITOR
        [SerializeField] private UnityEditor.SceneAsset _sceneToLoad;
#endif

        [SerializeField, HideInInspector] private string _sceneToLoadName;

        int IOrderedCallback.callbackOrder => throw new System.NotImplementedException();

        void Awake()
        {
            MultiSceneManager.LoadScene(_sceneToLoadName);
        }

#if UNITY_EDITOR
        void OnValidate() => SceneAssetToName();

        void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport report) => SceneAssetToName();

        void SceneAssetToName()
        {
            if (_sceneToLoad != null)
            {
                _sceneToLoadName = _sceneToLoad.name;
            }
        }
#endif
    }
}
