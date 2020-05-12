using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace TF.MultiSceneManager
{
    public abstract class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
    {
        private static T _instance;

        #region Properties
        public static T Instance
        {
            get
            {
                if (_instance == null)
                    _instance = GetOrCreateSceneManagerData(Filename);

                return _instance;
            }
        }

        public static string Filename { get => typeof(T).Name.ToString().ToProperCase(); }
        #endregion

        #region Methods
        private static T GetOrCreateSceneManagerData(string filename)
        {
            T sceneManagerData = Resources.Load<T>(filename);

            // create one, or throw error
            if (sceneManagerData == null)
                CreateSceneManagerData(out sceneManagerData, filename);

            return sceneManagerData;
        }

        private static void CreateSceneManagerData(out T sceneManagerData, string filename)
        {
#if !UNITY_EDITOR
            Debug.LogErrorFormat("No SceneManagerData file founded. Can't create one in Resources.");
            return;
#else

            Debug.LogFormat("<color=yellow>MultiScene</color> # SceneManager data file has been created at path Assets/Resources/.");

            Assert.IsNull(_instance, "Instance should be null while creating manager");

            sceneManagerData = ScriptableObject.CreateInstance<T>();

            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");

            AssetDatabase.CreateAsset(sceneManagerData, "Assets/Resources/" + filename + ".asset");
            AssetDatabase.SaveAssets();
#endif
        }
        #endregion
    }
}
