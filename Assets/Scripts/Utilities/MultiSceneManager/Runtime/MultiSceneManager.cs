using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnitySceneManager = UnityEngine.SceneManagement;

namespace TF.MultiSceneManager
{
    public delegate void OnSceneActivation();

    /// <summary>
    /// SceneManager that load scene w/ logics scenes.
    /// </summary>
    public static class MultiSceneManager
    {
        #region Fields
        private static bool _allowSceneActivation = false;

        private static List<AsyncOperation> _asyncLoad = new List<AsyncOperation>();
        private static List<AsyncOperation> _asyncUnload = new List<AsyncOperation>();
        #endregion

        #region Properties
        public static bool AllowSceneActivation
        {
            get => _allowSceneActivation; set
            {
                _allowSceneActivation = value;
                UpdateAsyncOperation_AllowSceneActivation();
            }
        }
        #endregion

        #region Methods
        #region Public Methods
        /// <summary>
        /// Reload current scene with logic scenes.
        /// </summary>
        public static void ReloadScene()
        {
            LoadScene(UnitySceneManager.SceneManager.GetActiveScene().name);
        }

        /// <summary>
        /// Reload current scene with logic scenes asynchronously.
        /// </summary>
        public static void ReloadSceneAsync()
        {
            LoadSceneAsync(UnitySceneManager.SceneManager.GetActiveScene().name);
        }

        /// <summary>
        /// Load scene with logic scenes.
        /// </summary>
        /// <param name="masterScene">Level to load</param>
        public static void LoadScene(string masterScene)
        {
            UnitySceneManager.SceneManager.LoadScene(masterScene);

            var additionalScenes = MultiSceneManagerData.Instance.GetSceneNeeds(masterScene);

            foreach (var additionalScene in additionalScenes)
            {
                UnitySceneManager.SceneManager.LoadScene(additionalScene, UnitySceneManager.LoadSceneMode.Additive);
            }
        }

        /// <summary>
        /// Load scene with logic scenes asynchronously.
        /// </summary>
        /// <param name="masterScene">Level to load</param>
        public static void LoadSceneAsync(string masterScene)
        {
            // load master scene
            UnitySceneManager.SceneManager.LoadScene(masterScene);
            
            _asyncLoad.Clear();
            var additionalScenes = MultiSceneManagerData.Instance.GetSceneNeeds(masterScene);

            // load additional scenes
            foreach (var additionalScene in additionalScenes)
            {
                AsyncOperation ao = UnitySceneManager.SceneManager.LoadSceneAsync(additionalScene, UnitySceneManager.LoadSceneMode.Additive);
                _asyncLoad.Add(ao); // keep reference to modify "allowSceneActivation"
            }

            UpdateAsyncOperation_AllowSceneActivation();
        }

        public static bool IsSceneLoad(string sceneToCheck)
        {
            // browse loaded scene
            for (int i = 0; i < UnitySceneManager.SceneManager.sceneCount; i++)
            {
                var loadedScene = UnitySceneManager.SceneManager.GetSceneAt(i);
                
                if (loadedScene.name == sceneToCheck)
                    return true;
            }

            return false;
        }
        #endregion

        #region Private methods        
        static void UpdateAsyncOperation_AllowSceneActivation()
        {
            foreach (var ao in _asyncLoad)
            {
                ao.allowSceneActivation = _allowSceneActivation;
            }

            foreach (var ao in _asyncUnload)
            {
                ao.allowSceneActivation = _allowSceneActivation;
            }
        }
        #endregion
        #endregion
    }
}
