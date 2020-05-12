using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TF.MultiSceneManager.Editor
{
    [CustomPropertyDrawer(typeof(SceneInput))]
    public class SceneInputDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var sceneAsset = property.FindPropertyRelative("sceneAsset");
            var sceneName = property.FindPropertyRelative("sceneName");

            Rect customRect = position;
            customRect.height = EditorGUIUtility.singleLineHeight;
            sceneAsset.objectReferenceValue = EditorGUI.ObjectField(customRect, property.displayName, sceneAsset.objectReferenceValue, typeof(SceneAsset), false);

            EditorGUI.EndProperty();

            if (sceneAsset != null)
            {

                string name = (sceneAsset.objectReferenceValue as SceneAsset).name;
                sceneName.stringValue = name;

                Debug.Log("name " + name);
            }
        }
    }
}
