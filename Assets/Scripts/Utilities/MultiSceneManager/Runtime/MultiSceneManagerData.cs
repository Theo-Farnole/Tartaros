using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Scripting;

namespace TF.MultiSceneManager
{
    public class MultiSceneManagerData : SingletonScriptableObject<MultiSceneManagerData>, IPreprocessBuildWithReport
    {
        #region Fields
#if UNITY_EDITOR
        [Header("DEFAULTS")]
        [SerializeField] private SceneAsset[] _defaultAddionalScenesAssets = new SceneAsset[0];
#endif
        [Header("RULES")]
        [SerializeField] private SceneRule[] _rules = new SceneRule[0];

        [SerializeField, HideInInspector] private string[] _defaultAdditionalScenes = new string[0];
        #endregion

        #region Methods
        #region Get Scene Needs
        #region Public Methods
        /// <summary>
        /// Returns every scenes' name the sceneToLoad needs.
        /// </summary>
        public string[] GetSceneNeeds(string sceneToLoad)
        {
            SceneRule[] correspondingRules = _rules.Where(x => x.master.sceneName == sceneToLoad).ToArray();

            List<string> output = new List<string>(0);

            // warning, the following functions are not optimized.
            // we could use only one foreach loop (that could be a for loop)
            // however, this code is rarely accessed in game and that's more maintanable

            ThrowErrorIfRulesColliding(sceneToLoad, correspondingRules);

            AddDefaultAdditionalScene(correspondingRules, ref output);

            AddAdditionalScenesFromRule(correspondingRules, ref output);

            return output.ToArray();
        }
        #endregion

        #region Private methods
        private void AddDefaultAdditionalScene(SceneRule[] correspondingRules, ref List<string> output)
        {
            if (correspondingRules == null || correspondingRules.Length == 0)
            {
                output.AddRange(_defaultAdditionalScenes);
            }
            else
            {
                foreach (var rule in correspondingRules)
                {
                    if (!rule.dontIncludeDefaultAdditionalScenes)
                    {
                        output.AddRange(_defaultAdditionalScenes);
                        continue;
                    }
                }
            }
        }

        private void AddAdditionalScenesFromRule(IEnumerable<SceneRule> correspondingRules, ref List<string> output)
        {
            foreach (var rule in correspondingRules)
            {
                output.AddRange(rule.additionalScenes.Select(x => x.sceneName));
            }
        }

        /// <summary>
        /// Display error if dontIncludeDefaultAdditionalScene parameter collide.
        /// </summary>
        /// <param name="sceneToLoad"></param>
        /// <param name="correspondingRules"></param>
        /// <returns></returns>
        private static bool ThrowErrorIfRulesColliding(string sceneToLoad, IEnumerable<SceneRule> correspondingRules)
        {
            SceneRule previousRule = null;

            foreach (var rule in correspondingRules)
            {
                if (previousRule != null && rule.dontIncludeDefaultAdditionalScenes != previousRule.dontIncludeDefaultAdditionalScenes)
                {
                    Debug.LogErrorFormat("Rules of scene {0} are colliding with parameter {1}.", sceneToLoad, "dontIncludeDefaultAdditionalScenes");
                    return true;
                }

                previousRule = rule;
            }

            return false;
        }
        #endregion
        #endregion

        #region SceneAssets to scenes' name
#if UNITY_EDITOR
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            ScenesAssetsToName();
        }

        void OnValidate()
        {
            ScenesAssetsToName();
        }

        void ScenesAssetsToName()
        {
            if (_defaultAdditionalScenes != null)
                _defaultAdditionalScenes = _defaultAddionalScenesAssets.Where(x => x != null).Select(x => x.name).ToArray();
        }
#endif
        #endregion
        #endregion
    }
}

