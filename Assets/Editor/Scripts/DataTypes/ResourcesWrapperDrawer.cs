

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ResourcesWrapper))]
public class ResourcesWrapperDrawer : PropertyDrawer
{
    public static Dictionary<Resource, Texture> _icons = new Dictionary<Resource, Texture>();    

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        LoadIcons();

        // ===
        // prefix
        EditorGUILayout.PrefixLabel(label);

        // ===
        // content
        GUILayout.BeginHorizontal();

        foreach (Resource resource in Enum.GetValues(typeof(Resource)))
        {
            // resource label
            GUILayout.Label(resource.ToString());
            //var iconStyle = new GUIStyle
            //{
            //    fixedHeight = EditorGUIUtility.singleLineHeight,
            //    fixedWidth = EditorGUIUtility.singleLineHeight
            //};

            //GUILayout.Box(_icons[resource], iconStyle);

            // int label
            var resourceProperty = property.FindPropertyRelative(resource.ToString().ToLower());
            var resourceCount = resourceProperty.intValue;

            resourceProperty.intValue = EditorGUILayout.IntField(resourceCount);
        }

        GUILayout.EndHorizontal();
    }

    private static void LoadIcons()
    {
        foreach (Resource resource in Enum.GetValues(typeof(Resource)))
        {
            // if resource's icon is already loaded, skip it
            if (_icons.ContainsKey(resource))
            {
                if (_icons[resource] == null) _icons.Remove(resource);
                else continue;
            }

            Texture tex = Resources.Load<Texture>("Sprites/Icons/" + resource.ToString()) as Texture;
            _icons.Add(resource, tex);
        }
    }
}