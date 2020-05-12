using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TF.MultiSceneManager.Editor
{
    [InitializeOnLoad]
    class AutomaticLoadAdditionalScenes
    {
        static AutomaticLoadAdditionalScenes()
        {
            Debug.LogFormat("<color=yellow>MultiScene</color> # Automatic loading initalized.");

            EditorSceneManager.sceneOpened += EditorSceneManager_sceneOpened;
        }

        private static void EditorSceneManager_sceneOpened(Scene scene, OpenSceneMode mode)
        {
            // prevent scene loading twice
            if (Application.isPlaying)
                return;

            EditorMultiSceneManager.LoadScene(scene, mode);
        }
    }
}