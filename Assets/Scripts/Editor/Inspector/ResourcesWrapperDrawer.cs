

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
        EditorGUI.indentLevel++;
        EditorGUILayout.BeginHorizontal();

        foreach (Resource resource in Enum.GetValues(typeof(Resource)))
            DrawResource(property, resource);
        

        EditorGUILayout.EndHorizontal();
        EditorGUI.indentLevel--;
    }

    private static void DrawResource(SerializedProperty property, Resource resource)
    {
        GUILayoutOption[] options = { GUILayout.MaxWidth(100.0f), GUILayout.MinWidth(10.0f) };

        // resource label
        EditorGUILayout.LabelField(resource.ToString(), options);

        // int label
        var resourceProperty = property.FindPropertyRelative(resource.ToString().ToLower());
        var resourceCount = resourceProperty.intValue;

        resourceProperty.intValue = EditorGUILayout.IntField(resourceCount, options);
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

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 0;
    }
}