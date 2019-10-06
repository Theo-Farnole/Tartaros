using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class MultiSceneManager
{
    private readonly static string SCENE_PATH = "Assets/Scenes/";

    private readonly static string[] LEVEL_SCENES_NAMES = new string[] { "SC_level" };
    private readonly static string[] LOGIC_SCENES_NAMES = new string[] { "SC_game_logic" };

    [MenuItem("Leonidas Legacy/Load Logic for current level")]
    private static void LoadLogicScenes()
    {
        string[] loadedLevelScenes = GetLoadedSceneInArray(LEVEL_SCENES_NAMES);
        string[] loadedLogicScenes = GetLoadedSceneInArray(LOGIC_SCENES_NAMES);

        if (loadedLevelScenes.Length != 1)
        {
            if (loadedLevelScenes.Length == 0)
            {
                Debug.LogError("Open a level scene before loading required logic.");
            }
            else if (loadedLevelScenes.Length > 1)
            {
                Debug.LogError("Can't load required logic with 2 differents levels.");
            }

            return;
        }

        // load missing logic
        for (int i = 0; i < LOGIC_SCENES_NAMES.Length; i++)
        {
            bool isLogicSceneLoaded = Array.Exists(loadedLogicScenes, element => element == LOGIC_SCENES_NAMES[i]);

            if (!isLogicSceneLoaded)
            {
                EditorSceneManager.OpenScene(SCENE_PATH + LOGIC_SCENES_NAMES[i] + ".unity", OpenSceneMode.Additive);
            }
        }
    }

    static string[] GetLoadedSceneInArray(string[] scenesNames)
    {
        List<string> o = new List<string>();

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene loadedScene = SceneManager.GetSceneAt(i);
            bool isAWantedScene = Array.Exists(scenesNames, element => element == loadedScene.name);

            if (isAWantedScene && loadedScene.isLoaded)
            {
                o.Add(loadedScene.name);
            }
        }

        return o.ToArray();
    }
}
