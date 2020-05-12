using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TF.MultiSceneManager
{
    [System.Serializable]
    public class SceneInput
    {
#if UNITY_EDITOR
        public SceneAsset sceneAsset;
#endif

        public string sceneName;

#if UNITY_EDITOR
        public void AssetToName()
        {
            if (sceneAsset != null)
                sceneName = sceneAsset.name;
        }
#endif
    }
}
