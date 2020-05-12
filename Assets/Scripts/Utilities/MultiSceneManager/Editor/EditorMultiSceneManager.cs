using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TF.MultiSceneManager;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class EditorMultiSceneManager
{
    public static void LoadScene(Scene masterScene, OpenSceneMode mode)
    {
        var sceneNeeds = MultiSceneManagerData.Instance.GetSceneNeeds(masterScene.name);

        for (int i = 0; i < sceneNeeds.Length; i++)
        {
            var additionalSceneName = sceneNeeds[i];

            // don't load a second time a loaded scene
            if (IsSceneLoaded(additionalSceneName))
                continue;

            // find path from additionalSceneName
            if (GetScenePath(additionalSceneName, out string scenePath))
            {
                // loading scene
                var additionalScene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);

                // set master scene at the end of the scenes list
                EditorSceneManager.MoveSceneBefore(additionalScene, masterScene);
            }
        }
    }

    public static bool IsSceneLoaded(string sceneToCheck)
    {
        for (int i = 0; i < EditorSceneManager.sceneCount; i++)
        {
            var loadedScene = EditorSceneManager.GetSceneAt(i);

            if (loadedScene.name == sceneToCheck)
                return true;
        }

        return false;
    }

    public static bool GetScenePath(string sceneName, out string path)
    {
        string filters = string.Format("{0} t:Scene", sceneName);

        var foundedAssets = AssetDatabase.FindAssets(filters);

        if (foundedAssets.Length == 0)
        {
            path = string.Empty;
            return false;
        }
        else if (foundedAssets.Length == 1)
        {
            path = AssetDatabase.GUIDToAssetPath(foundedAssets[0]);
            return true;
        }
        else
        {
            Debug.LogErrorFormat("There is more than one scene with name {0}. MultiScene manager cannot work properly. Please rename one of your scene.", sceneName);

            path = string.Empty;
            return false;
        }
    }
}
