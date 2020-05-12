using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TF.MultiSceneManager
{
    [System.Serializable]
    public class SceneRule
    {
        public SceneInput master;
        public bool dontIncludeDefaultAdditionalScenes = false;

        public SceneInput[] additionalScenes;
    }
}
