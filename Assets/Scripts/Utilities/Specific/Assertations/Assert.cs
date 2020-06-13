using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TF.Assertations
{
    public static class Assert
    {
        public static void AreEquals(object expected, object current, string message)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            if (expected != current)
            {
                Debug.LogErrorFormat("Assert : " + message);
            }
#endif
        }

        public static void HasComponent<T>(Component monoBehaviour, string message, params string[] args) where T : Component
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            HasComponent<T>(monoBehaviour.gameObject, message);
#endif
        }

        public static void HasComponent<T>(GameObject gameObject, string message, params string[] args) where T : Component
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            HasComponent<T>(gameObject, string.Format(message, args));
#endif
        }
            
        public static void HasComponent<T>(GameObject gameObject, string message) where T : Component
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            if (!gameObject.GetComponent((typeof(T))))
            {
                Debug.LogError(message);
            }
#endif
        }

        public static void IsTrue(bool expected, string message)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            if (!expected)
            {
                Debug.LogError("Assert : " + message);
            }
#endif
        }

        public static void IsTrue(bool expected, string message, params string[] args)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            if (!expected)
            {
                Debug.LogErrorFormat("Assert : " + message, args);
            }
#endif
        }
    }
}
