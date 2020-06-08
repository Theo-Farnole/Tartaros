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
