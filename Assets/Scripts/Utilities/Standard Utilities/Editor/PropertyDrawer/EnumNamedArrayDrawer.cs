using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <author>
/// https://answers.unity.com/questions/1589226/showing-an-array-with-enum-as-keys-in-the-property.html
/// </author>

namespace Lortedo.Utilities.Inspector
{
    [CustomPropertyDrawer(typeof(EnumNamedArrayAttribute))]
    public class EnumNamedArrayDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, true);
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EnumNamedArrayAttribute enumNames = attribute as EnumNamedArrayAttribute;
            //propertyPath returns something like component_hp_max.Array.data[4]
            //so get the index from there
            int index = System.Convert.ToInt32(property.propertyPath.Substring(property.propertyPath.IndexOf("[")).Replace("[", "").Replace("]", ""));
            //change the label
            if (index < enumNames.names.Length)
            {
                label.text = enumNames.names[index];
            }
            //draw field
            EditorGUI.PropertyField(rect, property, label, true);
        }
    }
}